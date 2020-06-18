using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SamLearnsAzure.Service.EFCore;
using StackExchange.Redis;
using System;
using System.Net.Http;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BaseRepoIntegrationTest
    {
        public IConfigurationRoot? Configuration;
       // public DbContextOptions<SamsAppDBContext>? DbOptions;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<BaseIntegrationTest>();
            Configuration = config.Build();

            string azureKeyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:ClientId"];
            string clientSecret = Configuration["AppSettings:ClientSecret"];
            //AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            //KeyVaultClient keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //config.AddAzureKeyVault(azureKeyVaultURL, keyVaultClient, new DefaultKeyVaultSecretManager());
            config.AddAzureKeyVault(azureKeyVaultURL, clientId, clientSecret);
            Configuration = config.Build();

            //Setup the database options
            //string sqlConnectionStringName = "ConnectionStrings:SamsAppConnectionString" + Configuration["AppSettings:Environment"];
            //DbOptions = new DbContextOptionsBuilder<SamsAppDBContext>()
            //                .UseSqlServer(Configuration[sqlConnectionStringName])
            //                .Options;
        }
    }
}
