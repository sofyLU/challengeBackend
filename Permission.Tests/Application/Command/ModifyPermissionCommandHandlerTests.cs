using CQRS.Core.Handlers;
using Moq;
using Permission.Application.Commands;
using Permission.Common.Domain.Specification;
using Permission.Domain.Aggregates;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using Permission.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Permission.Tests.Application.Command
{
    public class ModifyPermissionCommandHandlerTests
    {
        private readonly Mock<IEventSourcingHandler<PermissionAggregate>> _eventSourcing;
        private readonly Mock<IPermissionRepository> _permissionRepository;
        private readonly Mock<IPermissionTypeRepository> _permissionTypeRepository;

        private readonly Mock<IUnitOfWork> _dbContext;

        private readonly CommandHandler _handler;

        public ModifyPermissionCommandHandlerTests()
        {
            _eventSourcing = new Mock<IEventSourcingHandler<PermissionAggregate>>();
            _permissionRepository = new Mock<IPermissionRepository>();
            _permissionTypeRepository = new Mock<IPermissionTypeRepository>();

            _dbContext = new Mock<IUnitOfWork>();

            _handler = new CommandHandler(_eventSourcing.Object, _permissionRepository.Object, _permissionTypeRepository.Object);
        }

        [Fact]
        public async Task Handle_NewRequestPermission()
        {
            //Arrange
            var id = 1;
            var permissionTypeId = 1;
            var employeeForName = "test";
            var employeeSurname = "test";
            var permissionDate = DateTime.Now;

            _permissionRepository.Setup(x => x.UnitOfWork)
                .Returns(_dbContext.Object);

            var dbPermission = new Mock<PermissionEntity>();
            dbPermission.Object.Id = id;
            dbPermission.Object.PermissionDate = permissionDate;
            dbPermission.Object.EmployeeForename = employeeForName;
            dbPermission.Object.EmployeeSurname = employeeSurname;

            var dbPermissionType = new Mock<PermissionTypeEntity>();
            dbPermissionType.Object.Id = permissionTypeId;
            dbPermissionType.Object.Description = "test";

            dbPermission.Object.PermissionType = dbPermissionType.Object;

            dbPermissionType.Object.Permissions = new List<PermissionEntity> { dbPermission.Object };

            _permissionTypeRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(dbPermissionType.Object);

            _permissionRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(dbPermission.Object);

            var permissionAggregate = new Mock<PermissionAggregate>();
            permissionAggregate.Object._active = true;

            _eventSourcing.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(permissionAggregate.Object);

            var command = new ModifyPermissionCommand 
            { 
                Id = id,
                EmployeeName = employeeForName, 
                EmployeeSurName = employeeSurname,
                PermissionTypeId = permissionTypeId
            };

            var expected = new PermissionEntity
            {
                EmployeeForename = employeeForName,
                EmployeeSurname = employeeSurname,
                PermissionDate = permissionDate,
                PermissionType = dbPermissionType.Object
            };

            //Act
            var actual = await _handler.HandleAsync(command);

            //Assert
            _permissionRepository.Verify(x => x.Update(
                It.IsAny<PermissionEntity>()));
        }
    }
}
