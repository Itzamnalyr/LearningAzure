using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestA")]
    public class OwnersServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetOwnersIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowners?useCache=true");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<Owners> items = JsonConvert.DeserializeObject<IEnumerable<Owners>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one owner
                Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first owner has an id
                Assert.IsTrue(items.FirstOrDefault().OwnerName?.Length > 0); //The first owner has an name
            }
        }

        [TestMethod]
        public async Task GetOwnersIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowners?useCache=false");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<Owners> items = JsonConvert.DeserializeObject<IEnumerable<Owners>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one owner
                Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first owner has an id
                Assert.IsTrue(items.FirstOrDefault().OwnerName?.Length > 0); //The first owner has an name
            }
        }

        [TestMethod]
        public async Task GetOwnerIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int ownerId = 1;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowner?ownerId=" + ownerId.ToString() + "&useCache=true");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                Owners item = JsonConvert.DeserializeObject<Owners>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(item != null);
                Assert.IsTrue(item?.Id > 0); //The owner has an id
                Assert.IsTrue(item?.OwnerName?.Length > 0); //The owner has an name
            }
        }

        [TestMethod]
        public async Task GetOwnerIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int ownerId = 1;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowner?ownerId=" + ownerId.ToString() + "&useCache=false");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                Owners item = JsonConvert.DeserializeObject<Owners>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(item != null);
                Assert.IsTrue(item?.Id > 0); //The owner has an id
                Assert.IsTrue(item?.OwnerName?.Length > 0); //The owner has an name
            }
        }

    }
}
