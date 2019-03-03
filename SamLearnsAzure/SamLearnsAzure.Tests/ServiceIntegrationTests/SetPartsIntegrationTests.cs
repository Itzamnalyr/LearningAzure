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
    public class SetPartsIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetSetPartsIntegrationTest()
        {
            //Arrange
            string setNum = "75218-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/sets/getsetparts?setnum=" + setNum);
            response.EnsureSuccessStatusCode();
            IEnumerable<SetParts> items = await response.Content.ReadAsAsync<IEnumerable<SetParts>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().PartName.Length > 0); //The first item has an name
        }      

    }
}
