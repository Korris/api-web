#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Mcsg.Common.Core.Extensions;

using SeedWork.Dtos;

/// <summary>
/// IQueryable extension for using [this IQueryableExtension] only
/// </summary>
public static class IQueryableExtension
{
    /// <summary>
    /// Get And expression
    /// </summary>
    /// <typeparam name="T">Model class type</typeparam>
    /// <param name="o">Expression</param>
    /// <param name="e">Or expression</param>
    /// <returns>Return the Or expression</returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> o, Expression<Func<T, bool>> e)
    {
        if (o == null)
        {
            return e;
        }

        Replace(e, e.Parameters[0], o.Parameters[0]);
        var t = Expression.Or(o.Body, e.Body);
        var res = Expression.Lambda<Func<T, bool>>(t, o.Parameters);

        return res;
    }

    /// <summary>
    /// Get And expression
    /// </summary>
    /// <typeparam name="T">Model class type</typeparam>
    /// <param name="o">Expression</param>
    /// <param name="e">And expression</param>
    /// <returns>Return the And expression</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> o, Expression<Func<T, bool>> e)
    {
        if (o == null)
        {
            return e;
        }

        Replace(e, e.Parameters[0], o.Parameters[0]);
        var t = Expression.And(o.Body, e.Body);
        var res = Expression.Lambda<Func<T, bool>>(t, o.Parameters);

        return res;
    }

    /// <summary>
    /// Sort query
    /// </summary>
    /// <typeparam name="T">Model class type</typeparam>
    /// <param name="o">Query</param>
    /// <param name="sorts">List field sort</param>
    /// <returns>Return the sort query</returns>
    public static IQueryable<T> Sort<T>(this IQueryable<T> o, List<SortDto> sorts)
    {
        var res = o;

        try
        {
            if (sorts == null)
            {
                sorts = new List<SortDto>();
            }

            var exp = o.Expression;
            var param = Expression.Parameter(typeof(T));

            foreach (var i in sorts)
            {
                var property = Expression.PropertyOrField(param, i.Field);
                var sort = Expression.Lambda(property, param);

                var direction = string.Empty;
                if (i.Direction != SortDto.Ascending)
                {
                    direction = ListSortDirection.Descending.ToString();
                }

                var method = sorts.IndexOf(i) == 0 ? "OrderBy" : "ThenBy";
                method += direction;

                var t1 = new[] { typeof(T), property.Type };
                var t2 = Expression.Quote(sort);
                var t3 = Expression.Call(typeof(Queryable), method, t1, exp, t2);

                res = o.Provider.CreateQuery<T>(t3);
                exp = res.Expression;
            }
        }
        catch { }

        return res;
    }

    /// <summary>
    /// Where if
    /// </summary>
    /// <typeparam name="T">Model class type</typeparam>
    /// <param name="q">Query</param>
    /// <param name="c">Condition</param>
    /// <param name="p">Predicate</param>
    /// <returns>Return the query</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> q, bool c, Expression<Func<T, bool>> p)
    {
        if (!c)
        {
            return q;
        }

        return q.Where(p);
    }

    /// <summary>
    /// Page by
    /// </summary>
    /// <typeparam name="T">Model class type</typeparam>
    /// <param name="q">Query</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <returns></returns>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> q, ushort skip, ushort take)
    {
        return q.Skip(skip).Take(take);
    }

    /// <summary>
    /// Replace object support Or and And method
    /// </summary>
    /// <param name="o">Current instance object</param>
    /// <param name="old">Old object</param>
    /// <param name="new">New object</param>
    private static void Replace(object o, object old, object @new)
    {
        var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        for (var i = o.GetType(); i != null; i = i.BaseType)
        {
            var t = i.GetFields(flag);
            foreach (var j in t)
            {
                var val = j.GetValue(o);
                if (val != null && val.GetType().Assembly == typeof(Expression).Assembly)
                {
                    if (object.ReferenceEquals(val, old))
                    {
                        j.SetValue(o, @new);
                    }
                    else
                    {
                        Replace(val, old, @new);
                    }
                }
            }
        }
    }
}
