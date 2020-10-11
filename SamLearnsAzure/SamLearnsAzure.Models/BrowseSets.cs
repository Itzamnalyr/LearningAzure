using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class BrowseSets
    {
        public BrowseSets()
        {
            Theme = new Themes();
            Inventories = new HashSet<Inventories>();
            InventorySets = new HashSet<InventorySets>();
            OwnerSets = new HashSet<OwnerSets>();
        }

        public string SetNum { get; set; } = null!;
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int? ThemeId { get; set; }
        public int NumParts { get; set; }

        public Themes Theme { get; set; }
        public ICollection<Inventories> Inventories { get; set; }
        public ICollection<InventorySets> InventorySets { get; set; }
        public ICollection<OwnerSets> OwnerSets { get; set; }
        
    }
}
