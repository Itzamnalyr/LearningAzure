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
    public class IndexViewUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetIndexViewMockTest()
        {
            //Arrange
            string environmentName = "DevResult";
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IFeatureFlagsServiceApiClient> mockFFService = new Mock<IFeatureFlagsServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            mockService.Setup(repo => repo.GetOwnerSets(It.IsAny<int>())).Returns(Task.FromResult(GetOwnerSetsTestData()));
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(environmentName);
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object, mockFFService.Object);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            ViewResult viewResult = (ViewResult)result;
            IndexViewModel indexViewModel = (IndexViewModel)viewResult.Model;
            Assert.IsTrue(indexViewModel != null);
            Assert.IsTrue(indexViewModel?.Environment as string == environmentName);
            Assert.IsTrue(indexViewModel?.OwnerSets.Count() == 1);
            TestOwnerSets(indexViewModel?.OwnerSets.FirstOrDefault() ?? new OwnerSets());
        }

        private void TestOwnerSets(OwnerSets OwnerSet)
        {
            Assert.IsTrue(OwnerSet.SetNum == "abc");
            Assert.IsTrue(OwnerSet.OwnerId == 1);
            Assert.IsTrue(OwnerSet.Owned == false);
            Assert.IsTrue(OwnerSet.Wanted == true);
            Assert.IsTrue(OwnerSet.OwnerSetId == 2);
            Assert.IsTrue(OwnerSet.Owner != null);
            Assert.IsTrue(OwnerSet.Set != null);
        }

        private List<OwnerSets> GetOwnerSetsTestData()
        {
            List<OwnerSets> OwnerSets = new List<OwnerSets>
            {
                GetTestRow()
            };
            return OwnerSets;
        }

        private OwnerSets GetTestRow()
        {
            return new OwnerSets()
            {
                SetNum = "abc",
                OwnerId = 1,
                Owned = false,
                Wanted = true,
                OwnerSetId = 2,
                Owner = new Owners(),
                Set = new Sets()

            };
        }

    }
}
