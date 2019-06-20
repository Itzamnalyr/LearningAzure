using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class UpdateImageViewModel
    {
        public Sets Set { get; set; }
        public List<SetImages> SetImages { get; set; }
        public string BaseSetImagesStorageURL{ get; set; }
    }
}
