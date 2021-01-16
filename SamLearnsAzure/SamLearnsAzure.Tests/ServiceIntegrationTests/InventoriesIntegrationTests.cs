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
using Newtonsoft.Json;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestA")]
    public class InventoriesServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetInventoriesIntegrationTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/inventories/getinventories");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<Inventories> items = JsonConvert.DeserializeObject<IEnumerable<Inventories>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().SetNum?.Length > 0); //The setnum item has an name
            }
        }

    }
}
