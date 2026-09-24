using TestValidatorSystem.Api.Models;

namespace TestValidatorSystem.Api.Services
{
    public interface IProcessService
    {
        Task<ResponseModel> ProcessAsync(RequestModel request, CancellationToken ct = default);
    }
}
