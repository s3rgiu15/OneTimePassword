using OTP.DataAccess.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace OTP.Business.Services.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static void ConfigureApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            ServiceApplicationContextConfiguration.ConfigureIdentityContext(services, connectionString);
        }
    }
}
