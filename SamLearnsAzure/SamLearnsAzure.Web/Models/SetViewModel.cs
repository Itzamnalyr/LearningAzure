using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Models
{
    public class SetViewModel
    {
        public SetViewModel(Sets set, SetImages setImage, List<SetParts> setParts, string baseSetPartsImagesStorageURL, string baseSetImagesStorageURL)
        {
            Set = set;
            SetImage = setImage;
            SetParts = setParts;
            BaseSetPartsImagesStorageURL = baseSetPartsImagesStorageURL;
            BaseSetImagesStorageURL = baseSetImagesStorageURL;
        }
        public Sets Set { get; set; }
        public SetImages SetImage { get; set; }
        public List<SetParts> SetParts { get; set; }
        public string BaseSetPartsImagesStorageURL { get; set; }
        public string BaseSetImagesStorageURL { get; set; }
    }
}
