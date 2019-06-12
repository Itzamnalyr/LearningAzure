using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Web.Controllers;
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
            //Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            //Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            //HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object);


            ////Act
            //IActionResult result = controller.Error();

            //Assert
            Assert.IsTrue(model.ShowRequestId == true);
            Assert.IsTrue(model.RequestId == "abc123");
        }

      
    }
}
