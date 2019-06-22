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
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    [TestCategory("RedisTest")]
    public class SetImagesIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetSetImageWithCacheTest()
        {
            //Arrange
            string setNum = "41608-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/getsetimage?setnum=" + setNum + "&useCache=true");
            response.EnsureSuccessStatusCode();
            SetImages setImage = await response.Content.ReadAsAsync<SetImages>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImage != null);
            Assert.IsTrue(setImage.SetNum == setNum);
            Assert.IsTrue(setImage.SetImage != null); //We are including this in the repo, so want to test it specifically
        }

        [TestMethod]
        public async Task GetSetImageWithoutCacheWith1ResultTest()
        {
            //Arrange
            string setNum = "41608-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/getsetimage?setnum=" + setNum + "&useCache=false");
            response.EnsureSuccessStatusCode();
            SetImages setImage = await response.Content.ReadAsAsync<SetImages>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImage != null);
            Assert.IsTrue(setImage.SetNum == setNum);
            Assert.IsTrue(setImage.SetImage != null); //We are including this in the repo, so want to test it specifically
        }

        [TestMethod]
        public async Task GetSetImageWithoutCacheAndForceBingSearchWith1ResultTest()
        {
            //Arrange
            string setNum = "75218-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/getsetimage?setnum=" + setNum + "&useCache=false&forceBingSearch=true");
            response.EnsureSuccessStatusCode();
            SetImages setImage = await response.Content.ReadAsAsync<SetImages>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImage != null);
            Assert.IsTrue(setImage.SetNum == setNum);
            Assert.IsTrue(setImage.SetImage != null); //We are including this in the repo, so want to test it specifically
        }

        [TestMethod]
        public async Task GetSetImageWithoutCacheAndForceBingSearchWith10ResultsTest()
        {
            //Arrange
            string setNum = "75218-1";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/getsetimages?setnum=" + setNum + "&useCache=false&forceBingSearch=true&resultsToReturn=10");
            response.EnsureSuccessStatusCode();
            List<SetImages> setImages = await response.Content.ReadAsAsync<List<SetImages>>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImages != null);
            Assert.IsTrue(setImages.Count <= 10);
            Assert.IsTrue(setImages[0].SetNum == setNum);
            Assert.IsTrue(setImages[0].SetImage != null); //We are including this in the repo, so want to test it specifically
        }


        [TestMethod]
        public async Task SaveSetImageTest()
        {
            //Arrange
            string setNum = "75218-2";
            string imageUrl = "https://samlearnsazure.files.wordpress.com/2019/01/microsoft-certified-azure-solutions-architect-expert.png";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/savesetimage?setnum=" + setNum+ "&imageUrl=" + imageUrl);
            response.EnsureSuccessStatusCode();
            SetImages setImage = await response.Content.ReadAsAsync<SetImages>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImage != null);
            Assert.IsTrue(setImage.SetNum == setNum);
            Assert.IsTrue(setImage.SetImage == "75218-2.png"); //We are including this in the repo, so want to test it specifically
        }
    }
}
