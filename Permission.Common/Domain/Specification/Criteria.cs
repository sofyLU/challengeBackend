using System.Collections.Generic;
using System.Linq;

namespace Permission.Common.Domain.Specification
{
    public class Criteria<T> where T : class
    {
        public List<Specification<T>> Specifications { get; }
        public List<Order<T>> Orders { get; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public Criteria(List<Filter>? filters = null, List<OrderBy>? orderByList = null, int? page = null, int? pageSize = null)
        {
            filters ??= new List<Filter>();
            Specifications = GetSpecifications(filters);
            orderByList ??= new List<OrderBy>();
            Orders = GetOrders(orderByList);
            Page = page ?? 0;
            PageSize = pageSize ?? int.MaxValue;
        }
        
        private static List<Specification<T>> GetSpecifications(IEnumerable<Filter> filters)
        {
            return filters
                .Select(filter => new Specification<T>(
                        new FilterField<T>(filter.Field), filter.Operator, filter.Comparer, filter.Value)).ToList();
        }

        private static List<Order<T>> GetOrders(IEnumerable<OrderBy> orderByList)
        {
            return orderByList.Select(orderBy => new Order<T>(
                    new OrderField<T>(orderBy.Field), orderBy.OrderType)).ToList();
        }

        public void SetPage(int value)
        {
            Page = value;
        }

        public void SetPageSize(int value)
        {
            PageSize = value;
        }
    }
}