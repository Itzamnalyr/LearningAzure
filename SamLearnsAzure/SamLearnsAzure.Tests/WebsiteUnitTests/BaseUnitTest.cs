using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Tests.WebsiteUnitTests
{
    public class BaseUnitTest
    {
        //public DbContextOptions<SamsAppDBContext> DbOptions;

        //public BaseUnitTest()
        //{
        //    DbOptions = new DbContextOptionsBuilder<SamsAppDBContext>()
        //                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //                    .EnableSensitiveDataLogging()
        //                    .Options; 

        //}

        public T ModelFromActionResult<T>(ActionResult actionResult)
        {
            object model;
            if (actionResult.GetType() == typeof(ViewResult))
            {
                ViewResult viewResult = (ViewResult)actionResult;
                model = viewResult.Model;
            }
            else if (actionResult.GetType() == typeof(PartialViewResult))
            {
                PartialViewResult partialViewResult = (PartialViewResult)actionResult;
                model = partialViewResult.Model;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Actionresult of type {0} is not supported by ModelFromResult extractor.", actionResult.GetType()));
            }
            T typedModel = (T)model;
            return typedModel;
        }
    }
}
