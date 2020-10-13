using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Models
{
    public partial class BrowseSets
    {
        public string SetNum { get; set; } = null!;
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int? ThemeId { get; set; }
        public string? ThemeName { get; set; }
        public int NumParts { get; set; }      
    }
}
