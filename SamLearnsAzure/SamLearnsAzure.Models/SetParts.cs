using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.Models
{
    public class SetParts
    {
        public string? PartNum { get; set; }
        public string? PartName { get; set; }
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public int PartCategoryId { get; set; }
        public string? PartCategoryName { get; set; }
        public int Quantity { get; set; }
    }
}
