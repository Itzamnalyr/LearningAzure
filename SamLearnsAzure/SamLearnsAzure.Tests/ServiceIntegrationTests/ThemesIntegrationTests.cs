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
    [TestCategory("RedisTest")]
    public class ThemesServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetThemesIntegrationWithCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/themes/getthemes?useCache=true");
            response.EnsureSuccessStatusCode();
            IEnumerable<Themes> items = await response.Content.ReadAsAsync<IEnumerable<Themes>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }
        [TestMethod]
        public async Task GetThemesIntegrationWithoutCacheTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/themes/getthemes?useCache=false");
            response.EnsureSuccessStatusCode();
            IEnumerable<Themes> items = await response.Content.ReadAsAsync<IEnumerable<Themes>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() > 0); //There is more than one
            Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
            Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }

    }
}
