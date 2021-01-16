using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestB")]
    public class SetImagesRepoIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetImageNullSetNumTest()
        {
            if (base.Configuration != null)
            {
                //Arrange
                SetImagesRepository repo = new SetImagesRepository(base.Configuration);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                SetImages setImage = await repo.GetSetImage(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(setImage != null);
                Assert.AreEqual(null, setImage?.SetNum);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
