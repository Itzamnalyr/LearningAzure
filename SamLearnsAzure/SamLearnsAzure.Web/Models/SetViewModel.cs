using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class SetViewModel
    {
        public Sets Set { get; set; }
        public SetImages SetImage { get; set; }
        public List<SetParts> SetParts { get; set; }
        public string BaseSetPartsImagesStorageURL { get; set; }
        public string BaseSetImagesStorageURL { get; set; }
    }
}
