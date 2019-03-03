using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class KeyVaultUnitTest
    {
        [TestMethod]
        public void GetApplicationInsightsInstrumentationKeyUnitTest()
        {
            //Arrange
            Mock<IConfigurationRoot> configuration = new Mock<IConfigurationRoot>();
            configuration.SetupGet(x => x[It.IsAny<string>()]).Returns("AppInsightsInsKeySecret");
            KeyVaultTestController controller = new KeyVaultTestController(configuration.Object);

            //Act
            string item = controller.GetApplicationInsightsInstrumentationKey();

            //Assert
            Assert.IsTrue(item != null);
            Assert.IsTrue(item == "AppInsightsInsKeySecret");
        }


        [TestMethod]
        public void GetStorageKeyUnitTest()
        {
            //Arrange
            Mock<IConfigurationRoot> configuration = new Mock<IConfigurationRoot>();
            configuration.SetupGet(x => x[It.IsAny<string>()]).Returns("SecretStorageKey");
            KeyVaultTestController controller = new KeyVaultTestController(configuration.Object);

            //Act
            string item = controller.GetStorageKey();

            //Assert
            Assert.IsTrue(item != null);
            Assert.IsTrue(item == "SecretStorageKey");
        }
    }
}
