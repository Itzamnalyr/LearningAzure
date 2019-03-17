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
    public class ServiceAPIClientIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetServiceAPIClientOwnerSetsIntegrationTest()
        {
            //Arrange
            ServiceAPIClient client = new ServiceAPIClient(base.Configuration);
            int ownerId = 1;

            //Act
            List<OwnerSets> ownerSets = await client.GetOwnerSets(ownerId);


            //Assert
            Assert.IsTrue(ownerSets.Count() >= 1);
        }

        [TestMethod]
        public async Task GetServiceAPIClientSetIntegrationTest()
        {
            //Arrange
            ServiceAPIClient client = new ServiceAPIClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            Sets set = await client.GetSet(setNum);


            //Assert
            Assert.IsTrue(set != null);
        }

        [TestMethod]
        public async Task GetServiceAPIClientSetPartsIntegrationTest()
        {
            //Arrange
            ServiceAPIClient client = new ServiceAPIClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            List<SetParts> setParts = await client.GetSetParts(setNum);


            //Assert
            Assert.IsTrue(setParts.Count() >= 1);
        }




    }
}
