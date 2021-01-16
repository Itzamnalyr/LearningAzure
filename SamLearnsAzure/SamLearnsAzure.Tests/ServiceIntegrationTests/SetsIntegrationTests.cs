using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestB")]
    public class SetsServiceIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetSetsTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getsets");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<Sets> items = JsonConvert.DeserializeObject<IEnumerable<Sets>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetSetWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75218-1";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getset?setnum=" + setNum + "&useCache=true");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                Sets set = JsonConvert.DeserializeObject<Sets>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(set != null);
                Assert.IsTrue(set?.SetNum == setNum);
                Assert.IsTrue(set?.Theme != null); //We are including this in the repo, so want to test it specifically
            }
        }

        [TestMethod]
        public async Task GetSetWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75218-1";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getset?setnum=" + setNum + "&useCache=false");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                Sets set = JsonConvert.DeserializeObject<Sets>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(set != null);
                Assert.IsTrue(set?.SetNum == setNum);
                Assert.IsTrue(set?.Theme != null); //We are including this in the repo, so want to test it specifically
            }
        }
        
    }
}
