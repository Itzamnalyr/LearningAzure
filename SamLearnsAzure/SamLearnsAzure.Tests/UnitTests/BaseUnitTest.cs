using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Tests.UnitTests
{
    public class BaseUnitTest
    {
        public DbContextOptions<SamsAppDBContext> DbOptions;

        public BaseUnitTest()
        {
            DbOptions = new DbContextOptionsBuilder<SamsAppDBContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .EnableSensitiveDataLogging()
                            .Options; 

        }
    }
}
