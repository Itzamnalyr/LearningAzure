//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using SamLearnsAzure.Models;
//using System.Data.SqlClient;
//using SamLearnsAzure.Service.Controllers;
//using Moq;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using System;
//using SamLearnsAzure.Service.DataAccess;
//using System.Net.Http;

//namespace SamLearnsAzure.Tests.ServiceIntegrationTests
//{
//    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
//    [TestClass]
//    [TestCategory("IntegrationTest")]
//    [TestCategory("RedisTest")]
//    public class PartImagesIntegrationTests : BaseIntegrationTest
//    {
//        [TestMethod]
//        public async Task GetPartImagesIntegrationWithCacheTest()
//        {
//            //Arrange

//            //Act
//            HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimages?useCache=true");
//            response.EnsureSuccessStatusCode();
//            IEnumerable<PartImages> items = await response.Content.ReadAsAsync<IEnumerable<PartImages>>();
//            response.Dispose();

//            //Assert
//            Assert.IsTrue(items != null);
//            Assert.IsTrue(items.Count() == 0); //There is more than one
//            //Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
//            //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
//        }

//        [TestMethod]
//        public async Task GetPartImagesIntegrationWithoutCacheTest()
//        {
//            //Arrange

//            //Act
//            HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimages?useCache=false");
//            response.EnsureSuccessStatusCode();
//            IEnumerable<PartImages> items = await response.Content.ReadAsAsync<IEnumerable<PartImages>>();
//            response.Dispose();

//            //Assert
//            Assert.IsTrue(items != null);
//            Assert.IsTrue(items.Count() == 0); //There is more than one
//            //Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
//            //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
//        }

//        [TestMethod]
//        public async Task GetPartImageIntegrationWithoutCacheTest()
//        {
//            //Arrange
//            string partNum = "";

//            //Act
//            HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimages?useCache=false&partNum=" + partNum);
//            response.EnsureSuccessStatusCode();
//            IEnumerable<PartImages> items = await response.Content.ReadAsAsync<IEnumerable<PartImages>>();
//            response.Dispose();

//            //Assert
//            Assert.IsTrue(items != null);
//            Assert.IsTrue(items.Count() == 0); //There is more than one
//            //Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
//            //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
//        }

//        [TestMethod]
//        public async Task GetPartImageIntegrationWithCacheTest()
//        {
//            //Arrange
//            string partNum = "";

//            //Act
//            HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimage?useCache=true&partNum=" + partNum);
//            response.EnsureSuccessStatusCode();
//            PartImages item = await response.Content.ReadAsAsync<PartImages>();
//            response.Dispose();

//            //Assert
//            Assert.IsTrue(item == null);
//            //Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
//            //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
//        }

//    }
//}
