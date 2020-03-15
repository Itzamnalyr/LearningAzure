using System.Collections.Generic;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Web.Models
{
    public class ThemeViewModel
    {
        public ThemeViewModel(string environment, List<Sets> sets)
        {
            Environment = environment;
            Sets = sets;
        }

        public string Environment { get; set; }
        public List<Sets> Sets { get; set; }
    }
}
