using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SetsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetSetsMockTest()
        {
            //Arrange
            Mock<ISetsRepository> mock = new Mock<ISetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetSets()).Returns(Task.FromResult(GetSetsTestData()));
            SetsController controller = new SetsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Sets> sets = await controller.GetSets();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestSets(sets.FirstOrDefault());
        }

        [TestMethod]
        public async Task GetSetMockTest()
        {
            //Arrange
            Mock<ISetsRepository> mock = new Mock<ISetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetSet(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
            SetsController controller = new SetsController(mock.Object, mockRedis.Object);
            string setNum = "75218-1";

            //Act
            Sets set = await controller.GetSet(setNum);

            //Assert
            Assert.IsTrue(set != null);
            TestSets(set ?? new Sets());
        }

        private void TestSets(Sets set)
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

        private IEnumerable<Sets> GetSetsTestData()
        {
            List<Sets> Sets = new List<Sets>
            {
                GetSetTestData()
            };
            return Sets;
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
