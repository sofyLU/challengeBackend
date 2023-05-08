using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Domain.Specification
{
    public class Specification<T> where T : class
    {
        public FilterField<T> FilterField { get; }
        public FilterOperator Operator { get; }
        public FilterComparer Comparer { get; }
        public object? Value { get; }

        public Specification(FilterField<T> filterField, FilterOperator @operator, FilterComparer comparer, object? value)
        {
            FilterField = filterField;
            Operator = @operator;
            Comparer = comparer;
            Value = value;
        }
    }
}
