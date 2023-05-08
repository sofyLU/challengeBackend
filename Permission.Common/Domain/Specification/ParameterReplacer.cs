using System.Linq.Expressions;

namespace Permission.Common.Domain.Specification
{
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(_parameter);
        }
    }
}