using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class SetImages
    {
        public SetImages(string setNum, string setImage)
        {
            SetNum = setNum;
            SetImage = setImage;
        }

        public int SetImageId { get; set; }
        public string SetNum { get; set; }
        public string SetImage { get; set; }
    }
}
