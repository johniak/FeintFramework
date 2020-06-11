

using System;
using System.Linq.Expressions;
using System.Reflection;
using FeintSDK;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;
using GraphQL;

namespace Feint.Graphql
{
    public class FeintObjectType<TSourceType> : ObjectGraphType<TSourceType> where TSourceType : BaseModel
    {
        public FeintObjectType(Expression<Func<TSourceType, dynamic>>[] excludedFields)
        {
            var excludedNames = new List<String>();
            excludedNames.Add("Id");
            foreach (var excludedExpression in excludedFields)
            {
                var unary = (UnaryExpression)excludedExpression.Body;
                var member = (MemberExpression)unary.Operand;
                excludedNames.Add(member.Member.Name);
            }
            var mainType = typeof(TSourceType);
            var properties = mainType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Field(h => h.Id, nullable: true);
            foreach (var property in properties)
            {
                if (excludedNames.Contains(property.Name))
                {
                    continue;
                }

                Field(GetCorrectType(property.PropertyType), property.Name);
            }
        }
        public FeintObjectType():this(new Expression<Func<TSourceType, dynamic>>[0])
        {
            
        }
        protected Type GetCorrectType(Type type)
        {
            if (type == typeof(String))
            {
                return typeof(StringGraphType);
            }
            if (type == typeof(int))
            {
                return typeof(IntGraphType);
            }
            if (type == typeof(float)) 
            {
                return typeof(FloatGraphType);
            }
            if (type == typeof(Boolean))
            {
                return typeof(BooleanGraphType);
            }
            if (type == typeof(DateTime))
            {
                return typeof(DateTimeGraphType);
            }
            return type;
        }
    }
}