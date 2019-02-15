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
    public class OwnerSetsUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetOwnerSetsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IOwnerSetsRepository> mock = new Mock<IOwnerSetsRepository>();
            mock.Setup(repo => repo.GetOwnerSets()).Returns(Task.FromResult(GetOwnerSetsTestData()));
            OwnerSetsController controller = new OwnerSetsController(mock.Object);

            //Act
            IEnumerable<OwnerSets> sets = await controller.GetOwnerSets();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestOwnerSets(sets.FirstOrDefault());
        }

        private void TestOwnerSets(OwnerSets OwnerSet)
        {
            Assert.IsTrue(OwnerSet.SetNum == "abc");
            Assert.IsTrue(OwnerSet.OwnerId == 1);
            Assert.IsTrue(OwnerSet.Owned == false);
            Assert.IsTrue(OwnerSet.Wanted == true);
            Assert.IsTrue(OwnerSet.OwnerSetId == 2);
            Assert.IsTrue(OwnerSet.Owner != null);
            Assert.IsTrue(OwnerSet.Set != null);
        }

        private IEnumerable<OwnerSets> GetOwnerSetsTestData()
        {
            List<OwnerSets> OwnerSets = new List<OwnerSets>
            {
                GetTestRow()
            };
            return OwnerSets;
        }

        private OwnerSets GetTestRow()
        {
            return new OwnerSets()
            {
                SetNum = "abc",
                OwnerId = 1,
                Owned = false,
                Wanted = true,
                OwnerSetId = 2,
                Owner = new Owners(),
                Set = new Sets()
                
    };
        }

    }
}
