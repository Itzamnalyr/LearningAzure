using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Service.Models
{
    public partial class Themes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
