//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using SamLearnsAzure.Service.Controllers;
//using SamLearnsAzure.Service.DataAccess;
//using SamLearnsAzure.Service.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SamLearnsAzure.Tests.UnitTests
//{
//    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
//    [TestClass]
//    [TestCategory("UnitTest")]
//    public class PartsSummaryUnitTests : BaseUnitTest
//    {
//        [TestMethod]
//        public async Task GetPartsSummaryMockTest()
//        {
//            //Arrange
//            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
//            Mock<IPartsRepository> mock = new Mock<IPartsRepository>();
//            mock.Setup(repo => repo.GetPartsSummary(It.IsAny<string>())).Returns(Task.FromResult(GetPartsTestData()));
//            PartsController controller = new PartsController(mock.Object);
//            string setNum = "75218-1";

//            //Act
//            IEnumerable<PartsSummary> parts = await controller.GetPartsSummary(setNum);

//            //Assert
//            Assert.IsTrue(parts != null);
//            Assert.IsTrue(parts.Count() == 1);
//            TestParts(parts.FirstOrDefault());
//        }

//        private void TestParts(PartsSummary Part)
//        {
//            Assert.IsTrue(Part.PartNum == "abc");
//            Assert.IsTrue(Part.PartName == "def");
//            Assert.IsTrue(Part.PartCategoryId == 1);
//            Assert.IsTrue(Part.PartCategoryName == "ghi");
//            Assert.IsTrue(Part.ColorId == 2);
//            Assert.IsTrue(Part.ColorName == "jkl");
//            Assert.IsTrue(Part.Quantity == 3);
//        }

//        private IEnumerable<PartsSummary> GetPartsTestData()
//        {
//            List<PartsSummary> Parts = new List<PartsSummary>
//            {
//                GetPartTestData()
//            };
//            return Parts;
//        }

//        private PartsSummary GetPartTestData()
//        {
//            return new PartsSummary()
//            {
//                PartNum = "abc",
//                PartName = "def",
//                PartCategoryId = 1,
//                PartCategoryName = "ghi",
//                ColorId = 2,
//                ColorName = "jkl",
//                Quantity = 3
//            };
//        }

//    }
//}
