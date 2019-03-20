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
    public class OwnerSetsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetOwnerSetsIntegrationWithCacheTest()
        {
            //Arrange
            int ownerId = 1;

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/getownersets?ownerid=" + ownerId + "&useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<OwnerSets> items = await response.Content.ReadAsAsync<IEnumerable<OwnerSets>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one owner
            OwnerSets item = items.FirstOrDefault();
            Assert.IsTrue(item.OwnerSetId > 0); //The first item has an id
            //Assert.IsTrue(item.Owner != null); //Ensure owner has been collected correctly
            //Assert.IsTrue(item.Owner.OwnerName.Length > 0); //The first item has an name  
            Assert.IsTrue(item.Set != null); //Ensure set has been collected correctly
            Assert.IsTrue(item.Set.Theme != null); //Ensure theme has been collected correctly
        }

        [TestMethod]
        public async Task GetOwnerSetsIntegrationWithoutCacheTest()
        {
            //Arrange
            int ownerId = 1;

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/getownersets?ownerid=" + ownerId + "&useCache=false");
            response.EnsureSuccessStatusCode();
            IEnumerable<OwnerSets> items = await response.Content.ReadAsAsync<IEnumerable<OwnerSets>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one owner
            OwnerSets item = items.FirstOrDefault();
            Assert.IsTrue(item.OwnerSetId > 0); //The first item has an id
            //Assert.IsTrue(item.Owner != null); //Ensure owner has been collected correctly
            //Assert.IsTrue(item.Owner.OwnerName.Length > 0); //The first item has an name  
            Assert.IsTrue(item.Set != null); //Ensure set has been collected correctly
            Assert.IsTrue(item.Set.Theme != null); //Ensure theme has been collected correctly
        }

    }
}
