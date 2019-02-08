using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Service.Controllers;

namespace SamLearnsAzure.Tests.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class KeyVaultIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public void GetStorageKeyIntegrationTest()
        {
            //Arrange
            KeyVaultTestController controller = new KeyVaultTestController(base.Configuration);
            string storageKeyPartial = "jMhmcvnO";
            string ClientSecret = "AgQsPi7JYwdPysgTwzzMcI3C+RLzZxjIiWFoVLieoaY=";

            //Act
            string storageKey = controller.GetStorageKey();

            //Assert
            Assert.IsTrue(storageKey.Contains(storageKeyPartial) == true);
            Assert.IsTrue(ClientSecret == "AgQsPi7JYwdPysgTwzzMcI3C+RLzZxjIiWFoVLieoaY=");
        }
    }
}
