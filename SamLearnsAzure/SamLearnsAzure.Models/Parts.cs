using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Parts
    {
        public Parts(string partNum)
        {
            InventoryParts = new HashSet<InventoryParts>();
            PartNum = partNum;
        }

        public string PartNum { get; set; }
        public string? Name { get; set; }
        public int? PartCatId { get; set; }
        public int? PartMaterialId { get; set; }

        public ICollection<InventoryParts> InventoryParts { get; set; }
    }
}
