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
    public class OwnersServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetOwnersIntegrationTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowners?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<Owners> items = await response.Content.ReadAsAsync<IEnumerable<Owners>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one owner
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first owner has an id
            Assert.IsTrue(items.FirstOrDefault().OwnerName.Length > 0); //The first owner has an name
        }

        [TestMethod]
        public async Task GetOwnerIntegrationTest()
        {
            //Arrange
            int ownerId = 1;

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/owners/getowner?ownerId=" + ownerId.ToString() + "&useCache=true");
            response.EnsureSuccessStatusCode();
            Owners item = await response.Content.ReadAsAsync<Owners>();

            //Assert
            Assert.IsTrue(item != null);
            Assert.IsTrue(item.Id > 0); //The owner has an id
            Assert.IsTrue(item.OwnerName.Length > 0); //The owner has an name
        }

    }
}
