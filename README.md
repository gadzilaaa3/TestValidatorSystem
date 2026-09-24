# TestValidatorSystem — тестовое задание C# .NET Middle (backend, REST API)

REST API на .NET 10, которое принимает JSON с CSS-селектором, HTML-страницей,
зашифрованным текстом и ключом, а затем выполняет парсинг HTML, извлекает
элементы по селектору, сохраняет их в PostgreSQL, находит email-адреса и
расшифровывает AES-256 (ECB, PaddingMode.None).

## Стек

- **.NET 10** — ASP.NET Core Web API с контроллерами
- **Dapper** — доступ к PostgreSQL
- **Npgsql** — драйвер PostgreSQL
- **AngleSharp** — парсинг HTML в DOM
- **FluentValidation** — валидация входного объекта
- **Swashbuckle.AspNetCore** — Swagger UI
- **PostgreSQL 18** и **pgAdmin 4** — в Docker Compose

## Запуск

Требуется установленный Docker Desktop.

```bash
docker compose up --build