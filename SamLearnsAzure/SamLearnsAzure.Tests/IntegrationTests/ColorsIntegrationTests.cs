using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class ColorsIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetColorsIntegrationTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/colors/getcolors");
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
