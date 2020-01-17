using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Owners
    {
        public Owners(string ownerName)
        {
            OwnerSets = new HashSet<OwnerSets>();
            OwnerName = ownerName;
        }

        public int Id { get; set; }
        public string OwnerName { get; set; }

        public ICollection<OwnerSets> OwnerSets { get; set; }
    }
}
