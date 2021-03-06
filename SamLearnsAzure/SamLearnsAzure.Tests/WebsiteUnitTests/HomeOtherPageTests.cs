using System.Threading.Tasks;
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
    public class HomeOtherPageTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetAboutViewTest()
        {
            //Arrange
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IFeatureFlagsServiceApiClient> mockFFService = new Mock<IFeatureFlagsServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("dev");

            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object, mockFFService.Object);

            //Act
            IActionResult result = await controller.About();

            //Assert
            ViewResult viewResult = result as ViewResult ?? new ViewResult();
        }

        [TestMethod]
        public void GetPrivacyViewTest()
        {
            //Arrange
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IFeatureFlagsServiceApiClient> mockFFService = new Mock<IFeatureFlagsServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object, mockFFService.Object);

            //Act
            IActionResult result = controller.Privacy();

            //Assert
            ViewResult viewResult = result as ViewResult ?? new ViewResult();
            Assert.IsTrue(viewResult.ViewData["Message"] != null);
        }

        //[TestMethod]
        //public void GetErrorViewTest()
        //{
        //    //Arrange
        //    Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
        //    Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
        //    System.Diagnostics.Activity activity = new System.Diagnostics.Activity("mockoperation");
        //    HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object);

        //    //Act
        //    IActionResult result = controller.Error();

        //    //Assert
        //    ViewResult viewResult = result as ViewResult ?? new ViewResult();
        //    ErrorViewModel errorViewModel = (ErrorViewModel)viewResult.Model;
        //    Assert.IsTrue(errorViewModel != null);
        //    Assert.IsTrue(errorViewModel.RequestId == null);
        //    Assert.IsTrue(errorViewModel.ShowRequestId == false);

        //}


    }
}
