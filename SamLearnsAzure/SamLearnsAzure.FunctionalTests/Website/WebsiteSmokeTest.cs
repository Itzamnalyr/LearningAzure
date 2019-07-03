using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.FunctionalTests.Website
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _webUrl = null;
        private string _environment = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureWebHomeIndexPageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "home";
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
       
            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "Environment: " + _environment);
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureWebHomeSetPageTest()
        {
            //Arrange
            bool webLoaded;
            string setNum = "75218-1";

            //Act
            string webURL = _webUrl + "home/set?setnum=" + setNum;
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/div[1]/span/strong");
           
            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.IsTrue(data.Text != null);
        }

        //[TestMethod]
        //[TestCategory("SkipWhenLiveUnitTesting")]
        //[TestCategory("SmokeTest")]
        //public void GotoSamLearnsAzureWebHomeSetImagesUpdatePageTest()
        //{
        //    //Arrange
        //    bool webLoaded;
        //    string setNum = "75218-1";

        //    //Act
        //    string webURL = _webUrl + "home/updateimage?setnum=" + setNum;
        //    _driver.Navigate().GoToUrl(webURL);
        //    webLoaded = (_driver.Url == webURL);
        //    OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/div[1]/span/strong");
        //    OpenQA.Selenium.IWebElement imageData = _driver.FindElementByXPath(@"/html/body/div/div[3]/div[1]/a/img");
            
        //    //Assert
        //    Assert.IsTrue(webLoaded);
        //    Assert.IsTrue(data != null);
        //    Assert.IsTrue(data.Text != null);
        //    Assert.IsTrue(imageData != null);
        //    Assert.IsTrue(imageData.Text != null);
        //    System.Diagnostics.Debug.WriteLine(imageData.GetAttribute("src"));
        //    Assert.IsTrue(imageData.GetAttribute("src") != null); //Make sure the element was assigned an image, and hence the bing search is working
        //}

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                _webUrl = "https://samsapp-dev-eu-web.azurewebsites.net/";
                _environment = "dev";
            }
            else
            {
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
