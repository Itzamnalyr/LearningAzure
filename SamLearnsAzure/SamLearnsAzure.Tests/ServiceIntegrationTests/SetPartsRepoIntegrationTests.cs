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
    public class SetPartsRepoIntegrationTests : BaseRepoIntegrationTest
    {

        [TestMethod]
        public async Task GetSetPartsNullSetNumTest()
        {
            if (base.DbOptions != null)
            {
                //Arrange
                SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
                SetPartsRepository repo = new SetPartsRepository(context);
                RedisService? redisService = null;
                bool useCache = false;
                string? setNum = null;

                //Act
#pragma warning disable CS8604 // Possible null reference argument.
                IEnumerable<SetParts> setParts = await repo.GetSetParts(redisService, useCache, setNum);
#pragma warning restore CS8604 // Possible null reference argument.

                //Assert
                Assert.IsTrue(setParts != null);
                Assert.IsTrue(setParts.Count() == 0);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

    }
}
