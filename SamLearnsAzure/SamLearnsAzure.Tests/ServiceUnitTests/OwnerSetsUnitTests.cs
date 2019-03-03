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
    public class OwnerSetsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetOwnerSetsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IOwnerSetsRepository> mock = new Mock<IOwnerSetsRepository>();
            mock.Setup(repo => repo.GetOwnerSets(It.IsAny<int>())).Returns(Task.FromResult(GetOwnerSetsTestData()));
            OwnerSetsController controller = new OwnerSetsController(mock.Object);
            int ownerId = 1;

            //Act
            IEnumerable<OwnerSets> sets = await controller.GetOwnerSets(ownerId);

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
