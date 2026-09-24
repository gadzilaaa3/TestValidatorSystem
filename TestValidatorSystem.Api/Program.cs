using FluentValidation;
using System.Text.Json;
using TestValidatorSystem.Api.Middleware;
using TestValidatorSystem.Api.Repositories;
using TestValidatorSystem.Api.Services;
using TestValidatorSystem.Api.Validators;

namespace TestValidatorSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- JSON ---
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // --- Swagger (Swashbuckle) ---
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // --- Валидация ---
            builder.Services.AddValidatorsFromAssemblyContaining<RequestModelValidator>();

            // --- Слои приложения ---
            builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
            builder.Services.AddScoped<IProcessService, ProcessService>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            // --- Swagger UI по адресу /api/swagger (требование ТЗ, п.5) ---
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api/swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestValidatorSystem API v1");
            });

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
