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
    public class PartsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartsMockTest()
        {
            //Arrange
            Mock<IPartsRepository> mock = new Mock<IPartsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetParts(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetPartsTestData()));
            PartsController controller = new PartsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Parts> parts = await controller.GetParts();

            //Assert
            Assert.IsTrue(parts != null);
            Assert.IsTrue(parts.Count() == 1);
            TestParts(parts.FirstOrDefault());
        }

        private void TestParts(Parts Part)
        {
            Assert.IsTrue(Part.PartNum == "abc");
            Assert.IsTrue(Part.Name == "def");
            Assert.IsTrue(Part.PartCatId == 1);
            Assert.IsTrue(Part.PartMaterialId == 2);
            Assert.IsTrue(Part.InventoryParts != null);
        }

        private IEnumerable<Parts> GetPartsTestData()
        {
            List<Parts> Parts = new List<Parts>
            {
                GetPartTestData()
            };
            return Parts;
        }

        private Parts GetPartTestData()
        {
            return new Parts()
            {
                PartNum = "abc",
                Name = "def",
                PartCatId = 1,
                PartMaterialId = 2
                //PartCategory = new PartCategories()
            };
        }

    }
}
