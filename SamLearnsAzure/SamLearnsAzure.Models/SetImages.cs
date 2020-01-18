using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class SetImages
    {
        public SetImages()
        {
        }

        public int SetImageId { get; set; }
        public string SetNum { get; set; } = null!;
        public string SetImage { get; set; } = null!;
    }
}
