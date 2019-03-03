using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.EFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Tests.ServiceUnitTests
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
