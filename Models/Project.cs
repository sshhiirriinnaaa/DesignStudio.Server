namespace DesignStudio.Server.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public string DisplayLocation { get; set; } = "both";
    }
}