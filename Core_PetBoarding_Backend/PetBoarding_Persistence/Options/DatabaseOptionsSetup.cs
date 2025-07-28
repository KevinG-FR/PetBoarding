using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace PetBoarding_Persistence.Options
{
    public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
    {
        private const string ConfigurationSectionName = "DatabaseOptions";
        private readonly IConfiguration _configuration;

        public DatabaseOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(DatabaseOptions options)
        {
            var connectionString = _configuration.GetConnectionString("Database");
            options.ConnectionString = connectionString ?? string.Empty;

            _configuration.GetSection(ConfigurationSectionName).Bind(options);
        }
    }
}