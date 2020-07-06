using System;
using OTP.Business.Interfaces;
using OTP.DataAccess.DataContext;
using OTP.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace OTP.Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;

        public TokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddToken(string userId, string token, DateTime creationDate)
        {
            _context.Database.ExecuteSqlCommand("EXEC dbo.AddToken {0}, {1}, {2}",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Token", token),
                new SqlParameter("@CreationDate", creationDate));
        }

        public void DeleteToken(string userId)
        {
            _context.Database.ExecuteSqlCommand("EXEC dbo.DeleteToken {0}",
                new SqlParameter("@UserId", userId));
        }

        public TokenExpiration GetTokenByUserId(string userId)
        {
            var token = _context.UserRefreshTokens
                .FromSqlRaw("EXECUTE dbo.GetTokenByUserId {0}", userId);

            if(token.ToListAsync().Result.Count == 0)
            {
                return null;
            }

            return token.ToListAsync().Result[0];
        }
    }
}
