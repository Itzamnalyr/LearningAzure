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
    public class ColorsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetColorsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IColorsRepository> mock = new Mock<IColorsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetColors(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetColorsTestData()));
            ColorsController controller = new ColorsController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Colors> sets = await controller.GetColors();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestColors(sets.FirstOrDefault());
        }

        private void TestColors(Colors Colors)
        {
            Assert.IsTrue(Colors.Id == 1);
            Assert.IsTrue(Colors.Name == "abc");
            Assert.IsTrue(Colors.Rgb == "def");
            Assert.IsTrue(Colors.IsTrans == false);
            Assert.IsTrue(Colors.InventoryParts != null);
        }

        private IEnumerable<Colors> GetColorsTestData()
        {
            List<Colors> Colors = new List<Colors>
            {
                GetTestRow()
            };
            return Colors;
        }

        private Colors GetTestRow()
        {
            return new Colors()
            {
                Id = 1,
                Name = "abc",
                Rgb = "def",
                IsTrans = false
            };
        }

    }
}
