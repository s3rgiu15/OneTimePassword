using Microsoft.Extensions.Configuration;
using System.IO;

namespace OTP.DataAccess.DataContext
{
    public class AppConfig
    {
        public string sqlConnectionString { get; set; }

        public AppConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var configurationPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(configurationPath, false);
            var configurationRoot = configurationBuilder.Build();
            sqlConnectionString = configurationRoot.GetSection("ConnectionStrings:DefaultConnection").Value;
        }
    }
}
