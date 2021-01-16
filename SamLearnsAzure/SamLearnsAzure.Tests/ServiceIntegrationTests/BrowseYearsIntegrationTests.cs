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
    public class BrowseYearsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetBrowseYearsIntegrationWithCacheTest()
        {

            if (base.Client != null)
            {
                //Arrange
                int? themeId = 158;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/BrowseYears/getBrowseYears?useCache=true&themeId=" + themeId);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<BrowseYears> items = JsonConvert.DeserializeObject<IEnumerable<BrowseYears>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().Year > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().YearName?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetBrowseYearsIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int? themeId = 158;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/BrowseYears/getBrowseYears?useCache=false&themeId=" + themeId);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<BrowseYears> items = JsonConvert.DeserializeObject<IEnumerable<BrowseYears>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().Year > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().YearName?.Length > 0); //The first item has an name
            }
        }

    }
}
