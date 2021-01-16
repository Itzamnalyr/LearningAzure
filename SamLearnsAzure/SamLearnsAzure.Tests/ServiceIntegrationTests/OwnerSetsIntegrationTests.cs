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
    public class OwnerSetsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetOwnerSetsIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int ownerId = 1;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/getownersets?ownerid=" + ownerId + "&useCache=true");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<OwnerSets> items = JsonConvert.DeserializeObject<IEnumerable<OwnerSets>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one owner
                OwnerSets item = items.FirstOrDefault();
                Assert.IsTrue(item.OwnerSetId > 0); //The first item has an id
                //Assert.IsTrue(item.Owner != null); //Ensure owner has been collected correctly
                //Assert.IsTrue(item.Owner.OwnerName.Length > 0); //The first item has an name  
                //Assert.IsTrue(item.SetName != null);
                //Assert.IsTrue(item.SetYear >= 0);
                //Assert.IsTrue(item.SetNumParts >= 0);
                //Assert.IsTrue(item.SetThemeName != null);
            }
        }

        [TestMethod]
        public async Task GetOwnerSetsIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int ownerId = 1;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/getownersets?ownerid=" + ownerId + "&useCache=false");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<OwnerSets> items = JsonConvert.DeserializeObject<IEnumerable<OwnerSets>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one owner
                OwnerSets item = items.FirstOrDefault();
                Assert.IsTrue(item.OwnerSetId > 0); //The first item has an id
                                                    //Assert.IsTrue(item.Owner != null); //Ensure owner has been collected correctly
                                                    //Assert.IsTrue(item.Owner.OwnerName.Length > 0); //The first item has an name  
                //Assert.IsTrue(item.SetName != null);
                //Assert.IsTrue(item.SetYear >= 0);
                //Assert.IsTrue(item.SetNumParts >= 0);
                //Assert.IsTrue(item.SetThemeName != null);
            }
        }

        [TestMethod]
        public async Task SaveOwnerSetsIntegrationTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75218-1";
                int ownerId = 1;
                bool owned = true;
                bool wanted = true;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/SaveOwnerSet?setnum=" + setNum + "&ownerid=" + ownerId + "&owned=" + owned + "&wanted=" + wanted);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                bool result = JsonConvert.DeserializeObject<bool>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(result == true);
            }
        }

    }
}
