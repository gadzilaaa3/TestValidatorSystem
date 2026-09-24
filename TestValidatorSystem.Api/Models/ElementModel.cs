namespace TestValidatorSystem.Api.Models
{
    /// <summary>
    /// Элемент, извлеченный из HTML-страницы.
    /// Используется и для сохранения в БД, и для чтения.
    /// </summary>
    public class ElementModel
    {
        public long Id { get; set; }
        public string? AttributeValue { get; set; }
        public string HtmlCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
