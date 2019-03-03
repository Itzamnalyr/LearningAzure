using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace SamLearnsAzure.FunctionalTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _serviceUrl = null;
        private string _webUrl = null;
        private string _environment = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureServiceValuesTest()
        {
            //Arrange
            bool serviceLoaded = false;

            //Act
            string serviceURL = _serviceUrl + "api/values";
            _driver.Navigate().GoToUrl(serviceURL);
            serviceLoaded = (_driver.Url == serviceURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/pre");
            //System.Diagnostics.Debug.WriteLine(data.ToString());

            //Assert
            Assert.IsTrue(serviceLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "[\"value1\",\"value2\"]");
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureServiceOwnersTest()
        {
            //Arrange
            bool serviceLoaded = false;

            //Act
            string serviceURL = _serviceUrl + "api/owners/getowners";
            _driver.Navigate().GoToUrl(serviceURL);
            serviceLoaded = (_driver.Url == serviceURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/pre");

            //Assert
            Assert.IsTrue(serviceLoaded);
            Assert.IsTrue(data != null);
            //Convert the JSON to the owners model
            IEnumerable<Owners> owners = JsonConvert.DeserializeObject<IEnumerable<Owners>>(data.Text);
            Assert.IsTrue(owners.Count() > 0); //There is more than one owner
            Assert.IsTrue(owners.FirstOrDefault().Id > 0); //The first owner has an id
            Assert.IsTrue(owners.FirstOrDefault().OwnerName.Length > 0); //The first owner has an name
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureWebHomePageTest()
        {
            //Arrange
            bool webLoaded = false;

            //Act
            string webURL = _webUrl + "home";
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/h2");
            //System.Diagnostics.Debug.WriteLine(data.ToString());

            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "Environment: " + _environment);
        }

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                _serviceUrl = "https://samsapp-dev-eu-service.azurewebsites.net/";
                _webUrl = "https://samsapp-dev-eu-web.azurewebsites.net/";
                _environment = "dev";
            }
            else
            {
                _serviceUrl = TestContext.Properties["ServiceUrl"].ToString();
                _webUrl = TestContext.Properties["WebUrl"].ToString();
                _environment = TestContext.Properties["TestEnvironment"].ToString();
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
