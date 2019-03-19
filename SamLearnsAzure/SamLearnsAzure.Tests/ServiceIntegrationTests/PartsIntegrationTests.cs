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

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class PartsInterationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetPartsIntegrationTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/parts/getparts?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<Parts> items = await response.Content.ReadAsAsync<IEnumerable<Parts>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }

        //[TestMethod]
        //public async Task GetPartsSummaryIntegrationTest()
        //{
        //    //Arrange
        //    PartsController controller = new PartsController(new PartsRepository(base.DBContext));
        //    string setNum = "75218-1";

        //    //Act
        //    IEnumerable<PartsSummary> parts = await controller.GetPartsSummary(setNum);

        //    //Assert
        //    Assert.IsTrue(parts != null);
        //    Assert.IsTrue(parts.Count() > 0);
        //}

    }
}
