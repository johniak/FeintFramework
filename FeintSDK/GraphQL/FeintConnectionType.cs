

using System;
using System.Linq.Expressions;
using System.Reflection;
using FeintSDK;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using FeintSDK;
using System.Linq;

namespace Feint.Graphql
{

    public class FeintConnection<Tsource>
    {
        public IQueryable<Tsource> FullQuerable { get; set; }
        public IQueryable<Tsource> PaginatedQuerable { get; set; }

        public int? Skip { get; set; }
        public int? SkipLast { get; set; }
        public int? First { get; set; }
        public int? Last { get; set; }
    }

    public class FeintConnectionType<Tsource, TGraphType> : ObjectGraphType<FeintConnection<Tsource>> where Tsource : BaseModel where TGraphType : ObjectGraphType<Tsource>
    {
        public FeintConnectionType()
        {
            Field(typeof(PageInfoType), "pageInfo", resolve: resolvePageInfo);
            Field(typeof(ListGraphType<FeintEdgeType<Tsource, TGraphType>>), "edges", resolve: resolveEdges);
        }

        public IEnumerable<FeintEdge<Tsource>> resolveEdges(ResolveFieldContext<FeintConnection<Tsource>> ctx)
        {
            var pageInfo = resolvePageInfo(ctx);
            var count = pageInfo.Skipped - 1;
            foreach (var item in ctx.Source.PaginatedQuerable)
            {
                count++;
                yield return new FeintEdge<Tsource>() { Cursor = $"arrayconnection:{count}".Base64Encode(), Node = item };
            }
        }

        public PageInfo resolvePageInfo(ResolveFieldContext<FeintConnection<Tsource>> ctx)
        {
            var totalCount = ctx.Source.FullQuerable.Count();
            var selectedCount = ctx.Source.PaginatedQuerable.Count();
            var skipped = 0;
            if (ctx.Source.Skip != null)
                skipped = ctx.Source.Skip.Value;
            if (ctx.Source.SkipLast != null)
            {
                skipped = totalCount - (ctx.Source.SkipLast == null ? 0 : ctx.Source.SkipLast.Value) - selectedCount;
            }
            var pageInfo = new PageInfo()
            {
                Count = selectedCount,
                Skipped = skipped,
                HasPreviousPage = skipped > 0,
                HasNextPage = skipped + selectedCount < totalCount
            };
            return pageInfo;
        }
    }

    public static class ObjecTypeExtension
    {
        public static FieldType FeintConnectionField<TSource, TGraphType>(this ObjectGraphType graphType,
         string name,
        Func<ResolveFieldContext<object>, IQueryable<TSource>> resolveQuerable = null,
           string description = null,
           QueryArguments arguments = null,
           string deprecationReason = null) where TSource : BaseModel where TGraphType : ObjectGraphType<TSource>
        {
            var firstArgument = new QueryArgument<IntGraphType>() { Name = "first" };
            var lastArgument = new QueryArgument<IntGraphType>() { Name = "last" };
            var afterArgument = new QueryArgument<StringGraphType>() { Name = "after" };
            var beforeArgument = new QueryArgument<StringGraphType>() { Name = "before" };
            List<QueryArgument> allArgumentList = new List<QueryArgument>();
            allArgumentList.Add(firstArgument);
            allArgumentList.Add(lastArgument);
            allArgumentList.Add(afterArgument);
            allArgumentList.Add(beforeArgument);
            if (arguments != null)
            {
                allArgumentList.AddRange(arguments);
            }
            var allArguments = new QueryArguments(allArgumentList);

            return graphType.Field<FeintConnectionType<TSource, TGraphType>>(name, description: description, arguments: arguments, deprecationReason: deprecationReason, resolve: ctx =>
             {
                 IQueryable<TSource> querable = DbBase.DbSet<TSource>();
                 if (resolveQuerable != null)
                 {
                     querable = resolveQuerable(ctx);
                 }
                 int? first = null;
                 int? last = null;
                 string after = null;
                 string before = null;
                 if (ctx.Arguments != null)
                 {
                     first = (int?)ctx.Arguments.GetValueOrDefault("first", null);
                     last = (int?)ctx.Arguments.GetValueOrDefault("last", null);
                     after = (string)ctx.Arguments.GetValueOrDefault("last", null);
                     before = (string)ctx.Arguments.GetValueOrDefault("first", null);
                 }
                 int? skip = null;
                 int? skipLast = null;
                 if (after != null)
                 {
                     skip = int.Parse(after.Base64Decode().Split(':')[1]);
                 }
                 if (before != null)
                 {
                     skipLast = int.Parse(before.Base64Decode().Split(':')[1]);
                 }
                 if (first != null && last != null)
                 {
                     throw new ExecutionError("You cannot use both first and last argument");
                 }
                 var paginatedQuerable = querable;
                 if (first != null)
                     paginatedQuerable = paginatedQuerable.Take(first.Value);
                 if (last != null)
                     paginatedQuerable = paginatedQuerable.TakeLast(last.Value);
                 if (after != null)
                     paginatedQuerable = paginatedQuerable.Skip(skip.Value);
                 if (after != null)
                     paginatedQuerable = paginatedQuerable.Skip(skipLast.Value);
                 return new FeintConnection<TSource>()
                 {
                     FullQuerable = querable,
                     PaginatedQuerable = paginatedQuerable,
                     First = first,
                     Last = last,
                     Skip = skip,
                     SkipLast = skipLast
                 };
             });
        }
    }
}