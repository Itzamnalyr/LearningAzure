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
    public class InventoryPartsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetInventoryPartsIntegrationTest()
        {
            if (base.Client != null)
            { 
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/inventoryparts/getinventoryparts");
                response.EnsureSuccessStatusCode();
                IEnumerable<InventoryParts> items = await response.Content.ReadAsAsync<IEnumerable<InventoryParts>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().InventoryPartId > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().PartNum.Length > 0); //The partnum item has an name
            }
        }

    }
}
