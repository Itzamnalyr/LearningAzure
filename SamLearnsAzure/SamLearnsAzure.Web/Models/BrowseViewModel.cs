using System.Collections.Generic;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Web.Models
{
    public class BrowseViewModel
    {
        public BrowseViewModel(string environment, List<Themes> themes)
        {
            Environment = environment;
            Themes = themes;
        }

        public string Environment { get; set; }
        public List<Themes> Themes { get; set; }
    }
}
