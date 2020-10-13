using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class BrowseThemesServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetBrowseThemesMockTest()
        {
            //Arrange
            Mock<IBrowseThemesRepository> mock = new Mock<IBrowseThemesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetBrowseThemes(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<int?>())).Returns(Task.FromResult(GetBrowseThemesTestData()));
            BrowseThemesController controller = new BrowseThemesController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<BrowseThemes> results = await controller.GetBrowseThemes();

            //Assert
            //Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count() == 1);
            TestBrowseThemes(results.FirstOrDefault());
        }

        private void TestBrowseThemes(BrowseThemes BrowseThemes)
        {
            Assert.IsTrue(BrowseThemes.Id == 1);
            Assert.IsTrue(BrowseThemes.Name == "abc");
            Assert.IsTrue(BrowseThemes.TopParentId == 3);
            Assert.IsTrue(BrowseThemes.ThemeName == "def");
        }

        private IEnumerable<BrowseThemes> GetBrowseThemesTestData()
        {
            List<BrowseThemes> BrowseThemes = new List<BrowseThemes>
            {
                GetTestRow()
            };
            return BrowseThemes;
        }

        private BrowseThemes GetTestRow()
        {
            return new BrowseThemes()
            {
                Id = 1,
                Name = "abc",
                TopParentId = 3,
                ThemeName = "def"
            };
        }

    }
}
