using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Parts
    {
        public Parts()
        {
            InventoryParts = new HashSet<InventoryParts>();
        }

        public string? PartNum { get; set; }
        public string? Name { get; set; }
        public int? PartCatId { get; set; }
        public int? PartMaterialId { get; set; }

        public ICollection<InventoryParts> InventoryParts { get; set; }
    }
}
