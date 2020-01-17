using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class Colors
    {
        public Colors()
        {
            InventoryParts = new HashSet<InventoryParts>();
            PartImages = new HashSet<PartImages>();
        }

        //The unique number for the color
        public int Id { get; set; }
        /// <summary>
        /// The name of the color
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The RGB/(Red Green Blue) color of the object
        /// </summary>
        public string? Rgb { get; set; }
        /// <summary>
        /// Is the object transparent?
        /// </summary>
        public bool? IsTrans { get; set; }
        /// <summary>
        /// A collection of inventory parts with this color
        /// </summary>
        public ICollection<InventoryParts> InventoryParts { get; set; }
        public ICollection<PartImages> PartImages { get; set; }
    }
}
