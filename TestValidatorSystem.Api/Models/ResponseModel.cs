namespace TestValidatorSystem.Api.Models
{
    /// <summary>
    /// Ответ API. Поля соответствуют пункту 8 тестового задания.
    /// Имена полей в JSON заданы глобальной настройкой snake_case (см. Program.cs).
    /// </summary>
    public class ResponseModel
    {
        public int IsError { get; set; }

        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public int ElementsCount { get; set; }
        public int EmailsCount { get; set; }

        public string Url { get; set; } = string.Empty;

        public List<string> ElementsAttrList { get; set; } = new();
        public List<string> EmailsList { get; set; } = new();
        public string DecryptedPlainText { get; set; } = string.Empty;
    }
}
