using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Permission.Common.Domain.Entities;
using Permission.Common.Domain.Interfaces.Services;
using Permission.Common.Domain.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Permission.Common.Infrastructure.EntityFramework
{
    public class EntityFrameworkBuilder<T> : IEntityFrameworkBuilder<T> where T : BaseEntity
    {
        public EntityFrameworkBuilder() { }

        #region Select Order

        /// <summary>
        ///     Creates lambda expression dynamically based on order expression
        /// </summary>
        private IOrderedQueryable<T> ApplyOrder(IQueryable<T> source, string propertyName, string methodName)
        {
            try
            {
                var entity = Expression.Parameter(typeof(T));
                var property = Expression.Property(entity, propertyName);

                var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.Type);
                var lambda = Expression.Lambda(delegateType, property, entity);

                var result = typeof(Queryable).GetMethods().Single(
                        method => method.Name == methodName
                                  && method.IsGenericMethodDefinition
                                  && method.GetGenericArguments().Length == 2
                                  && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type)
                    .Invoke(null, new object[] { source, lambda });

                return (IOrderedQueryable<T>)result!;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was an error selecting order for {PropertyName}",
                    propertyName);

                throw new Exception("There was en error ordering information with " +
                                    $"propertyName {propertyName}",
                    exception);
            }
        }

        #endregion

        #region OrderEnumerator

        private static class OrderByType
        {
            internal static string OrderBy => "OrderBy";
            internal static string OrderByDescending => "OrderByDescending";
            internal static string ThenBy => "ThenBy";
            internal static string ThenByDescending => "ThenByDescending";
        }

        #endregion

        #region Constructor & properties

        private readonly ILogger<EntityFrameworkBuilder<T>> _logger;

        public EntityFrameworkBuilder(ILogger<EntityFrameworkBuilder<T>> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Builds dynamically where ef expression based on a list of specifications
        /// </summary>
        // Based on: https://stackoverflow.com/questions/8315819/expression-lambda-and-query-generation-at-runtime-simplest-where-example
        public Expression<Func<T, bool>>? GetWhereExpression(List<Specification<T>> specifications)
        {
            if (!specifications.Any())
            {
                return null;
            }

            Expression<Func<T, bool>>? result = null;

            foreach (var specification in specifications)
            {
                // For first specification
                if (result == null)
                {
                    result = SelectOperator(specification);
                    continue;
                }

                // For more than one specification, apply && or || based on comparer
                result = specification.Comparer.Equals(FilterComparer.And)
                    ? And(result, SelectOperator(specification))
                    : Or(result, SelectOperator(specification));
            }

            return result;
        }

        // https://stackoverflow.com/questions/41244/dynamic-linq-orderby-on-ienumerablet-iqueryablet
        public async Task<List<T>> ToListOrderedPagedValues(IQueryable<T> query, Criteria<T> criteria)
        {
            var orders = criteria.Orders;
            var page = criteria.Page;
            var pageSize = criteria.PageSize;

            // Do not order by asc or desc if there is no order parameters
            if (!orders.Any())
            {
                return await query
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            IQueryable<T>? result = null;

            foreach (var order in orders)
            {
                // For first order
                if (result == null)
                {
                    result = ApplyOrder(query, order.OrderField.FieldName, order.OrderType.Equals(OrderType.Asc)
                        ? OrderByType.OrderBy
                        : OrderByType.OrderByDescending);

                    continue;
                }

                result = ApplyOrder(result, order.OrderField.FieldName, order.OrderType.Equals(OrderType.Asc)
                    ? OrderByType.ThenBy
                    : OrderByType.ThenByDescending);
            }

            if (result != null)
            {
                return await result
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return new List<T>();
        }

        #endregion

        #region Select Operator

        /// <summary>
        ///     Build lambda expression based on specification
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>> SelectOperator(Specification<T> specification)
        {
            try
            {
                return GetOperationExpression(specification.Value, specification.FilterField.Name,
                    specification.Operator.OperatorExpression, specification.Operator.Name,
                    specification.Operator.IsNegative);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "There was an error selecting operator for {@Specification}",
                    specification);

                throw new Exception("There was en error filtering information with " +
                                    $"filter {specification.FilterField.Name}",
                    exception);
            }
        }

        /// <summary>
        ///     Creates lambda expression dynamically based on operator expression
        /// </summary>
        private static Expression<Func<T, bool>> GetOperationExpression(object? value, string propertyName,
            Func<Expression, Expression, BinaryExpression> operatorExpression, string methodName,
            bool isNegative)
        {
            //Set lambda expression parameters
            SetExpressionParameters(value, propertyName, out var entity, out var property, out var constantValue);

            //Set operator
            var lambda = GetMethodExpression(operatorExpression, methodName, entity, property, constantValue, value);

            if (isNegative)
            {
                lambda = Expression.Lambda<Func<T, bool>>(Expression.Not(lambda.Body), entity);
            }

            return lambda;
        }

        /// <summary>
        ///     Returns lambda expression based on method definition
        /// </summary>
        private static Expression<Func<T, bool>> GetMethodExpression(
            Func<Expression, Expression, BinaryExpression>? operatorExpression,
            string methodName, ParameterExpression entity, MemberExpression property,
            ConstantExpression constantValue, object? value)
        {
            Expression<Func<T, bool>> lambda = s => s.Equals("");

            //For methods without an Expression, search for method
            if (operatorExpression == null)
            {
                MethodInfo? method;
                MethodCallExpression callExpression;

                if (methodName.Contains("Contains"))
                {
                    methodName = "Contains";
                }

                if (value is IList)
                {
                    var listType = typeof(List<>).MakeGenericType(property.Type);
                    method = listType.GetMethod(methodName, new[] { property.Type });
                    if (method != null)
                    {
                        callExpression = Expression.Call(constantValue, method, property);
                        lambda = Expression.Lambda<Func<T, bool>>(callExpression, entity);
                    }
                }
                else if (value is string valueStr)
                {
                    if (methodName.Equals("EqualString"))
                    {
                        method = typeof(DbFunctionsExtensions).GetMethod("Like",
                            new[] { typeof(DbFunctions), typeof(string), typeof(string) });
                        if (method != null)
                        {
                            callExpression = Expression.Call(
                                null,
                                method,
                                Expression.Constant(EF.Functions),
                                Expression.Call(property, "ToUpper", null),
                                Expression.Constant(valueStr.ToUpper()));
                            lambda = Expression.Lambda<Func<T, bool>>(callExpression, entity);
                        }
                    }
                    else
                    {
                        var callExpressionToUpper = Expression.Call(property, "ToUpper", null);
                        callExpression = Expression.Call(callExpressionToUpper, methodName, null,
                            Expression.Constant(valueStr.ToUpper()));
                        lambda = Expression.Lambda<Func<T, bool>>(callExpression, entity);
                    }
                }
                else
                {
                    method = property.Type.GetMethod(methodName, new[] { property.Type });
                    if (method != null)
                    {
                        callExpression = Expression.Call(property, method, constantValue);
                        lambda = Expression.Lambda<Func<T, bool>>(callExpression, entity);
                    }
                }
            }
            //Use expression
            else
            {
                if (constantValue.Value == null)
                {
                    var @operator = operatorExpression(property, Expression.Constant(null));
                    lambda = Expression.Lambda<Func<T, bool>>(@operator, entity);
                }
                else if (constantValue.Value.ToString()!.ToUpper().Equals("NULL"))
                {
                    var @operator = operatorExpression(property, Expression.Constant(null));
                    lambda = Expression.Lambda<Func<T, bool>>(@operator, entity);
                }
                else
                {
                    var @operator = operatorExpression(property, constantValue);
                    lambda = Expression.Lambda<Func<T, bool>>(@operator, entity);
                }
            }

            return lambda;
        }

        #endregion

        #region Other methods

        /// <summary>
        ///     Build lambda expression parameters
        /// </summary>
        private static void SetExpressionParameters(object? value, string propertyName,
            out ParameterExpression entity, out MemberExpression property,
            out ConstantExpression constantValue)
        {
            entity = Expression.Parameter(typeof(T));
            property = Expression.Property(entity, propertyName);

            if (value is IList)
            {
                var listType = typeof(List<>).MakeGenericType(property.Type);
                constantValue = Expression.Constant(value, listType);
            }
            else
            {
                value = CastValue(value, property.Type);
                constantValue = Expression.Constant(value, property.Type);
            }
        }

        /// <summary>
        ///     Cast values based on a type
        /// </summary>
        private static object? CastValue(object? value, Type type)
        {
            if (value == null)
            {
                return null;
            }

            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                var parsed = Guid.TryParse(value.ToString(), out var output);
                return parsed ? output : null;
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return value;
            }

            return Convert.ChangeType(value, type);
        }

        #endregion

        #region Compare methods

        /// <summary>
        ///     Joins two expression using && comparer
        /// </summary>
        private static Expression<Func<T, bool>> And(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.AndAlso(left.Body, right.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

            return finalExpr;
        }

        /// <summary>
        ///     Joins two expression using && comparer
        /// </summary>
        private static Expression<Func<T, bool>> Or(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.OrElse(left.Body, right.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

            return finalExpr;
        }

        #endregion
    }
}
