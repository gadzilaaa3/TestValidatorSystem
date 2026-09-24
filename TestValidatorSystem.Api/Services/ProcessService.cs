using AngleSharp.Html.Parser;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TestValidatorSystem.Api.Models;
using TestValidatorSystem.Api.Repositories;

namespace TestValidatorSystem.Api.Services
{
    public class ProcessService : IProcessService
    {
        private static readonly Regex EmailRegex = new(
            @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IProcessRepository _repository;
        private readonly ILogger<ProcessService> _logger;

        public ProcessService(IProcessRepository repository, ILogger<ProcessService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResponseModel> ProcessAsync(RequestModel request, CancellationToken ct = default)
        {
            var url = Encoding.UTF8.GetString(Convert.FromBase64String(request.UrlB64));
            var html = Encoding.UTF8.GetString(Convert.FromBase64String(request.PageB64));
            var keyBytes = Convert.FromBase64String(request.KeyBytesB64);
            var encryptedBytes = Convert.FromBase64String(request.EncryptedTextBytesB64);

            // 2. Парсим HTML.
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html, ct);
            var nodes = document.QuerySelectorAll(request.Selector);

            // 3. Собираем элементы.
            var elements = new List<ElementModel>();
            foreach (var node in nodes)
            {
                elements.Add(new ElementModel
                {
                    AttributeValue = node.GetAttribute(request.Attribute),
                    HtmlCode = node.OuterHtml
                });
            }

            var response = new ResponseModel
            {
                Url = url,
                ElementsCount = elements.Count,
                ElementsAttrList = elements
                    .Where(e => !string.IsNullOrEmpty(e.AttributeValue))
                    .Select(e => e.AttributeValue!)
                    .ToList()
            };

            // 4. Сохраняем в БД.
            try
            {
                await _repository.SaveElementsAsync(elements, ct);
            }
            catch (Npgsql.NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка сохранения в БД.");
                return Error("DB_ERROR", "Не удалось сохранить элементы в базу данных.");
            }

            // 5. Email-адреса.
            response.EmailsList = EmailRegex.Matches(html)
                .Select(m => m.Value).Distinct().ToList();
            response.EmailsCount = response.EmailsList.Count;

            // 6. AES.
            try
            {
                response.DecryptedPlainText = DecryptAesEcb(encryptedBytes, keyBytes);
            }
            catch (CryptographicException ex)
            {
                _logger.LogWarning(ex, "Ошибка расшифровки AES.");
                return Error("DECRYPT_ERROR", $"Не удалось расшифровать текст: {ex.Message}");
            }

            return response;
        }

        private static string DecryptAesEcb(byte[] cipherText, byte[] key)
        {
            if (key.Length != 32)
                throw new CryptographicException($"Длина ключа должна быть 32 байта, получено {key.Length}.");
            if (cipherText.Length % 16 != 0)
                throw new CryptographicException($"Длина шифротекста должна быть кратна 16, получено {cipherText.Length}.");

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            var txt = Encoding.UTF8.GetString(plainBytes);
            return txt;
        }

        private static ResponseModel Error(string code, string message) => new()
        {
            IsError = 1,
            ErrorCode = code,
            ErrorMessage = message
        };
    }
}
