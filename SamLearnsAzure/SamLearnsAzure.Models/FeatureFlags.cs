using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Models
{
    public partial class FeatureFlags
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool EnabledInDev { get; set; }
        public bool EnabledInQA { get; set; }
        public bool EnabledInProd { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
