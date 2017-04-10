using System;
using System.Collections.Generic;
using FeintApi.Serializers.Fields;


namespace FeintApi.Serializers
{
    public class ListSerializer<T> : Serializer<IEnumerable<T>>
    {
        public BaseField Child { get; protected set; }
        public ListSerializer(BaseField child, IEnumerable<T> instance = null, object data = null)
        {
            if (instance == null && data == null)
                throw new NotSupportedException("Useless serializer, please provide instance or data.");
            Child = child;
            this.Instance = instance;
            this.data = data;
            this.Many = true;
        }
        public override object ToRepresentation(object instance)
        {
            IEnumerable<T> enumerable = (IEnumerable<T>)instance;
            List<object> repChilds = new List<object>();
            foreach (var item in enumerable)
            {
                repChilds.Add(Child.ToRepresentation(item));
            }
            return repChilds;
        }
    }
}