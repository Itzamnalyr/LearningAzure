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

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class PartRelationshipsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetPartRelationshipsIntegrationWithCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/partrelationships/getpartrelationships?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<PartRelationships> items = await response.Content.ReadAsAsync<IEnumerable<PartRelationships>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().PartRelationshipId > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().ChildPartNum.Length > 0); //The child item has an name
            Assert.IsTrue(items.FirstOrDefault().ParentPartNum.Length > 0); //The parent item has an name
        }

        [TestMethod]
        public async Task GetPartRelationshipsIntegrationWithoutCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/partrelationships/getpartrelationships?useCache=false");
            response.EnsureSuccessStatusCode();
            IEnumerable<PartRelationships> items = await response.Content.ReadAsAsync<IEnumerable<PartRelationships>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().PartRelationshipId > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().ChildPartNum.Length > 0); //The child item has an name
            Assert.IsTrue(items.FirstOrDefault().ParentPartNum.Length > 0); //The parent item has an name
        }
        
    }
}
