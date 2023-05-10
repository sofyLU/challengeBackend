using Moq;
using Permission.Application.Commands;
using Permission.Application.Queries;
using Permission.Common.Domain.Specification;
using Permission.Domain.Entities;
using Permission.Domain.Interfaces.Repositories;
using Permission.Domain.SeedWork;
using Permission.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Permission.Tests.Application.Query
{
    public class GetAllPermissionsQueryCommandHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepository;
        private readonly Mock<IUnitOfWork> _dbContext;

        private readonly QueryHandler _handler;

        public GetAllPermissionsQueryCommandHandlerTests()
        {
            _permissionRepository = new Mock<IPermissionRepository>();
            _dbContext = new Mock<IUnitOfWork>();

            _handler = new QueryHandler(_permissionRepository.Object);
        }

        [Fact]
        public async Task Handle_ExistingPermissions()
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

            _permissionRepository.Setup(x => x.GetAll(It.IsAny<Criteria<PermissionEntity>>()))
                .ReturnsAsync(new List<PermissionEntity>
                {
                    dbPermission.Object,
                });

            var query = new GetAllPermissionsQuery(new List<Filter>(),new List<OrderBy>(),0,10);

            var expected = new PermissionEntity
            {
                Id = id,
                EmployeeForename = employeeForName,
                EmployeeSurname = employeeSurname,
                PermissionDate = permissionDate,
                PermissionType = dbPermissionType.Object
            };

            //Act
            var actual = await _handler.HandleAsync(query);

            //Assert
            Assert.Equal(expected.Id, actual.First().Id);
            Assert.Equal(expected.EmployeeForename, actual.First().EmployeeForename);
            Assert.Equal(expected.EmployeeSurname, actual.First().EmployeeSurname);

        }
    }
}
