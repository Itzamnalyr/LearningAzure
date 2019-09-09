using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Controllers
{

    public interface IFeatureFlagsServiceApiClient
    {
        Task<bool> CheckFeatureFlag(string name, string environment);
    }

}
