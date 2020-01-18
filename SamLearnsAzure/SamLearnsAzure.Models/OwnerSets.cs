using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class OwnerSets
    {
        public OwnerSets()
        {
            Owner = new Owners();
            Set = new Sets();
        }
        
        public string SetNum { get; set; } = null!;
        public int OwnerId { get; set; }
        public bool Owned { get; set; }
        public bool Wanted { get; set; }
        public int OwnerSetId { get; set; }

        public Owners Owner { get; set; }
        public Sets Set { get; set; }
    }
}
