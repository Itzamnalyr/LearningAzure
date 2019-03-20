using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.EFCore;

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
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
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
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<ISetsRepository> mock = new Mock<ISetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetSet(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
            SetsController controller = new SetsController(mock.Object, mockRedis.Object);
            string setNum = "75218-1";

            //Act
            Sets set = await controller.GetSet(setNum);

            //Assert
            Assert.IsTrue(set != null);
            TestSets(set);
        }

        private void TestSets(Sets Set)
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
