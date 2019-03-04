using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using SamLearnsAzure.Models;
using System.Data.SqlClient;
using SamLearnsAzure.Service.Controllers;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using SamLearnsAzure.Service.DataAccess;
using System.Net.Http;
using SamLearnsAzure.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Web.Models;
using Newtonsoft.Json;

namespace SamLearnsAzure.Tests.WebsiteIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class HomeControllerIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetIndexViewIntegrationTest()
        {
            //Arrange
            //string setNum = "75218-1";
            //ServiceAPIClient client = new ServiceAPIClient(this.Configuration);
            //HomeController controller = new HomeController(client, this.Configuration);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);

            //Act
            //IActionResult result = await controller.Index();
            //string model = (result as ViewResult).Model as string;
            //Assert.IsTrue(model != null);
            HttpResponseMessage response = await client.GetAsync("home/index");
            //IEnumerable<SetParts> items = await response.Content.ReadAsAsync<IEnumerable<SetParts>>();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
            //ViewResult viewResult = (ViewResult)result;
            //Assert.IsTrue(viewResult.StatusCode == 200);
            //string responseString = await response.Content.ReadAsStringAsync();
            //Assert.IsTrue(responseString.Contains(testSession.Name));
            //IndexViewModel indexViewModel = JsonConvert.DeserializeObject<IndexViewModel>(responseString);
            //Assert.IsTrue(indexViewModel != null);
            //Assert.IsTrue(indexViewModel.Environment as string == "VS dev");
            //Assert.IsTrue(indexViewModel.OwnerSets.Count() >= 1);

            //Assert.IsTrue(items != null);
            //Assert.IsTrue(items.Count() > 0); //There is more than one
            //Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
            //Assert.IsTrue(items.FirstOrDefault().PartName.Length > 0); //The first item has an name
        }

        [TestMethod]
        public async Task GetSetViewIntegrationTest()
        {
            //Arrange
            string setNum = "75218-1";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);

            //Act
            HttpResponseMessage response = await client.GetAsync("home/set?setnum=" + setNum);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetAboutViewIntegrationTest()
        {
            //Arrange
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);

            //Act
            HttpResponseMessage response = await client.GetAsync("home/about");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetContactViewIntegrationTest()
        {
            //Arrange
            //string setNum = "75218-1";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);

            //Act
            HttpResponseMessage response = await client.GetAsync("home/contact");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetPrivacyViewIntegrationTest()
        {
            //Arrange
            //string setNum = "75218-1";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);

            //Act
            HttpResponseMessage response = await client.GetAsync("home/privacy");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

       

    }
}
