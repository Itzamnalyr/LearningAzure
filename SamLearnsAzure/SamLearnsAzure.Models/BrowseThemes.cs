namespace SamLearnsAzure.Models
{
    public partial class BrowseThemes
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public int? TopParentId { get; set; }
        public int SetCount { get; set; }
        public string? ThemeName { get; set; }
    }
}