using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace SamLearnsAzure.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationRoot buildConfig;
            bool captureStartupErrors = false;

            IHostBuilder host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    //Load the appsettings.json configuration file
                    buildConfig = config.Build();

                    //Extract the capture start errors value from appsettings
                    bool.TryParse(buildConfig["AppSettings:CaptureStartErrors"], out captureStartupErrors);

                    //Load a connection to our Azure key vault instance
                    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                    KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    config.AddAzureKeyVault(buildConfig["AppSettings:KeyVaultURL"], keyVaultClient, new DefaultKeyVaultSecretManager());
                });

            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.CaptureStartupErrors(captureStartupErrors);
            });

            return host;
        }
    }
}
