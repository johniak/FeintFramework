
using System;
using System.Linq.Expressions;
using System.Reflection;
using FeintSDK;
using GraphQL.Types;
using GraphQL.Resolvers;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Builders;

namespace Feint.Graphql
{
    public abstract class FeintMutation<TGraphType> where TGraphType : GraphType
    {
        public QueryArguments Arguments;

        public String Name;

        public abstract object Mutate(ResolveFieldContext<object> ctx);
    
        public FieldType FieldType
        {
            get
            {

                FieldType fieldType = new FieldType()
                {
                    Arguments = this.Arguments,
                    Name = Name,
                    Type = typeof(TGraphType),
                    Resolver = new FuncFieldResolver<object, object>(this.Mutate)
                };
                return fieldType;
            }
        }
    }

}