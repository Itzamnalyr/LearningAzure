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
    public class OwnerSetsServiceIntegrationTests : BaseIntegrationTest
    {
        //[TestMethod]
        //public async Task GetOwnerSetsIntegrationTest()
        //{
        //    //Arrange

        //    //Act
        //    HttpResponseMessage response = await base.Client.GetAsync("/api/ownersets/getownersets");
        //    response.EnsureSuccessStatusCode();
        //    IEnumerable<OwnerSets> items = await response.Content.ReadAsAsync<IEnumerable<OwnerSets>>();

        //    //Assert
        //    Assert.IsTrue(items != null);
        //    Assert.IsTrue(items.Count() == 0); //There is more than one owner
        //    //Assert.IsTrue(items.FirstOrDefault().OwnerSetId > 0); //The first item has an id
        //    //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name        
        //}

    }
}
