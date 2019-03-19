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
    public class InventorySetsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetInventorySetsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IInventorySetsRepository> mock = new Mock<IInventorySetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetInventorySets(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetInventorySetsTestData()));
            InventorySetsController controller = new InventorySetsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<InventorySets> sets = await controller.GetInventorySets();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestInventorySets(sets.FirstOrDefault());
        }

        private void TestInventorySets(InventorySets InventorySet)
        {
            Assert.IsTrue(InventorySet.InventoryId == 1);
            Assert.IsTrue(InventorySet.SetNum == "abc");
            Assert.IsTrue(InventorySet.Quantity == 2);
            Assert.IsTrue(InventorySet.InventorySetId == 3);
            Assert.IsTrue(InventorySet.Inventory != null);
            Assert.IsTrue(InventorySet.Set != null);
        }

        private IEnumerable<InventorySets> GetInventorySetsTestData()
        {
            List<InventorySets> InventorySets = new List<InventorySets>
            {
                GetTestRow()
            };
            return InventorySets;
        }

        private InventorySets GetTestRow()
        {
            return new InventorySets()
            {
                InventoryId = 1,
                SetNum = "abc",
                Quantity = 2,
                InventorySetId = 3,
                Inventory = new Inventories(),
                Set = new Sets()
            };
        }

    }
}
