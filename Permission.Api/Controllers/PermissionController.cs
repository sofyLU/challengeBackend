using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Permission.Api.DTOs;
using Permission.Application.Commands;
using Permission.Common.DTOs;

namespace Permission.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public PermissionController(ILogger<PermissionController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> RequestPermission(PermissionRequest request)
        {
            try
            {
                var command = new RequestPermissionCommand 
                { 
                    EmployeeName = request.EmployeeName,
                    EmployeeSurName = request.EmployeeSurName,
                    PermissionTypeId = request.PermissionTypeId
                };

                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status201Created, new RequestPermissionResponse
                {
                    Message = "New request permission completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");

                return BadRequest(new BaseResponse
                {
                    Message= ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create permission";

                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new RequestPermissionResponse 
                { 
                    Message = SAFE_ERROR_MESSAGE 
                });
            }
            
        }
    }
}
