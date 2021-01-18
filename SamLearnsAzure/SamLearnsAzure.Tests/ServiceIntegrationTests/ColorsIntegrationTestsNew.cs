using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using StackExchange.Redis;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestA"), TestCategory("Colors")]
    public class ColorsIntegrationTestsNew //: BaseIntegrationTestNew
    {
        public IConfigurationRoot? Configuration;
        public IDatabase? RedisDatabase;
        public Stopwatch Watch;

        [TestMethod]
        public async Task GetColorsIntegrationTestWithoutCacheNew()
        {
            //Prep
            Watch = new Stopwatch();
            Watch.Start();
            Debug.WriteLine("Starting base setup-config: " + Watch.Elapsed.TotalSeconds);

            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<BaseIntegrationTest>();
            Configuration = config.Build();

            Debug.WriteLine("Starting base setup-keyvault: " + Watch.Elapsed.TotalSeconds);
            string azureKeyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:ClientId"];
            string clientSecret = Configuration["AppSettings:ClientSecret"];

            config.AddAzureKeyVault(azureKeyVaultURL, clientId, clientSecret);
            Configuration = config.Build();

            Debug.WriteLine("Starting base setup-redis: " + Watch.Elapsed.TotalSeconds);
            string redisConnectionStringName = "AppSettings:RedisCacheConnectionString" + Configuration["AppSettings:Environment"];
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(Configuration[redisConnectionStringName]);
            if (connectionMultiplexer != null)
            {
                RedisDatabase = connectionMultiplexer.GetDatabase();
            }


            //Arrange
            Debug.WriteLine("Starting test: " + Watch.Elapsed.TotalSeconds);
            ColorsController? controller = null;
            IEnumerable<Colors>? items = null;
            if (Configuration != null)
            {
                ColorsRepository repo = new ColorsRepository(Configuration);
                RedisService redis = new RedisService(RedisDatabase);
                controller = new ColorsController(repo, redis);
            }
            else
            {
                Assert.IsTrue("Redis Configuration is invalid" == "");
            }
            Debug.WriteLine("Arrange complete: " + Watch.Elapsed.TotalSeconds);

            //Act
            if (controller != null)
            {
                items = await controller.GetColors();
            }
            Debug.WriteLine("Act complete: " + Watch.Elapsed.TotalSeconds);

            //Assert
            Assert.IsTrue(controller != null);
            Assert.IsTrue(items != null);
            Assert.IsTrue(items?.Any() == true); //There is more than one
            Assert.IsTrue(items?.FirstOrDefault()?.Id > 0); //The first item has an id
            Assert.IsTrue(items?.FirstOrDefault()?.Name?.Length > 0); //The first item has an name
            Debug.WriteLine("Assert complete: " + Watch.Elapsed.TotalSeconds);
        }

    }
}
