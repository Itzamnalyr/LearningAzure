using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    [TestCategory("RedisTest")]
    public class OwnersServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetOwnersIntegrationWithCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowners?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<Owners> items = await response.Content.ReadAsAsync<IEnumerable<Owners>>();
            response.Dispose();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Any()); //There is more than one owner
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first owner has an id
            Assert.IsTrue(items.FirstOrDefault().OwnerName.Length > 0); //The first owner has an name
        }

        [TestMethod]
        public async Task GetOwnersIntegrationWithoutCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowners?useCache=false");
            response.EnsureSuccessStatusCode();
            IEnumerable<Owners> items = await response.Content.ReadAsAsync<IEnumerable<Owners>>();
            response.Dispose();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Any()); //There is more than one owner
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first owner has an id
            Assert.IsTrue(items.FirstOrDefault().OwnerName.Length > 0); //The first owner has an name
        }

        [TestMethod]
        public async Task GetOwnerIntegrationWithCacheTest()
        {
            //Arrange
            int ownerId = 1;

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowner?ownerId=" + ownerId.ToString() + "&useCache=true");
            response.EnsureSuccessStatusCode();
            Owners item = await response.Content.ReadAsAsync<Owners>();
            response.Dispose();

            //Assert
            Assert.IsTrue(item != null);
            Assert.IsTrue(item.Id > 0); //The owner has an id
            Assert.IsTrue(item.OwnerName.Length > 0); //The owner has an name
        }

        [TestMethod]
        public async Task GetOwnerIntegrationWithoutCacheTest()
        {
            //Arrange
            int ownerId = 1;

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowner?ownerId=" + ownerId.ToString() + "&useCache=false");
            response.EnsureSuccessStatusCode();
            Owners item = await response.Content.ReadAsAsync<Owners>();
            response.Dispose();

            //Assert
            Assert.IsTrue(item != null);
            Assert.IsTrue(item.Id > 0); //The owner has an id
            Assert.IsTrue(item.OwnerName.Length > 0); //The owner has an name
        }

    }
}
