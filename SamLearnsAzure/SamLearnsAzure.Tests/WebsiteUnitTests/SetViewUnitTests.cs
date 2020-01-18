using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Controllers;
using SamLearnsAzure.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.WebsiteUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class SetViewUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetSetViewMockTest()
        {
            //Arrange
            string setNum = "abc123";
            string configValue = "xyz321";
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IFeatureFlagsServiceApiClient> mockFFService = new Mock<IFeatureFlagsServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            mockService.Setup(repo => repo.GetSet(It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
            mockService.Setup(repo => repo.GetSetImage(It.IsAny<string>())).Returns(Task.FromResult(GetSetImageTestData()));
            mockService.Setup(repo => repo.GetSetParts(It.IsAny<string>())).Returns(Task.FromResult(GetSetPartsTestData()));
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(configValue);
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object, mockFFService.Object);

            //Act
            IActionResult result = await controller.Set(setNum);

            //Assert
            ViewResult viewResult = (ViewResult)result;
            SetViewModel setViewModel = (SetViewModel)viewResult.Model;
            Assert.IsTrue(setViewModel != null);
            Assert.IsTrue(setViewModel?.Set != null);
            TestSet(setViewModel?.Set ?? new Sets());
            Assert.IsTrue(setViewModel?.SetParts.Count() == 1);
            TestSetParts(setViewModel?.SetParts.FirstOrDefault() ?? new SetParts());
            Assert.IsTrue(setViewModel?.BaseSetPartsImagesStorageURL == (configValue + configValue));
            Assert.IsTrue(setViewModel?.BaseSetImagesStorageURL == (configValue + configValue));
            TestSetImage(setViewModel?.SetImage ?? new SetImages());
        }

        private void TestSetParts(SetParts setPart)
        {
            Assert.IsTrue(setPart.PartNum == "abc");
            Assert.IsTrue(setPart.PartName == "def");
            Assert.IsTrue(setPart.ColorId == 1);
            Assert.IsTrue(setPart.ColorName == "ghi");
            Assert.IsTrue(setPart.PartCategoryId == 2);
            Assert.IsTrue(setPart.PartCategoryName == "jkl");
            Assert.IsTrue(setPart.Quantity == 3);
        }

        private List<SetParts> GetSetPartsTestData()
        {
            List<SetParts> SetParts = new List<SetParts>
            {
                GetTestRow()
            };
            return SetParts;
        }

        private SetParts GetTestRow()
        {
            return new SetParts()
            {
                PartNum = "abc",
                PartName = "def",
                ColorId = 1,
                ColorName = "ghi",
                PartCategoryId = 2,
                PartCategoryName = "jkl",
                Quantity = 3
            };
        }

        private void TestSet(Sets set)
        {
            Assert.IsTrue(set.SetNum == "abc");
            Assert.IsTrue(set.Name == "def");
            Assert.IsTrue(set.NumParts == 1);
            Assert.IsTrue(set.ThemeId == 2);
            Assert.IsTrue(set.Year == 3);
            Assert.IsTrue(set.Theme != null);
            Assert.IsTrue(set.Inventories != null);
            Assert.IsTrue(set.InventorySets != null);
            Assert.IsTrue(set.OwnerSets != null);
        }

        private Sets GetSetTestData()
        {
            Themes Theme = new Themes
            {
                Id = 2,
                Name = "ghi",
                ParentId = null
            };

            return new Sets()
            {
                SetNum = "abc",
                Name = "def",
                NumParts = 1,
                ThemeId = 2,
                Year = 3,
                Theme = Theme
            };
        }

        private void TestSetImage(SetImages setImage)
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
