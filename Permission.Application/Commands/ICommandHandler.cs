using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Commands
{
    public interface ICommandHandler
    {
        Task<int> HandleAsync(RequestPermissionCommand command);
        Task<int> HandleAsync(ModifyPermissionCommand command);
    }
}
