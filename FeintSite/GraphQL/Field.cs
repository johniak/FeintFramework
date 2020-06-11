using System;

namespace Feint.GraphQL{

    public delegate dynamic FieldResolver(dynamic parent);
    public class Field {
        public BaseType Type;
        public string Name;
        public string Source;
        public bool Required;
        public string DeprecationReason;
        public FieldResolver Resolver;
    }
}