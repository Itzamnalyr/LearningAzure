using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Service.Models
{
    public partial class InventorySets
    {
        public int InventoryId { get; set; }
        public string SetNum { get; set; }
        public int? Quantity { get; set; }
        public int InventorySetId { get; set; }

        public Inventories Inventory { get; set; }
        public Sets Set { get; set; }
    }
}
