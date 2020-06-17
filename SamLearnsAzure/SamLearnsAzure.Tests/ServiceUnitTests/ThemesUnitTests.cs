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
    public class ThemesServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetThemesMockTest()
        {
            //Arrange
            Mock<IThemesRepository> mock = new Mock<IThemesRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetThemes(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetThemesTestData()));
            ThemesController controller = new ThemesController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Themes> sets = await controller.GetThemes();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestThemes(sets.FirstOrDefault());
        }

        private void TestThemes(Themes Themes)
        {
            Assert.IsTrue(Themes.Id == 1);
            Assert.IsTrue(Themes.Name == "abc");
            Assert.IsTrue(Themes.ParentId == 2);
            //Assert.IsTrue(Themes.Sets != null);
        }

        private IEnumerable<Themes> GetThemesTestData()
        {
            List<Themes> Themes = new List<Themes>
            {
                GetTestRow()
            };
            return Themes;
        }

        private Themes GetTestRow()
        {
            return new Themes()
            {
                Id = 1,
                Name = "abc",
                ParentId = 2
            };
        }

    }
}
