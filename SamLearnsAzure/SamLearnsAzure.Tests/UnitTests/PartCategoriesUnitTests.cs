using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.UnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class PartCategoriesUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartCategoriesMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IPartCategoriesRepository> mock = new Mock<IPartCategoriesRepository>();
            mock.Setup(repo => repo.GetPartCategories()).Returns(Task.FromResult(GetPartCategoriesTestData()));
            PartCategoriesController controller = new PartCategoriesController(mock.Object);

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
