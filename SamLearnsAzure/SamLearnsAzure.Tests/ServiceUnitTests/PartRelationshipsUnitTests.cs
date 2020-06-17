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
    public class PartRelationshipsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartRelationshipsMockTest()
        {
            //Arrange
            Mock<IPartRelationshipsRepository> mock = new Mock<IPartRelationshipsRepository>();
            mock.Setup(repo => repo.GetPartRelationships()).Returns(Task.FromResult(GetPartRelationshipsTestData()));
            PartRelationshipsController controller = new PartRelationshipsController(mock.Object);

            //Act
            IEnumerable<PartRelationships> sets = await controller.GetPartRelationships();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestPartRelationships(sets.FirstOrDefault());
        }

        private void TestPartRelationships(PartRelationships PartRelationships)
        {
            Assert.IsTrue(PartRelationships.RelType == "abc");
            Assert.IsTrue(PartRelationships.ChildPartNum == "def");
            Assert.IsTrue(PartRelationships.ParentPartNum == "ghi");
            Assert.IsTrue(PartRelationships.PartRelationshipId == 1);
        }

        private IEnumerable<PartRelationships> GetPartRelationshipsTestData()
        {
            List<PartRelationships> PartRelationships = new List<PartRelationships>
            {
                GetTestRow()
            };
            return PartRelationships;
        }

        private PartRelationships GetTestRow()
        {
            return new PartRelationships()
            {
                RelType = "abc",
                ChildPartNum = "def",
                ParentPartNum = "ghi",
                PartRelationshipId = 1
            };
        }

    }
}
