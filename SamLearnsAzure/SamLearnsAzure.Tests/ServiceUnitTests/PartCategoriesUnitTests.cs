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
    public class PartCategoriesServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartCategoriesMockTest()
        {
            //Arrange
            Mock<IPartCategoriesRepository> mock = new Mock<IPartCategoriesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetPartCategories(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetPartCategoriesTestData()));
            PartCategoriesController controller = new PartCategoriesController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<PartCategories> sets = await controller.GetPartCategories();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestPartCategories(sets.FirstOrDefault());
        }

        private void TestPartCategories(PartCategories PartCategories)
        {
            Assert.IsTrue(PartCategories.Id == 1);
            Assert.IsTrue(PartCategories.Name == "abc");
            //Assert.IsTrue(PartCategories.Parts != null);
        }

        private IEnumerable<PartCategories> GetPartCategoriesTestData()
        {
            List<PartCategories> PartCategories = new List<PartCategories>
            {
                GetTestRow()
            };
            return PartCategories;
        }

        private PartCategories GetTestRow()
        {
            return new PartCategories()
            {
                Id = 1,
                Name = "abc"
            };
        }

    }
}
