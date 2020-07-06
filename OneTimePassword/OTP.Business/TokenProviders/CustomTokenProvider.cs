using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using OTP.Business.Interfaces;
using Microsoft.Extensions.Options;
using OTP.Models;

namespace OTP.Business.Services
{
    public class CustomTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        private ITokenService _tokenService;
        private readonly TwoFactorTokenValability _twoFactorTokenConfiguration;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataProtectorTokenProvider{TUser}"/> class.
        /// </summary>
        /// <param name="dataProtectionProvider">The system data protection provider.</param>
        /// <param name="options">The configured <see cref="DataProtectionTokenProviderOptions"/>.</param>
        public CustomTokenProvider(ITokenService tokenService, IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, IOptions<TwoFactorTokenValability> twoFactorTokenValability)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }
            Options = options?.Value ?? new DataProtectionTokenProviderOptions();
            // Use the Name as the purpose which should usually be distinct from others
            Protector = dataProtectionProvider.CreateProtector(Name ?? "DataProtectorTokenProvider");
            _tokenService = tokenService;
            _twoFactorTokenConfiguration = twoFactorTokenValability.Value;
        }

        /// <summary>
        /// Gets the <see cref="DataProtectionTokenProviderOptions"/> for this instance.
        /// </summary>
        /// <value>
        /// The <see cref="DataProtectionTokenProviderOptions"/> for this instance.
        /// </value>
        protected DataProtectionTokenProviderOptions Options { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDataProtector"/> for this instance.
        /// </summary>
        /// <value>
        /// The <see cref="IDataProtector"/> for this instance.
        /// </value>
        protected IDataProtector Protector { get; private set; }

        /// <summary>
        /// Gets the name of this instance.
        /// </summary>
        /// <value>
        /// The name of this instance.
        /// </value>
        public string Name { get { return Options.Name; } }

        /// <summary>
        /// Generates a protected token for the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="purpose">The purpose the token will be used for.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token will be generated from.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the generated token.</returns>
        public virtual async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            var token = await manager.CreateSecurityTokenAsync(user);
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var generatedToken = Rfc6238AuthenticationService.GenerateCode(token, modifier).ToString("D6", System.Globalization.CultureInfo.InvariantCulture);

            var userId = typeof(TUser).GetProperty("Id").GetValue(user).ToString();
            var dateNow = DateTime.UtcNow;
            _tokenService.AddToken(userId, generatedToken, dateNow);

            return generatedToken;
        }

        public virtual async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            var userId = await manager.GetUserIdAsync(user);
            return "Totp:" + purpose + ":" + userId;
        }

        /// <summary>
        /// Validates the protected <paramref name="token"/> for the specified <paramref name="user"/> and <paramref name="purpose"/> as an asynchronous operation.
        /// </summary>
        /// <param name="purpose">The purpose the token was be used for.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token was generated for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous validation,
        /// containing true if the token is valid, otherwise false.
        /// </returns>
        public virtual async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var userId = typeof(TUser).GetProperty("Id").GetValue(user).ToString();
            var tokenObj = _tokenService.GetTokenByUserId(userId);

            if (tokenObj == null)
            {
                return false;
            }

            if (DateTime.UtcNow >= tokenObj.CreationDate.AddSeconds(_twoFactorTokenConfiguration.Seconds))
            {
                return false;
            }

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            int code;
            if (!int.TryParse(token, out code))
            {
                return false;
            }
            var securityToken = await manager.CreateSecurityTokenAsync(user);
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var tokenValid =  securityToken != null && Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier);

            if (tokenValid)
            {
                _tokenService.DeleteToken(userId);
            }

            return tokenValid;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether a token generated by this instance
        /// can be used as a Two Factor Authentication token as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token was generated for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query,
        /// containing true if a token generated by this instance can be used as a Two Factor Authentication token, otherwise false.
        /// </returns>
        /// <remarks>This method will always return false for instances of <see cref="DataProtectorTokenProvider{TUser}"/>.</remarks>
        public virtual Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Utility extensions to streams
    /// </summary>
    internal static class StreamExtensions
    {
        internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryReader CreateReader(this Stream stream)
        {
            return new BinaryReader(stream, DefaultEncoding, true);
        }

        public static BinaryWriter CreateWriter(this Stream stream)
        {
            return new BinaryWriter(stream, DefaultEncoding, true);
        }

        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
        {
            return new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
        }

        public static void Write(this BinaryWriter writer, DateTimeOffset value)
        {
            writer.Write(value.UtcTicks);
        }
    }
}
