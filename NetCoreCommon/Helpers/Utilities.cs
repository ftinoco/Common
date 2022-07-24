using NetCoreCommon.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NetCoreCommon.Helpers
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Get lazy pagination options either from json or base64 string
        /// </summary>
        /// <param name="paginationOpts">String to process</param>
        /// <param name="isBase64">Flag that indicate if the string is base64, otherwise json fomat</param>
        /// <returns>Lazy pagination options object</returns>
        public static LazyPageOptions GetPaginationOptionsFromString(string paginationOpts, bool isBase64 = true)
        {
            string strJSON;
            if (isBase64)
            {
                var base64EncodedBytes = Convert.FromBase64String(paginationOpts);
                strJSON = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            else
                strJSON = paginationOpts;

            try
            {
                return JsonSerializer.Deserialize<LazyPageOptions>(strJSON, new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                });
            }
            catch (Exception)
            {
                return new LazyPageOptions();
            }
        }

        /// <summary>
        /// Build lambda function to filter records
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="parameters">Parameter to filter</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(LazyPageOptions parameters)
        {
            var predicate = PredicateBuilder.True<TEntity>();
            if (parameters != null && parameters.Filter != null)
            {
                Dictionary<string, LazyPageFilter> filters = new();
                GetFilters(parameters.Filter, ref filters);

                MethodInfo containsMethod = typeof(string).GetMethods().Where(x => x.Name == "Contains").FirstOrDefault();
                MethodInfo endsWithMethod = typeof(string).GetMethods().Where(x => x.Name == "EndsWith").FirstOrDefault();
                MethodInfo startsWithMethod = typeof(string).GetMethods().Where(x => x.Name == "StartsWith").FirstOrDefault();

                foreach (var item in filters)
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
                                );
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

        /// <summary>
        /// Get filters
        /// </summary>
        /// <param name="parameters">List of dynamic objects</param>
        /// <param name="filters">Filters to set</param>
        private static void GetFilters(System.Collections.IList parameters, ref Dictionary<string, LazyPageFilter> filters)
        {
            int index = 0;
            bool oneFilter = false;
            foreach (JsonElement item in parameters)
            {
                if (oneFilter) break;

                if (index == 0 && item.ValueKind == JsonValueKind.String)
                    oneFilter = true;

                if (item.ValueKind == JsonValueKind.Array)
                {
                    var subList = JsonSerializer.Deserialize<object[]>(item.GetRawText());
                    GetFilters(subList, ref filters);
                }
                else if (item.ValueKind == JsonValueKind.String)
                {
                    if (!item.ToString().Equals("and") && !item.ToString().Equals("or"))
                    {
                        if (oneFilter)
                        {
                            string matchMode = parameters[1].ToString();
                            filters.Add(item.ToString(), new LazyPageFilter()
                            {
                                MatchMode = matchMode == "=" ? "equals" : matchMode,
                                Value = parameters[2].ToString()
                            });
                        }
                    }
                }
                index++;
            }
        }
    }
}
