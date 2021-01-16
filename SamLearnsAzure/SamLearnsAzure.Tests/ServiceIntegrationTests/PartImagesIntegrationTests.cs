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
using Newtonsoft.Json;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestB")]
    public class PartImagesIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetPartImagesIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimages?useCache=true");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<PartImages> items = JsonConvert.DeserializeObject<IEnumerable<PartImages>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().PartImageId > 0);
                Assert.IsTrue(items.FirstOrDefault().PartNum.Length > 0);
            }
        }

        [TestMethod]
        public async Task GetPartImagesIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimages?useCache=false");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<PartImages> items = JsonConvert.DeserializeObject<IEnumerable<PartImages>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().PartImageId > 0);
                Assert.IsTrue(items.FirstOrDefault().PartNum.Length > 0);
            }
        }

        [TestMethod]
        public async Task GetPartImageIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string partNum = "13195pr0001";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimage?useCache=false&partNum=" + partNum);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                PartImages item = JsonConvert.DeserializeObject<PartImages>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(item != null);
                Assert.IsTrue(item?.PartImageId > 0);
                Assert.IsTrue(item?.PartNum.Length > 0);
            }
        }

        [TestMethod]
        public async Task GetPartImageIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string partNum = "13195pr0001";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/getpartimage?useCache=true&partNum=" + partNum);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                PartImages item = JsonConvert.DeserializeObject<PartImages>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(item != null);
                Assert.IsTrue(item?.PartImageId > 0);
                Assert.IsTrue(item?.PartNum.Length > 0);
            }
        }

        [TestMethod]
        public async Task SavePartImageIntegrationTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string partNum = "13195pr0001";
                string sourceImage = "http://i.ebayimg.com/00/s/NTAwWDU5Mg==/z/EgIAAOSwnDZT8iRD/$_35.JPG";
                int colorId = 326;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/savepartimage?partNum=" + partNum + "&sourceImage=" + sourceImage + "&colorId=" + colorId);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                PartImages item = JsonConvert.DeserializeObject<PartImages>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(item != null);
                Assert.IsTrue(item?.PartImageId > 0);
                Assert.IsTrue(item?.PartNum.Length > 0);
            }
        }

        //TODO: REsolve this flaky test
        //[TestMethod]
        //public async Task SearchForPotentialPartImagesIntegrationTest()
        //{
        //    if (base.Client != null)
        //    {
        //        //Arrange
        //        string partNum = "13195pr0001";
        //        int colorId = 326;
        //        string colorName = "Olive Green";
        //        int resultsToReturn = 2;
        //        int resultsToSearch = 4;

        //        //Act
        //        HttpResponseMessage response = await base.Client.GetAsync("/api/partimages/SearchForPotentialPartImages?partNum=" + partNum + "&colorId=" + colorId + "&colorName=" + colorName + "&resultsToReturn=" + resultsToReturn + "&resultsToSearch=" + resultsToSearch);
        //        response.EnsureSuccessStatusCode();
        //        List<PartImages> partImages = JsonConvert.DeserializeObject<List<PartImages>>();
        //        response.Dispose();

        //        //Assert
        //        Assert.IsTrue(partImages != null);
        //        Assert.IsTrue(partImages.Any());
        //    }
        //}

    }
}
