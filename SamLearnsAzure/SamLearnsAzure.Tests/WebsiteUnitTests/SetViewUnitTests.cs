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
            Mock<IServiceAPIClient> mockService = new Mock<IServiceAPIClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            mockService.Setup(repo => repo.GetSet(It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
            mockService.Setup(repo => repo.GetSetParts(It.IsAny<string>())).Returns(Task.FromResult(GetSetPartsTestData()));
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(configValue);
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object);

            //Act
            IActionResult result = await controller.Set(setNum);

            //Assert
            ViewResult viewResult = (ViewResult)result;
            SetViewModel setViewModel = (SetViewModel)viewResult.Model;
            Assert.IsTrue(setViewModel != null);
            Assert.IsTrue(setViewModel.Set != null);
            TestSet(setViewModel.Set);
            Assert.IsTrue(setViewModel.SetParts.Count() == 1);
            TestSetParts(setViewModel.SetParts.FirstOrDefault());
            Assert.IsTrue(setViewModel.BaseSetPartsImagesStorageURL == configValue);
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

        private void TestSet(Sets Set)
        {
            Assert.IsTrue(Set.SetNum == "abc");
            Assert.IsTrue(Set.Name == "def");
            Assert.IsTrue(Set.NumParts == 1);
            Assert.IsTrue(Set.ThemeId == 2);
            Assert.IsTrue(Set.Year == 3);
            Assert.IsTrue(Set.Theme != null);
            Assert.IsTrue(Set.Inventories != null);
            Assert.IsTrue(Set.InventorySets != null);
            Assert.IsTrue(Set.OwnerSets != null);
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

    }
}
