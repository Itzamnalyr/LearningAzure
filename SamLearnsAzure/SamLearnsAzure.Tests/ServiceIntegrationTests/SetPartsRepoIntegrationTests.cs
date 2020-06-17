using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class SetPartsRepoIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetPartsNullSetNumTest()
        {
            if (base.Configuration != null)
            {
                //Arrange
                SetPartsRepository repo = new SetPartsRepository(base.Configuration);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                IEnumerable<SetParts> setParts = await repo.GetSetParts(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(setParts != null);
                Assert.IsTrue(setParts.Count() == 0);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
