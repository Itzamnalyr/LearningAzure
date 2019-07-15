using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class PartImagesViewModel
    {
        public List<PartImages> PartImages { get; set; }
        public string BasePartsImagesStorageURL { get; set; }
    }
}
