using Dapper;
using Npgsql;
using TestValidatorSystem.Api.Models;
using TestValidatorSystem.Api.Sql;

namespace TestValidatorSystem.Api.Repositories
{
    /// <summary>
    /// Репозиторий для работы с таблицей elements через Dapper.
    /// </summary>
    public class ProcessRepository : IProcessRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProcessRepository> _logger;

        public ProcessRepository(IConfiguration configuration, ILogger<ProcessRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
            _logger = logger;
        }

        public async Task SaveElementsAsync(IEnumerable<ElementModel> elements, CancellationToken ct = default)
        {
            var list = elements.ToList();
            if (list.Count == 0)
            {
                _logger.LogInformation("Нечего сохранять: список элементов пуст.");
                return;
            }

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                new CommandDefinition(DbQueries.InsertElement, list, cancellationToken: ct));

            _logger.LogInformation("Сохранено {Count} элементов в БД.", list.Count);
        }

        public async Task<IReadOnlyList<ElementModel>> GetAllElementsAsync(CancellationToken ct = default)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.QueryAsync<ElementModel>(
                new CommandDefinition(DbQueries.SelectAllElements, cancellationToken: ct));
            return result.AsList();
        }
    }
}
