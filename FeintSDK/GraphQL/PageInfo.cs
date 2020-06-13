

using System;
using System.Linq.Expressions;
using System.Reflection;
using FeintSDK;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using FeintSDK;

namespace Feint.Graphql
{
    public class PageInfo
    {
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Skipped { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public String StartCursor
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return $"arrayconnection:{Skipped}".Base64Encode();
            }
        }
        public String EndCursor
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return $"arrayconnection:{Skipped + Count - 1}".Base64Encode();
            }
        }
    }
    public class PageInfoType : ObjectGraphType<PageInfo>
    {
        public PageInfoType()
        {
            Field(pi => pi.HasNextPage);
            Field(pi => pi.HasPreviousPage);
            Field(pi => pi.StartCursor);
            Field(pi => pi.EndCursor);
            Field(pi => pi.TotalCount);
        }
    }
}