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
    public class SetsRepoServiceIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetNullSetNumTest()
        {
            if (base.DbOptions != null)
            {
                //Arrange
                SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
                SetsRepository repo = new SetsRepository(context);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                Sets set = await repo.GetSet(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(set != null);
                Assert.AreEqual(null, set?.SetNum);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
