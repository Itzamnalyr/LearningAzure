using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class SetImagesUnitTests : BaseUnitTest
    {

        [TestMethod]
        public async Task GetSetImageMockTest()
        {
            //Arrange
            Mock<ISetImagesRepository> mock = new Mock<ISetImagesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mock.Setup(repo => repo.GetSetImage(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(GetSetImageTestData()));
            SetImagesController controller = new SetImagesController(mock.Object, mockRedis.Object, mockConfig.Object);
            string setNum = "75218-1";

            //Act
            SetImages setImage = await controller.GetSetImage(setNum);

            //Assert
            Assert.IsTrue(setImage != null);
            TestSetImages(setImage ?? new SetImages());
        }

        private void TestSetImages(SetImages setImage)
        {
            Assert.IsTrue(setImage.SetNum == "abc");
            Assert.IsTrue(setImage.SetImage == "def");
            Assert.IsTrue(setImage.SetImageId == 1);
        }

        private SetImages GetSetImageTestData()
        {
            return new SetImages()
            {
                SetNum = "abc",
                SetImage = "def",
                SetImageId = 1
            };
        }

    }
}
