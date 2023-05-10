using Permission.Common.DTOs;
using Permission.Domain.Entities;

namespace Permission.Api.DTOs
{
    public class PermissionsResponse : BaseResponse
    {
        public List<PermissionEntity> Permissions { get; set; }
    }
}
