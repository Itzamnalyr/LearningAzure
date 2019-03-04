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
    public class SetsServiceIntegrationTests : BaseIntegrationTest
    {
       
        [TestMethod]
        public async Task GetSetsTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getsets");
            response.EnsureSuccessStatusCode();
            IEnumerable<Sets> items = await response.Content.ReadAsAsync<IEnumerable<Sets>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().SetNum != ""); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }

        [TestMethod]
        public async Task GetSetTest()
        {
            //Arrange
            string setNum = "75218-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getset?setnum=" + setNum);
            response.EnsureSuccessStatusCode();
            Sets set = await response.Content.ReadAsAsync<Sets>();

            //Assert
            Assert.IsTrue(set != null);
            Assert.IsTrue(set.SetNum == setNum);
            Assert.IsTrue(set.Theme != null); //We are including this in the repo, so want to test it specifically
        }

    }
}
