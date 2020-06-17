using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class SetsRepoServiceIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetNullSetNumTest()
        {
            if (base.Configuration != null)
            {
                //Arrange
                SetsRepository repo = new SetsRepository(base.Configuration);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                Sets set = await repo.GetSet(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(set != null);
                Assert.AreNotEqual(null, set?.SetNum);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
