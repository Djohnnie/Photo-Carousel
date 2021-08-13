using Microsoft.Extensions.Configuration;

namespace PhotoCarousel.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        const string DB_CONNECTION_STRING = nameof(DB_CONNECTION_STRING);

        public static string GetConnectionString(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(DB_CONNECTION_STRING);
        }
    }
}