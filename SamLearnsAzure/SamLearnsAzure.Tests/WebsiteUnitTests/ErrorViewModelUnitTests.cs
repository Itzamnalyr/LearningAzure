using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Web.Models;

namespace SamLearnsAzure.Tests.WebsiteUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class ErrorViewModelUnitTests 
    {
        [TestMethod]
        public void GetErrorViewModelTest()
        {
            //Arrange
            ErrorViewModel model = new ErrorViewModel
            {
                RequestId = "abc123"
            };

            //Act

            //Assert
            Assert.IsTrue(model.ShowRequestId == true);
            Assert.IsTrue(model.RequestId == "abc123");
        }

      
    }
}
