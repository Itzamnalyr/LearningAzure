using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class PartRelationships
    {
        public string RelType { get; set; }
        public string ChildPartNum { get; set; }
        public string ParentPartNum { get; set; }
        public int PartRelationshipId { get; set; }
    }
}
