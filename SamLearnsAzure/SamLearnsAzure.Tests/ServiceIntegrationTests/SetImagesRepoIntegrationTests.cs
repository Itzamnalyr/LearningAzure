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
    public class SetImagesRepoIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetImageNullSetNumTest()
        {
            if (base.DbOptions != null)
            {
                //Arrange
                SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
                SetImagesRepository repo = new SetImagesRepository(context);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                SetImages setImage = await repo.GetSetImage(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(setImage != null);
                Assert.AreEqual(null, setImage?.SetNum);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
