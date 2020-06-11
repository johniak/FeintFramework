using System.Collections.Generic;

namespace Feint.GraphQL
{
    public class ObjectType : BaseType
    {
        public Dictionary<string, Field> Fields = new Dictionary<string, Field>();

        public void Field(BaseType type, string name, string source, bool required, string deprecationReason, FieldResolver resolver)
        {
            Fields.Add(name, new Field() { Type = type, Name = name, Source = source, Required = required, DeprecationReason = deprecationReason, Resolver = resolver });
        }
        
    }
}