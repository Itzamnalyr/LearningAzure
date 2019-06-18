using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class ValuesIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetValuesTestServerTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/values");
            response.EnsureSuccessStatusCode();
            IEnumerable<string> items = await response.Content.ReadAsAsync<IEnumerable<string>>();
            response.Dispose();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 2);
            Assert.IsTrue(items.FirstOrDefault<string>() == "value1");
            Assert.IsTrue(items.ElementAt<string>(1) == "value2");
        }

    }
}
