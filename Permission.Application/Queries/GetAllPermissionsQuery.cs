using CQRS.Core.Queries;
using Permission.Common.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Application.Queries
{
    public class GetAllPermissionsQuery : BaseQuery
    {
        public List<Filter> Filters { get; set; }
        public List<OrderBy> OrderBy { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public GetAllPermissionsQuery(List<Filter> filters, List<OrderBy> orderBy, int? page, int? pageSize)
        {
            Filters = filters;
            OrderBy = orderBy;
            Page = page;
            PageSize = pageSize;
        }
    }
}
