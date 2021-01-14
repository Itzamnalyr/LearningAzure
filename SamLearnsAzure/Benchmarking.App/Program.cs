using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace Benchmarking.App
{
    public class Program
    {
        private static IConfiguration Config;

        [Benchmark]
        public async Task ColorsRepo()
        {
            //Arrange
            ColorsRepository repo = new ColorsRepository(Config);

            //Act
            IEnumerable<Colors> colors = await repo.GetColors(null, false);
            Console.WriteLine("Colors count: " + colors.Count());
        }

        [Benchmark]
        public async Task OwnersRepo()
        {
            //Arrange
            OwnersRepository repo = new OwnersRepository(Config);

            //Act
            IEnumerable<Owners> owners = await repo.GetOwners(null, false);
            Console.WriteLine("Owners count: " + owners.Count());
        }

        [Benchmark]
        public async Task OwnerSetsRepo()
        {
            //Arrange
            OwnerSetsRepository repo = new OwnerSetsRepository(Config);
            int ownerSet = 1;

            //Act
            IEnumerable<OwnerSets> ownerSets = await repo.GetOwnerSets(null, false, ownerSet);
            Console.WriteLine("Owner sets count: " + ownerSets.Count());
        }

        [GlobalSetup]
        public void Setup()
        {
            Console.WriteLine("Configuration starting");
            Config = GetConnectionString();
            Console.WriteLine("Configuration done");
        }

        public static void Main(string[] args)
        {
            //dotnet run -p Benchmarking.App.csproj -c Release
            var summary = BenchmarkRunner.Run<Program>();
        }

        private static IConfiguration GetConnectionString()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
                           .SetBasePath(AppContext.BaseDirectory)
                           .AddJsonFile("appsettings.json")
                           .AddUserSecrets<Program>();
            IConfiguration Configuration = config.Build();

            string azureKeyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:ClientId"];
            string clientSecret = Configuration["AppSettings:ClientSecret"];
            config.AddAzureKeyVault(azureKeyVaultURL, clientId, clientSecret);
            Configuration = config.Build();

            return Configuration;
        }
    }
}
