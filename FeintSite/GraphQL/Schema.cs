using System;
using System.Collections.Generic;

namespace Feint.GraphQL
{
    public class Schema
    {
        public ObjectType Query { get; set; }
        public ObjectType Mutation { get; set; }

        public HashSet<Type> Types { get => types; }
        private HashSet<Type> types = new HashSet<Type>();

        public void BuildTypes()
        {
            AnalyzeQuery(Query);
        }


        void AnalyzeQuery(BaseType type)
        {
            var currentType = type.GetType();
            Types.Add(currentType);
            if (type.IsScalar())
            {
                return;
            }
            var objType = (ObjectType)type;

            foreach (var field in objType.Fields)
            {
                AnalyzeQuery(field.Value.Type);
            }
        }
    }
}