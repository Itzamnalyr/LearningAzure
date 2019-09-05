using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Models
{
    public partial class FeatureFlags
    {
        int Id{ get; set; }
        string Name{ get; set; }
        bool EnabledInDev{ get; set; }
        bool EnabledInQA{ get; set; }
        bool EnabledInProd{ get; set; }
        DateTime LastUpdated{ get; set; }
    }
}
