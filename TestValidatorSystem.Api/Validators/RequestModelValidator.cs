using FluentValidation;
using TestValidatorSystem.Api.Models;

namespace TestValidatorSystem.Api.Validators
{
    public class RequestModelValidator : AbstractValidator<RequestModel>
    {
        private const int AesKeyLength = 32;
        private const int AesBlockSize = 16;

        public RequestModelValidator()
        {
            // --- selector ---
            RuleFor(x => x.Selector)
                .NotEmpty().WithErrorCode("EMPTY_SELECTOR")
                .WithMessage("Selector не может быть пустым.");

            // --- attribute ---
            RuleFor(x => x.Attribute)
                .NotEmpty().WithErrorCode("EMPTY_ATTRIBUTE")
                .WithMessage("Attribute не может быть пустым.");

            // --- url_b64 ---
            RuleFor(x => x.UrlB64)
                .NotEmpty().WithErrorCode("MISSING_URL")
                .WithMessage("Поле url_b64 отсутствует или пустое.")
                .Must(BeValidBase64).WithErrorCode("INVALID_BASE64_URL")
                .WithMessage("Поле url_b64 не является корректной Base64-строкой.");

            // --- page_b64 ---
            RuleFor(x => x.PageB64)
                .NotEmpty().WithErrorCode("MISSING_PAGE")
                .WithMessage("Поле page_b64 отсутствует или пустое.")
                .Must(BeValidBase64).WithErrorCode("INVALID_BASE64_PAGE")
                .WithMessage("Поле page_b64 не является корректной Base64-строкой.");

            // --- key_bytes_b64 ---
            RuleFor(x => x.KeyBytesB64)
                .NotEmpty().WithErrorCode("MISSING_KEY")
                .WithMessage("Поле key_bytes_b64 отсутствует или пустое.")
                .Must(BeValidBase64).WithErrorCode("INVALID_BASE64_KEY")
                .WithMessage("Поле key_bytes_b64 не является корректной Base64-строкой.")
                .Must(BeAes256Key).WithErrorCode("INVALID_KEY_LENGTH")
                .WithMessage($"Длина ключа AES-256 должна быть {AesKeyLength} байт.");

            // --- encrypted_text_bytes_b64 ---
            RuleFor(x => x.EncryptedTextBytesB64)
                .NotEmpty().WithErrorCode("MISSING_ENCRYPTED_TEXT")
                .WithMessage("Поле encrypted_text_bytes_b64 отсутствует или пустое.")
                .Must(BeValidBase64).WithErrorCode("INVALID_BASE64_ENCRYPTED_TEXT")
                .WithMessage("Поле encrypted_text_bytes_b64 не является корректной Base64-строкой.")
                .Must(BeAesBlockAligned).WithErrorCode("INVALID_CIPHER_LENGTH")
                .WithMessage($"Длина шифротекста должна быть кратна {AesBlockSize} байтам (PaddingMode.None).");
        }

        private static bool BeValidBase64(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;
            try { Convert.FromBase64String(input); return true; }
            catch (FormatException) { return false; }
        }

        private static bool BeAes256Key(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;
            if (!BeValidBase64(input)) return true;
            return Convert.FromBase64String(input).Length == AesKeyLength;
        }

        private static bool BeAesBlockAligned(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;
            if (!BeValidBase64(input)) return true;
            return Convert.FromBase64String(input).Length % AesBlockSize == 0;
        }
    }
}
