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
    public class BrowseSetsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetBrowseSetsMockTest()
        {
            //Arrange
            Mock<IBrowseSetsRepository> mock = new Mock<IBrowseSetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetBrowseSets(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<int?>())).Returns(Task.FromResult(GetBrowseSetsTestData()));
            BrowseSetsController controller = new BrowseSetsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<BrowseSets> results = await controller.GetBrowseSets();

            //Assert
            Assert.IsTrue(results.Count() == 1);
            TestBrowseSets(results.FirstOrDefault());
        }

        private void TestBrowseSets(BrowseSets set)
        {
            Assert.IsTrue(set.SetNum == "abc");
            Assert.IsTrue(set.Name == "def");
            Assert.IsTrue(set.NumParts == 1);
            Assert.IsTrue(set.ThemeId == 2);
            Assert.IsTrue(set.ThemeName == "ghi");
            Assert.IsTrue(set.Year == 3);
        }

        private IEnumerable<BrowseSets> GetBrowseSetsTestData()
        {
            List<BrowseSets> browseSets = new List<BrowseSets>
            {
                GetSetTestData()
            };
            return browseSets;
        }

        private BrowseSets GetSetTestData()
        {
            Themes Theme = new Themes
            {
                Id = 2,
                Name = "ghi",
                ParentId = null
            };

            return new BrowseSets()
            {
                SetNum = "abc",
                Name = "def",
                NumParts = 1,
                ThemeId = 2,
                ThemeName = "ghi",
                Year = 3
            };
        }

    }
}
