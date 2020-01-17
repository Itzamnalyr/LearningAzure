using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Inventories
    {
        public Inventories()
        {
            Set = new Sets("");
            InventoryParts = new HashSet<InventoryParts>();
            InventorySets = new HashSet<InventorySets>();
        }

        public int Id { get; set; }
        public int? Version { get; set; }
        public string? SetNum { get; set; }

        public Sets Set { get; set; }
        public ICollection<InventoryParts> InventoryParts { get; set; }
        public ICollection<InventorySets> InventorySets { get; set; }
    }
}
