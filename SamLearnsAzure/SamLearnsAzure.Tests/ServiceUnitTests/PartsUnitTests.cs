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
    public class PartsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
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
            //Assert.IsTrue(Part.PartCategory != null);
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
                PartCatId = 1//,
                //PartCategory = new PartCategories()
    };
        }

    }
}
