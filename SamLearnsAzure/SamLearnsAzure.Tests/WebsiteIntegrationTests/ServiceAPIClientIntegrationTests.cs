using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using SamLearnsAzure.Models;
using System.Data.SqlClient;
using SamLearnsAzure.Service.Controllers;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using SamLearnsAzure.Service.DataAccess;
using System.Net.Http;
using SamLearnsAzure.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Web.Models;
using Newtonsoft.Json;

namespace SamLearnsAzure.Tests.WebsiteIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class ServiceApiClientIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetServiceApiClientOwnersIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Owners> owners = await client.GetOwners();

            //Assert
            Assert.IsTrue(owners.Any());
        }

        [TestMethod]
        public async Task GetServiceApiClientOwnerSetsIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            int ownerId = 1;

            //Act
            List<OwnerSets> ownerSets = await client.GetOwnerSets(ownerId);

            //Assert
            Assert.IsTrue(ownerSets.Any());
        }

        [TestMethod]
        public async Task GetServiceApiClientSetsIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Sets> sets = await client.GetSets();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Any());
        }

        [TestMethod]
        public async Task GetServiceApiClientSetIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            Sets set = await client.GetSet(setNum);

            //Assert
            Assert.IsTrue(set != null);
        }

        [TestMethod]
        public async Task GetServiceApiClientSetImageIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            SetImages setImage = await client.GetSetImage(setNum);

            //Assert
            Assert.IsTrue(setImage != null);
        }

        [TestMethod]
        public async Task GetServiceApiClientSetPartsIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            List<SetParts> setParts = await client.GetSetParts(setNum);

            //Assert
            Assert.IsTrue(setParts.Any());
        }

        [TestMethod]
        public async Task GetServiceApiClientThemesIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Themes> themes = await client.GetThemes();

            //Assert
            Assert.IsTrue(themes.Any());
        }

    }
}
