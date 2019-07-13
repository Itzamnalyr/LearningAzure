using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class PartImages
    {
        public string PartNum { get; set; }
        public string SourceImageUrl { get; set; }
        public int ColorId { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
