namespace DesignStudio.Server.Models
{
    public class Lead
    {
        public int Id { get; set; } // Уникальный номер заявки
        public string Name { get; set; } = string.Empty; // Имя клиента
        public string Email { get; set; } = string.Empty; // Почта клиента
        public string Message { get; set; } = string.Empty; // Текст сообщения
        public string Status { get; set; } = "new"; // Статус: "new" (новая) или "contacted" (обработана)
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания заявки
    }
}