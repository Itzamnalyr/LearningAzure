using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class PartImages
    {
        public int PartImageId { get; set; }
        public string PartNum { get; set; }
        public string SourceImage { get; set; }
        public int ColorId { get; set; }
        public DateTime LastUpdated { get; set; }

        public Colors Color { get; set; }
    }
}
