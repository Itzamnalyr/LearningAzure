using System.Collections.Generic;
using SamLearnsAzure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SamLearnsAzure.Web.Models
{
    public class BrowseViewModel
    {
        public BrowseViewModel()
        {
        }

        public BrowseViewModel(string environment, List<BrowseThemes> themes, List<BrowseYears> years, List<BrowseSets> sets)
        {
            Environment = environment;
            Themes = new List<SelectListItem>();
            Themes.Add(new SelectListItem("<All themes>", null));
            foreach (BrowseThemes item in themes)
            {
                Themes.Add(new SelectListItem(item.ThemeName, item.Id.ToString()));
            }
            Years = new List<SelectListItem>();
            Years.Add(new SelectListItem("<All years>", null));
            foreach (BrowseYears item in years)
            {
                Years.Add(new SelectListItem(item.YearName, item.Year.ToString()));
            }
            Sets = sets;
        }

        public string Environment { get; set; }
        public int? ThemeId { get; set; }
        public List<SelectListItem> Themes { get; set; }
        public int? Year { get; set; }
        public List<SelectListItem> Years { get; set; }
        public List<BrowseSets> Sets { get; set; }
        public bool BrowseFeatureFlag { get; set; }
    }
}
