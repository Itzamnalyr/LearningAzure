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
    public class InventorySetsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetInventorySetsIntegrationTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/inventorysets/getinventorysets");
            response.EnsureSuccessStatusCode();
            IEnumerable<InventorySets> items = await response.Content.ReadAsAsync<IEnumerable<InventorySets>>();
            response.Dispose();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Any()); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().InventorySetId > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().SetNum.Length > 0); //The set num item has an name
        }
                
    }
}
