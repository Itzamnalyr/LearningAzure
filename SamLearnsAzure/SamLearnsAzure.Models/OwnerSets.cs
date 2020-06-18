using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class OwnerSets
    {
        public OwnerSets()
        {
        }
        
        public string SetNum { get; set; } = null!;
        public int OwnerId { get; set; }
        public bool Owned { get; set; }
        public bool Wanted { get; set; }
        public int OwnerSetId { get; set; }

        //Replaced Owners object with simple objects 
        public string OwnerName { get; set; } = null!;

        //Replaced Sets object with simple objects
        public string SetName { get; set; } = null!;
        public int SetYear { get; set; }
        public string SetThemeName { get; set; } = null!;
        public int SetNumParts{ get; set; }
    }
}
