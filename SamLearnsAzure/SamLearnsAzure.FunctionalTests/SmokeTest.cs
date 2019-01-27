using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace SamLearnsAzure.FunctionalTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _samLearnsAzureServiceUrl = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureServiceTest()
        {
            //Arrange
            bool serviceLoaded = false;

            //Act
            string serviceURL = _samLearnsAzureServiceUrl + "api/values";
            _driver.Navigate().GoToUrl(serviceURL);
            serviceLoaded = (_driver.Url == serviceURL);
            //OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/pre");
            //System.Diagnostics.Debug.WriteLine(data.ToString());

            //Assert
            Assert.IsTrue(serviceLoaded);
            //Assert.IsTrue(data != null);
        }

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);
            
            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                _samLearnsAzureServiceUrl = "https://samsapp-dev-eu-web.azurewebsites.net/";
            }
            else
            {
                _samLearnsAzureServiceUrl = TestContext.Properties["SamLearnsAzureServiceUrl"].ToString();
            }
        }

        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        [TestCleanup()]
        public void CleanupTests()
        {
            _driver.Quit();
        }
    }
}
