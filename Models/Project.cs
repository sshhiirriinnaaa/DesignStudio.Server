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
        public string? Image { get; set; } // Знак вопроса означает, что фото может не быть
    }
}