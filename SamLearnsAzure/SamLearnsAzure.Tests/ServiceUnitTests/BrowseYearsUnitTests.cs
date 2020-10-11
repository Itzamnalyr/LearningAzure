using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class BrowseYearsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetBrowseYearsMockTest()
        {
            //Arrange
            Mock<IBrowseYearsRepository> mock = new Mock<IBrowseYearsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetBrowseYears(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<int?>())).Returns(Task.FromResult(GetBrowseYearsTestData()));
            BrowseYearsController controller = new BrowseYearsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<BrowseYears> results = await controller.GetBrowseYears();

            //Assert
            Assert.IsTrue(results.Count() == 1);
            TestBrowseYears(results.FirstOrDefault());
        }

        private void TestBrowseYears(BrowseYears BrowseYears)
        {
            Assert.IsTrue(BrowseYears.Year == 1);
            Assert.IsTrue(BrowseYears.YearName == "abc");
        }

        private IEnumerable<BrowseYears> GetBrowseYearsTestData()
        {
            List<BrowseYears> BrowseYears = new List<BrowseYears>
            {
                GetTestRow()
            };
            return BrowseYears;
        }

        private BrowseYears GetTestRow()
        {
            return new BrowseYears()
            {
                Year = 1,
                YearName = "abc"
            };
        }

    }
}
