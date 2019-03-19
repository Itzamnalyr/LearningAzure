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
    public class ColorsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetColorsIntegrationWithCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/colors/getcolors?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<Colors> items = await response.Content.ReadAsAsync<IEnumerable<Colors>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }

        [TestMethod]
        public async Task GetColorsIntegrationWithoutCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/colors/getcolors?useCache=false");
            response.EnsureSuccessStatusCode();
            IEnumerable<Colors> items = await response.Content.ReadAsAsync<IEnumerable<Colors>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }

    }
}
