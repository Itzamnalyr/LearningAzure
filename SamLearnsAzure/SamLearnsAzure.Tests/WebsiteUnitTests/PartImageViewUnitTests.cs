using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Controllers;
using SamLearnsAzure.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.WebsiteUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class PartImageViewUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartImagesViewMockTest()
        {
            //Arrange
            string configValue = "xyz321";
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IFeatureFlagsServiceApiClient> mockFFService = new Mock<IFeatureFlagsServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
             mockService.Setup(repo => repo.GetPartImages()).Returns(Task.FromResult(GetPartImagesTestData()));
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(configValue);
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object, mockFFService.Object);

            //Act
            IActionResult result = await controller.PartImages();

            //Assert
            ViewResult viewResult = (ViewResult)result;
            PartImagesViewModel updateImageViewModel = (PartImagesViewModel)viewResult.Model;
            Assert.IsTrue(updateImageViewModel != null);
            Assert.IsTrue(updateImageViewModel.BasePartsImagesStorageURL != null);
            Assert.IsTrue(updateImageViewModel.PartImages != null);
            Assert.IsTrue(updateImageViewModel.PartImages.Any());
            TestPartImage(updateImageViewModel.PartImages[0]);
        }

        private void TestPartImage(PartImages partImageset)
        {
            Assert.IsTrue(partImageset.PartImageId == 1);
            Assert.IsTrue(partImageset.PartNum == "abc");
            Assert.IsTrue(partImageset.SourceImage == "def");
            Assert.IsTrue(partImageset.ColorId == 1);
            Assert.IsTrue(partImageset.Color != null);
            Assert.IsTrue(partImageset.Color.Id == 1);
            Assert.IsTrue(partImageset.Color.Name == "ghi");
            Assert.IsTrue(partImageset.LastUpdated > DateTime.MinValue);
        }

        private PartImages GetSetTestData()
        {
            Colors color = new Colors
            {
                Id = 1,
                Name = "ghi"
            };

            return new PartImages()
            {
                PartImageId = 1,
                PartNum = "abc",
                SourceImage = "def",
                ColorId = 1,
                Color = color,
                LastUpdated = DateTime.Now
            };
        }

        private void TestSetImages(PartImages partImage)
        {
            Assert.IsTrue(partImage.PartImageId == 1);
            Assert.IsTrue(partImage.PartNum == "abc");
            Assert.IsTrue(partImage.SourceImage == "def");
        }

        private List<PartImages> GetPartImagesTestData()
        {
            Colors color = new Colors
            {
                Id = 1,
                Name = "ghi"
            };

            List<PartImages> items = new List<PartImages>
            {
                new PartImages()
            {
                PartImageId = 1,
                PartNum = "abc",
                SourceImage = "def",
                ColorId = 1,
                Color = color,
                LastUpdated = DateTime.Now
            }
            };
            return items;
        }

    }
}
