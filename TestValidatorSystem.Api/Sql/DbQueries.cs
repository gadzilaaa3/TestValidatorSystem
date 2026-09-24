namespace TestValidatorSystem.Api.Sql
{
    /// <summary>
    /// Централизованное хранилище SQL-запросов.
    /// Позволяет переиспользовать запросы и избегать дублирования.
    /// </summary>
    public static class DbQueries
    {
        public const string InsertElement = """
        INSERT INTO elements (attribute_value, html_code)
        VALUES (@AttributeValue, @HtmlCode);
        """;

        public const string SelectAllElements = """
        SELECT id, attribute_value, html_code, created_at
        FROM elements
        ORDER BY id;
        """;
    }
}
