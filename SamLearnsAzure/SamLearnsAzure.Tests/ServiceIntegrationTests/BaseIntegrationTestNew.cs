using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BaseIntegrationTestNew
    {
        public IConfigurationRoot? Configuration;
        public IDatabase? RedisDatabase;
        public Stopwatch Watch;

        [TestInitialize]
        public void TestStartUp()
        {
            Watch = new Stopwatch();
            Watch.Start();
            Debug.WriteLine("Starting base setup: " + Watch.Elapsed.TotalSeconds);

            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<BaseIntegrationTest>();
            Configuration = config.Build();

            string azureKeyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:ClientId"];
            string clientSecret = Configuration["AppSettings:ClientSecret"];
            
            config.AddAzureKeyVault(azureKeyVaultURL, clientId, clientSecret);
            Configuration = config.Build();

            string redisConnectionStringName = "AppSettings:RedisCacheConnectionString" + Configuration["AppSettings:Environment"];
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(Configuration[redisConnectionStringName]);
            if (connectionMultiplexer != null)
            {
                RedisDatabase = connectionMultiplexer.GetDatabase();
            }
        }
    }
}
