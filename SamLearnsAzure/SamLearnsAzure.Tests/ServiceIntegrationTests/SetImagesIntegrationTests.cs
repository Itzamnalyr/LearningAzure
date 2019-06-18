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
            string setNum = "75218-1";

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
        public async Task GetSetImageWithoutCacheTest()
        {
            //Arrange
            string setNum = "75218-1";

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
        public async Task GetSetImageWithoutCacheAndForceBingSearchTest()
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
        public async Task SaveSetImageTest()
        {
            //Arrange
            string setNum = "75218-2";
            string fileName = "testfilename.test";

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/setimages/savesetimage?setnum=" + setNum+ "&fileName="+ fileName);
            response.EnsureSuccessStatusCode();
            SetImages setImage = await response.Content.ReadAsAsync<SetImages>();
            response.Dispose();

            //Assert
            Assert.IsTrue(setImage != null);
            Assert.IsTrue(setImage.SetNum == setNum);
            Assert.IsTrue(setImage.SetImage != null); //We are including this in the repo, so want to test it specifically
        }
    }
}
