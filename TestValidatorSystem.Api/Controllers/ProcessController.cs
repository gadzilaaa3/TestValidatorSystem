using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestValidatorSystem.Api.Models;
using TestValidatorSystem.Api.Services;

namespace TestValidatorSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly IValidator<RequestModel> _validator;
        private readonly IProcessService _service;
        private readonly ILogger<ProcessController> _logger;

        public ProcessController(
            IValidator<RequestModel> validator,
            IProcessService service,
            ILogger<ProcessController> logger)
        {
            _validator = validator;
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Process([FromBody] RequestModel request, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                var first = validation.Errors.First();
                return BadRequest(new ResponseModel
                {
                    IsError = 1,
                    ErrorCode = first.ErrorCode,
                    ErrorMessage = first.ErrorMessage
                });
            }

            var result = await _service.ProcessAsync(request, ct);
            return Ok(result);
        }
    }
}
