using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Colors
    {
        public Colors()
        {
            InventoryParts = new HashSet<InventoryParts>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Rgb { get; set; }
        public bool? IsTrans { get; set; }

        public ICollection<InventoryParts> InventoryParts { get; set; }
    }
}
