using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using SamLearnsAzure.Models;
using OpenQA.Selenium;
using System.Threading;
using System.Collections;
using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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
        private string _keyVaultURL = null;
        private string _keyVaultClientId = null;
        private string _keyVaultClientSecret = null;
        private string _email = null;
        private string _password = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoSamLearnsAzureWebHomeIndexPageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "home";
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
            Console.WriteLine("data:" + data.Text);

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
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/div[1]/span/strong");
            Console.WriteLine("data:" + data.Text);

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
        //    bool webLoaded;
        //    string setNum = "75218-1";

        //    //Act - login: check that if the login link is showing, the page needs to log in
        //    string homePageLoginText = CheckForLogin();

        //    bool successfullyLoggedin = false;
        //    if (homePageLoginText == "Login")
        //    {
        //        //we need to log in
        //        Login();

        //        homePageLoginText = CheckForLogin();
        //        if (homePageLoginText == "Logout")
        //        {
        //            successfullyLoggedin = true;
        //        }
        //    }
        //    else if (homePageLoginText == "Logout")
        //    {
        //        //we don't need to log in, we can continue successfully
        //        successfullyLoggedin = true;
        //    }

        //    Assert.IsTrue(successfullyLoggedin);

        //    //Act
        //    string webURL = _webUrl + "home/updateimage?setnum=" + setNum;
        //    _driver.Navigate().GoToUrl(webURL);
        //    webLoaded = (_driver.Url == webURL);
        //    OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/div[1]/span/strong");
        //    OpenQA.Selenium.IWebElement imageData = _driver.FindElementByXPath(@"/html/body/div/main/div[3]/div[1]/a/img");

        //    //Assert
        //    Assert.IsTrue(webLoaded);
        //    Assert.IsTrue(data != null);
        //    Assert.IsTrue(data.Text != null);
        //    Assert.IsTrue(imageData != null);
        //    Assert.IsTrue(imageData.Text != null);
        //    Console.WriteLine(imageData.GetAttribute("src"));
        //    Assert.IsTrue(imageData.GetAttribute("src") != null); //Make sure the element was assigned an image, and hence the bing search is working
        //}

        private string CheckForLogin()
        {

            string webLoginURL = _webUrl + "home/index";
            _driver.Navigate().GoToUrl(webLoginURL);
            IWebElement loginHeader;
            IEnumerable<IWebElement> elements = _driver.FindElements(By.XPath(@"/html/body/header/nav/div/div/ul[1]/li[2]/a"));
            if (elements.Any())
            {
                loginHeader = _driver.FindElementByXPath(@"/html/body/header/nav/div/div/ul[1]/li[2]/a");
            }
            else
            {
                loginHeader = _driver.FindElementByXPath(@"/html/body/header/nav/div/div/ul[1]/li[2]/form/button");
            }
            return loginHeader.Text;
        }

        private void Login()
        {
            SetupKeyVault();

            //browse to the login page
            string webURL = _webUrl + "Identity/Account/Login";
            _driver.Navigate().GoToUrl(webURL);

            //Set the login email and password
            IWebElement emailText = _driver.FindElementByXPath(@"//*[@id=""Input_Email""]");
            IWebElement passwordText = _driver.FindElementByXPath(@"//*[@id=""Input_Password""]");
            emailText.SendKeys(_email);
            passwordText.SendKeys(_password);

            //Login
            IWebElement loginButton = _driver.FindElement(By.XPath(@"//*[@id=""account""]/div[5]/button"));
            loginButton.Click();

            //Wait for the login to finish
            Thread.Sleep(3000);

        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(_keyVaultClientId, _keyVaultClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        private void SetupKeyVault()
        {
            using (KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken)))
            {
                _email = keyVaultClient.GetSecretAsync(_keyVaultURL, "IdentitySamsAppEmail").Result.Value;
                _password = keyVaultClient.GetSecretAsync(_keyVaultURL, "IdentitySamsAppPassword").Result.Value;
            }
        }

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                throw new Exception("Select test settings file to continue");
            }
            else
            {
                _webUrl = TestContext.Properties["WebsiteUrl"].ToString();
                _environment = TestContext.Properties["TestEnvironment"].ToString();
                _keyVaultURL = TestContext.Properties["KeyVaultURL"].ToString();
                _keyVaultClientId = TestContext.Properties["KeyVaultClientId"].ToString();
                _keyVaultClientSecret = TestContext.Properties["KeyVaultClientSecret"].ToString();
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
