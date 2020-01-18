using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class UpdatePartImageViewModel
    {
        public UpdatePartImageViewModel(Sets set, SetParts currentSetPart, List<PartImages> potentialSetParts, string basePartsImagesStorageURL)
        {
            Set = set;
            CurrentSetPart = currentSetPart;
            PotentialSetParts = potentialSetParts;
            BasePartsImagesStorageURL = basePartsImagesStorageURL;
        }

        public Sets Set{ get; set; }
        public SetParts CurrentSetPart { get; set; }
        public List<PartImages> PotentialSetParts { get; set; }
        public string BasePartsImagesStorageURL { get; set; }
    }

    public class SetPartImages : SetParts
    {
        public SetPartImages(string partNum, string partImage)
        {
            base.PartNum = partNum;
            PartImage = partImage;
        }

        public string PartImage { get; set; }
    }
}
