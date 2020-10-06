using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(string environment, List<OwnerSets> ownerSets)
        {
            Environment = environment;
            OwnerSets = ownerSets;
        }

        public string Environment { get; set; }
        public List<OwnerSets> OwnerSets { get; set; }
    }
}
