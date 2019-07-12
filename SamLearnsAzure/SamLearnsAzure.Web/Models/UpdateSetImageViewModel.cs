using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class UpdateSetImageViewModel
    {
        public Sets Set { get; set; }
        public List<SetImages> PotentialSetImages { get; set; }
        public string BaseSetImagesStorageURL{ get; set; }
    }
}
