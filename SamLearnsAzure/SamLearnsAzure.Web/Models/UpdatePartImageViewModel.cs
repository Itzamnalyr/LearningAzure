using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class UpdatePartImageViewModel
    {
        public Sets Set{ get; set; }
        public SetParts CurrentSetPart { get; set; }
        public List<PartImages> PotentialSetParts { get; set; }
        public string BasePartsImagesStorageURL { get; set; }
    }

    public class SetPartImages : SetParts
    {
        public string PartImage { get; set; }
    }
}
