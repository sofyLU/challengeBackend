using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Commands
{
    public interface ICommandHandler
    {
        Task HandleAsync(RequestPermissionCommand command);
        Task HandleAsync(ModifyPermissionCommand command);
    }
}
