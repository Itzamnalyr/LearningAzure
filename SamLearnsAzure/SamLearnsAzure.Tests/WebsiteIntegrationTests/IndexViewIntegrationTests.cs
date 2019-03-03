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
//using SamLearnsAzure.Web.Controllers;
//using Microsoft.AspNetCore.Mvc;

//namespace SamLearnsAzure.Tests.WebsiteIntegrationTests
//{
//    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
//    [TestClass]
//    [TestCategory("IntegrationTest")]
//    public class IndexViewIntegrationTests : BaseIntegrationTest
//    {

//        [TestMethod]
//        public async Task GetIndexViewIntegrationTest()
//        {
//            //Arrange
//            string setNum = "75218-1";
//            ServiceAPIClient client = new ServiceAPIClient(this.Configuration);
//            HomeController controller = new HomeController(client, this.Configuration);

//            //Act
//            ActionResult result = await controller.Index();
//            //string model = (result as ViewResult).Model as string;
//            //Assert.IsTrue(model != null);
//            //HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getsetparts?setnum=" + setNum);
//            //response.EnsureSuccessStatusCode();
//            //IEnumerable<SetParts> items = await response.Content.ReadAsAsync<IEnumerable<SetParts>>();

//            //Assert
//            //Assert.IsTrue(items != null);
//            //Assert.IsTrue(items.Count() > 0); //There is more than one
//            //Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
//            //Assert.IsTrue(items.FirstOrDefault().PartName.Length > 0); //The first item has an name
//        }

//    }
//}
