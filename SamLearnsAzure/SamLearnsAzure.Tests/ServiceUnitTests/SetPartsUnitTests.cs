using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
    public class SetPartsUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetSetPartsMockTest()
        {
            //Arrange
            Mock<ISetPartsRepository> mockSetParts = new Mock<ISetPartsRepository>();
            Mock<IPartImagesRepository> mockImageParts = new Mock<IPartImagesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockSetParts.Setup(repo => repo.GetSetParts(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(GetSetPartsTestData()));
            SetPartsController controller = new SetPartsController(mockSetParts.Object, mockImageParts.Object, mockRedis.Object, mockConfig.Object);
            string setNum = "abc123";

            //Act
            IEnumerable<SetParts> setParts = await controller.GetSetParts(setNum);

            //Assert
            Assert.IsTrue(setParts != null);
            Assert.IsTrue(setParts.Count() == 1);
            TestSetParts(setParts.FirstOrDefault());
        }

        private void TestSetParts(SetParts setParts)
        {
            Assert.IsTrue(setParts.PartNum == "abc");
            Assert.IsTrue(setParts.PartName == "def");
            Assert.IsTrue(setParts.ColorId == 1);
            Assert.IsTrue(setParts.ColorName == "ghi");
            Assert.IsTrue(setParts.PartCategoryId == 2);
            Assert.IsTrue(setParts.PartCategoryName == "jkl");
            Assert.IsTrue(setParts.Quantity == 3);
        }

        private IEnumerable<SetParts> GetSetPartsTestData()
        {
            List<SetParts> setParts = new List<SetParts>
            {
                GetSetPartTestData()
            };
            return setParts;
        }

        private SetParts GetSetPartTestData()
        {
            return new SetParts()
            {
                PartNum = "abc",
                PartName = "def",
                ColorId = 1,
                ColorName = "ghi",
                PartCategoryId = 2,
                PartCategoryName = "jkl",
                Quantity = 3
            };
        }

    }
}
