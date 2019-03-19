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
    public class OwnersUnitTest : BaseUnitTest
    {
        [TestMethod]
        public async Task GetOwnersUnitTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IOwnersRepository> mock = new Mock<IOwnersRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetOwners(It.IsAny<IRedisService>(), It.IsAny<bool>())).Returns(Task.FromResult(GetOwnersTestData()));
            OwnersController controller = new OwnersController(mock.Object, mockRedis.Object);

            //Act
            IEnumerable<Owners> items = await controller.GetOwners();

            //Assert
            Assert.IsTrue(items != null);
            TestOwners(items.FirstOrDefault<Owners>());
        }


        [TestMethod]
        public async Task GetOwnerUnitTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IOwnersRepository> mock = new Mock<IOwnersRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetOwner(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<int>())).Returns(Task.FromResult(GetOwnersRow()));
            OwnersController controller = new OwnersController(mock.Object, mockRedis.Object);
            int id = 99;

            //Act
            Owners item = await controller.GetOwner(id);

            //Assert
            Assert.IsTrue(item != null);
            TestOwners(item);
        }


        private void TestOwners(Owners owner)
        {
            Assert.IsTrue(owner.Id == 99);
            Assert.IsTrue(owner.OwnerName == "abc123");
            Assert.IsTrue(owner.OwnerSets != null);
        }

        private IEnumerable<Owners> GetOwnersTestData()
        {
            List<Owners> owners = new List<Owners>
            {
                GetOwnersRow()
            };
            return owners;
        }

        private Owners GetOwnersRow()
        {
            return new Owners()
            {
                Id = 99,
                OwnerName = "abc123"

            };
        }
    }
}
