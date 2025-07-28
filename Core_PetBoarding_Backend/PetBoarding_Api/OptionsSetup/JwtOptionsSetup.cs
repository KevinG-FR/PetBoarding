using Microsoft.Extensions.Options;

using PetBoarding_Infrastructure.Options;

namespace PetBoarding_Api.OptionsSetup
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string SectionName = "JwtOptions";
        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection(SectionName).Bind(options);
        }
    }
}
