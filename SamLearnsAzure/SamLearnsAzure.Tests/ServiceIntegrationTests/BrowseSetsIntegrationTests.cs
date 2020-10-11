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
    public class BrowseSetsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetBrowseSetsIntegrationWithCacheTest()
        {

            if (base.Client != null)
            {
                //Arrange
                int? themeId = null;
                int? year = null;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/browsesets/getBrowseSets?useCache=true&themeId=" + themeId + "&year=" + year);
                response.EnsureSuccessStatusCode();
                IEnumerable<BrowseSets> items = await response.Content.ReadAsAsync<IEnumerable<BrowseSets>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetBrowseSetsIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int? themeId = null;
                int? year = null;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/browsesets/getBrowseSets?useCache=false&themeId=" + themeId + "&year=" + year);
                response.EnsureSuccessStatusCode();
                IEnumerable<BrowseSets> items = await response.Content.ReadAsAsync<IEnumerable<BrowseSets>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetBrowseSetsStarWarsIntegrationWithCacheTest()
        {

            if (base.Client != null)
            {
                //Arrange
                int? themeId = 158;
                int? year = 2020;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/browsesets/getBrowseSets?useCache=true&themeId=" + themeId + "&year=" + year);
                response.EnsureSuccessStatusCode();
                IEnumerable<BrowseSets> items = await response.Content.ReadAsAsync<IEnumerable<BrowseSets>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetBrowseSetsStarWarsIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int? themeId = 158;
                int? year = 2020;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/browsesets/getBrowseSets?useCache=false&themeId=" + themeId + "&year=" + year);
                response.EnsureSuccessStatusCode();
                IEnumerable<BrowseSets> items = await response.Content.ReadAsAsync<IEnumerable<BrowseSets>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

    }
}
