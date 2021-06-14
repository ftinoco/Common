using NetCoreCommon.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetCoreCommon.Helpers
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Build lambda function to filter records
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="parameters">Parameter to filter</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> Filter<TEntity>(LazyPageParameters parameters)
        {
            MethodInfo containsMethod = typeof(string).GetMethods().Where(x => x.Name == "Contains").FirstOrDefault();
            MethodInfo endsWithMethod = typeof(string).GetMethods().Where(x => x.Name == "EndsWith").FirstOrDefault();    
            MethodInfo startsWithMethod = typeof(string).GetMethods().Where(x => x.Name == "StartsWith").FirstOrDefault();

            var predicate = PredicateBuilder.True<TEntity>();
            if (parameters != null && parameters?.Filters != null)
            {
                foreach (var item in parameters.Filters)
                {
                    if (!item.Key.Equals("globalFilter"))
                    {
                        var param = Expression.Parameter(typeof(TEntity));
                        var member = Expression.Property(param, item.Key);

                        if (!GetConstant(member, item, out UnaryExpression constant)) continue;

                        switch (item.Value.MatchMode)
                        {
                            case "equals":
                                var condition =
                                    Expression.Lambda<Func<TEntity, bool>>(
                                        Expression.Equal(member, constant), param
                                    );//.Compile(); // for LINQ to SQl/Entities skip Compile() call
                                predicate = predicate.And(condition);
                                break;
                            default:
                                var call = Expression.Call(member,
                                    item.Value.MatchMode.Equals("contains") ? containsMethod :
                                    (item.Value.MatchMode.Equals("startsWith") ? startsWithMethod : endsWithMethod),
                                    constant);
                                predicate = predicate.And(Expression.Lambda<Func<TEntity, bool>>(call, param));

                                break;
                        }
                    }
                }
            }
            return predicate;
        }

        /// <summary>
        /// Get expression from member and filter options
        /// </summary>
        /// <param name="member">Member expresion</param>
        /// <param name="filter">Filter option</param>
        /// <param name="expression">Parameter by ref, unary expresion</param>
        /// <returns></returns>
        private static bool GetConstant(MemberExpression member, KeyValuePair<string, LazyPageFilter> filter, out UnaryExpression expression)
        {
            expression = null;
            try
            {
                var propertyType = ((PropertyInfo)member.Member).PropertyType;
                var converter = TypeDescriptor.GetConverter(propertyType); // 1

                if (!converter.CanConvertFrom(typeof(string))) // 2
                    throw new NotSupportedException();

                var propertyValue = converter.ConvertFromInvariantString(filter.Value.Value); // 3
                var constant = Expression.Constant(propertyValue);
                expression = Expression.Convert(constant, propertyType); // 4
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
