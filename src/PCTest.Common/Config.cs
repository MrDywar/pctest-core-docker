using Microsoft.Extensions.Configuration;
using System.IO;

namespace PCTestCommon
{
    public static class Config
    {
        static Config()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            DOCKER_MACHINE_IP = configuration["docker_machine_ip"];
        }

        public static readonly string DOCKER_MACHINE_IP;
    }
}
