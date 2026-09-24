using TestValidatorSystem.Api.Models;

namespace TestValidatorSystem.Api.Repositories
{
    public interface IProcessRepository
    {
        Task SaveElementsAsync(IEnumerable<ElementModel> elements, CancellationToken ct = default);
        Task<IReadOnlyList<ElementModel>> GetAllElementsAsync(CancellationToken ct = default);
    }
}
