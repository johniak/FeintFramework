using System;
using System.Text;

namespace Feint.GraphQL
{
    public class SchemaBuilder
    {
        public StringBuilder SchemaStringBuilder = new StringBuilder();
        Schema Schema;
        string tab ="    ";
        public SchemaBuilder(Schema schema)
        {
            Schema = schema;
        }

        private void buildRecursive(Type type)
        {

        }

        public string Build()
        {
            SchemaStringBuilder.Append("schema{\n");
            SchemaStringBuilder.Append($"{tab}query: Query\n");
            SchemaStringBuilder.Append("}\n");
            foreach (var type in Schema.Types)
            {
                var typeInstance = (BaseType)Activator.CreateInstance(type);
                if (typeInstance.IsScalar())
                {
                    SchemaStringBuilder.Append($"scalar {typeInstance.Name}\n\n");
                }
                if (typeInstance is ObjectType)
                {
                    SchemaStringBuilder.Append($"type {typeInstance.Name}{{\n");
                    foreach (var field in ((ObjectType)typeInstance).Fields)
                    {
                        SchemaStringBuilder.Append($"{tab}{field.Key}: {field.Value.Type.Name}\n");
                    }
                    SchemaStringBuilder.Append("}\n\n");
                }
            }
            return SchemaStringBuilder.ToString();
        }
    }
}