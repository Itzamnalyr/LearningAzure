using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.EFCore;
using Microsoft.Extensions.Configuration;
using System;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class PartImagesServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartImagesMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IPartImagesRepository> mock = new Mock<IPartImagesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mock.Setup(repo => repo.GetPartImages(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetPartImagesTestData()));
            PartImagesController controller = new PartImagesController(mock.Object, mockRedis.Object, mockConfig.Object);

            //Act
            IEnumerable<PartImages> partImages = await controller.GetPartImages();

            //Assert
            Assert.IsTrue(partImages != null);
            Assert.IsTrue(partImages.Count() == 1);
            TestPartImages(partImages.FirstOrDefault());
        }

        [TestMethod]
        public async Task GetPartImageMockTest()
        {
            //Arrange
            string partNum = "abc";
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IPartImagesRepository> mock = new Mock<IPartImagesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mock.Setup(repo => repo.GetPartImage(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(GetTestRow()));
            PartImagesController controller = new PartImagesController(mock.Object, mockRedis.Object, mockConfig.Object);

            //Act
            PartImages partImage = await controller.GetPartImage(partNum);

            //Assert
            Assert.IsTrue(partImage != null);
            TestPartImages(partImage);
        }

        [TestMethod]
        public async Task SavePartImageMockTest()
        {
            //Arrange
            string partNum = "abc";
            string sourceImage = "def";
            int colorId = 1;
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IPartImagesRepository> mock = new Mock<IPartImagesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mock.Setup(repo => repo.SavePartImage( It.IsAny<PartImages>())).Returns(Task.FromResult(GetTestRow()));
            PartImagesController controller = new PartImagesController(mock.Object, mockRedis.Object, mockConfig.Object);

            //Act
            PartImages partImage = await controller.SavePartImage(partNum, sourceImage, colorId);

            //Assert
            Assert.IsTrue(partImage != null);
            TestPartImages(partImage);
        }       

        private void TestPartImages(PartImages PartImages)
        {
            Assert.IsTrue(PartImages.PartNum == "abc");
            Assert.IsTrue(PartImages.SourceImageUrl == "def");
            Assert.IsTrue(PartImages.ColorId == 1);
            Assert.IsTrue(PartImages.LastUpdated > DateTime.MinValue);
        }

        private List<PartImages> GetPartImagesTestData()
        {
            List<PartImages> PartImages = new List<PartImages>
            {
                GetTestRow()
            };
            return PartImages;
        }

        private PartImages GetTestRow()
        {
            return new PartImages()
            {
                PartNum = "abc",
                SourceImageUrl = "def",
                ColorId = 1,
                LastUpdated = DateTime.Now
            };
        }

    }
}
