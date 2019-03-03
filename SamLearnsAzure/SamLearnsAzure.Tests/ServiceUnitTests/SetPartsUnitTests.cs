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
    public class SetPartsUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetSetPartsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<ISetsRepository> mock = new Mock<ISetsRepository>();
            mock.Setup(repo => repo.GetSetParts(It.IsAny<string>())).Returns(Task.FromResult(GetSetPartsTestData()));
            SetsController controller = new SetsController(mock.Object);
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
