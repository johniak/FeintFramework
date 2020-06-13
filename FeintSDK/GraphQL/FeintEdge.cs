

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

    public class FeintEdge<Tsource>
    {
        public String Cursor { get; set; }
        public Tsource Node { get; set; }
    }
    public class FeintEdgeType<Tsource, TGraphType> : ObjectGraphType<FeintEdge<Tsource>>  where Tsource : BaseModel where TGraphType : ObjectGraphType<Tsource>
    {
        public FeintEdgeType()
        {
            Field(edge => edge.Cursor);
            Field(typeof(TGraphType), "node", resolve: ctx => ctx.Source.Node);
        }
    }
}