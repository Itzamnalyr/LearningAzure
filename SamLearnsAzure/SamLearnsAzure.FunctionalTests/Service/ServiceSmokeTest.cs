using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using SamLearnsAzure.Models;
using System;

namespace SamLearnsAzure.FunctionalTests.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ServiceSmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _serviceUrl = null;
        private string _service2Url = null;
        //private string _environment = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureServiceValuesTest()
        {
            //Arrange
            bool serviceLoaded;

            //Act
            string serviceURL = _serviceUrl + "api/values";
            Console.WriteLine(serviceURL);
            _driver.Navigate().GoToUrl(serviceURL);
            serviceLoaded = (_driver.Url == serviceURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/pre");


            //Assert
            Assert.IsTrue(serviceLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "[\"value1\",\"value2\"]");
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureService2ValuesTest()
        {
            //Arrange
            bool serviceLoaded;

            //Act
            string serviceURL = _service2Url + "api/values";
            Console.WriteLine(serviceURL);
            _driver.Navigate().GoToUrl(serviceURL);
            serviceLoaded = (_driver.Url == serviceURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/pre");


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
            bool serviceLoaded;

            //Act
            string serviceURL = _serviceUrl + "api/owners/getowners";
            Console.WriteLine(serviceURL);
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

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            //_driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);
            _driver = new ChromeDriver(chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                _serviceUrl = "https://samsapp-dev-eu-service.azurewebsites.net/";
                _service2Url = "https://samsapp-dev2-eu-service.azurewebsites.net/";
                //_environment = "dev";
            }
            else
            {
                _serviceUrl = TestContext.Properties["ServiceUrl"].ToString();
                _service2Url = TestContext.Properties["Service2Url"].ToString();
                // _environment = TestContext.Properties["TestEnvironment"].ToString();
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
