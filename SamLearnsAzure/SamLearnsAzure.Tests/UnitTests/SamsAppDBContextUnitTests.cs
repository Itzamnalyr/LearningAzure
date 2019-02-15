using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Service.Models;

namespace SamLearnsAzure.Tests.UnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class SamsAppDBContextUnitTests : BaseUnitTest
    {
        [TestMethod]
        public void InitializeSamsAppDBContextMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext();

            //Act

            //Assert
            Assert.IsTrue(context != null);
        }
        
    }
}
