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
    public class InventoriesServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetInventoriesMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IInventoriesRepository> mock = new Mock<IInventoriesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetInventories(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetInventoriesTestData()));
            InventoriesController controller = new InventoriesController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Inventories> sets = await controller.GetInventories();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestInventories(sets.FirstOrDefault());
        }

        private void TestInventories(Inventories inventory)
        {
            Assert.IsTrue(inventory.Id == 1);
            Assert.IsTrue(inventory.Version == 2);
            Assert.IsTrue(inventory.SetNum == "abc");
            Assert.IsTrue(inventory.Set != null);
            Assert.IsTrue(inventory.InventoryParts != null);
            Assert.IsTrue(inventory.InventorySets != null);
        }

        private IEnumerable<Inventories> GetInventoriesTestData()
        {
            List<Inventories> Inventories = new List<Inventories>
            {
                GetTestRow()
            };
            return Inventories;
        }

        private Inventories GetTestRow()
        {
            return new Inventories()
            {
                Id = 1,
                Version = 2,
                SetNum = "abc",
                Set = new Sets()
            };
        }

    }
}
