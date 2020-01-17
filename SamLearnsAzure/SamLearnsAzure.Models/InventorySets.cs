using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class InventorySets
    {
        public InventorySets(string setNum)
        {
            Inventory = new Inventories();
            Set = new Sets(setNum);
            SetNum = setNum;
        }

        public int InventoryId { get; set; }
        public string SetNum { get; set; }
        public int Quantity { get; set; }
        public int InventorySetId { get; set; }

        public Inventories Inventory { get; set; }
        public Sets Set { get; set; }
    }
}
