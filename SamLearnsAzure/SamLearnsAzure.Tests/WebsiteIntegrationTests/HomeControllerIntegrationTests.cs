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
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
            };

            //Act
            HttpResponseMessage response = await client.GetAsync("home/index");
           
            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetSetViewIntegrationTest()
        {
            //Arrange
            string setNum = "75218-1";
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
            };

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
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
            };

            //Act
            HttpResponseMessage response = await client.GetAsync("home/about");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        //[TestMethod]
        //public async Task GetContactViewIntegrationTest()
        //{
        //    //Arrange
        //    //string setNum = "75218-1";
        //    HttpClient client = new HttpClient
        //    {
        //        BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
        //    };

        //    //Act
        //    HttpResponseMessage response = await client.GetAsync("home/contact");

        //    //Assert
        //    response.EnsureSuccessStatusCode();
        //    Assert.IsTrue(true);
        //}

        [TestMethod]
        public async Task GetPrivacyViewIntegrationTest()
        {
            //Arrange
            //string setNum = "75218-1";
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
            };

            //Act
            HttpResponseMessage response = await client.GetAsync("home/privacy");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetErrorViewIntegrationTest()
        {
            //Arrange
            //string setNum = "75218-1";
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["AppSettings:WebURL"])
            };

            //Act
            HttpResponseMessage response = await client.GetAsync("home/error");

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.IsTrue(true);
        }


    }
}
