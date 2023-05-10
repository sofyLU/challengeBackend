using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Permission.Api.DTOs;
using Permission.Application.Commands;
using Permission.Application.Queries;
using Permission.Common.Domain.Models;
using Permission.Common.DTOs;
using Permission.Domain.Entities;
using Serilog;
using System.Net;

namespace Permission.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher<PermissionEntity> _queryDispatcher;

        public PermissionController(ILogger<PermissionController> logger, ICommandDispatcher commandDispatcher,
            IQueryDispatcher<PermissionEntity> queryDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// Save a request permission
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     POST /api/v1/Permission
        ///     {
        ///         "employeeName" : "string",
        ///         "employeeSurName" : "string",
        ///         "permissionType" : int,
        ///     }
        /// </remarks>
        /// <param name="request">Data to create a permission</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(RequestPermissionResponse))]
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
                Log.Information("RequestPermission Index executed at {date}", DateTime.UtcNow);

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
                    Message = ex.Message
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

        /// <summary>
        /// Update a request permission
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     PUTT /api/v1/Permission/:permissionId
        ///     {
        ///         "employeeName" : "string",
        ///         "employeeSurName" : "string",
        ///         "permissionType" : int,
        ///     }
        /// </remarks>
        /// <param name="request">Data to create a permission</param>
        /// <param name="permissionId">Id to identify a permission</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(RequestPermissionResponse))]
        public async Task<ActionResult> ModifyPermission(PermissionModify modify, int permissionId)
        {
            try
            {
                var command = new ModifyPermissionCommand
                {
                    Id = permissionId,
                    EmployeeName = modify.EmployeeName,
                    EmployeeSurName = modify.EmployeeSurName,
                    PermissionTypeId = modify.PermissionTypeId
                };

                await _commandDispatcher.SendAsync(command);
                Log.Information("ModifyPermission Index executed at {date}", DateTime.UtcNow);

                return Ok(new RequestPermissionResponse
                {
                    Message = "New modify permission completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");

                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed incorrect " +
                    "permission Id targetting the aggregate!");

                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to modify permission";

                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        /// <summary>
        /// Gets all Permissions
        /// </summary>
        /// <remarks>
        /// Sample request all data:
        ///
        ///     GET /permissions
        /// Sample request all transaction type filter by name and slug:
        ///
        ///     GET /permissions?filters=employeeForname,name1,=,or&amp;filters=employeSurname,surname1,!=,and
        /// Sample request all transaction type paginated:
        ///
        ///     GET /permissions?page=0\&amp;pageSize=100
        /// Sample request all transaction type order by desc:
        ///
        ///     GET /permissions?orderBy=employeeForName,asc&amp;orderBy=employeeSurname,desc
        /// </remarks>
        /// <param name="filterPagination"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(List<PermissionsResponse>))]
        public async Task<IActionResult> GetAllPermissions([FromQuery] FilterPaginationQueryModel filterPagination)
        {
            try
            {
                var query = new GetAllPermissionsQuery(filterPagination.GetFilters(), filterPagination.GetOrders(),
                filterPagination.Page, filterPagination.PageSize);

                var permissions = await _queryDispatcher.SendAsync(query);
                if (permissions == null || !permissions.Any())
                {
                    return NoContent();
                }

                Log.Information("GetAllPermissions Index executed at {date}", DateTime.UtcNow);

                var count = permissions.Count;

                return Ok(new PermissionsResponse
                {
                    Permissions = permissions,
                    Message = $"Successfully returned {count} permission{(count > 1 ? "s" : string.Empty)}!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all permissions";

                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
