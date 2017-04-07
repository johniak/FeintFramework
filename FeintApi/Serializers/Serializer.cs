using System;
using System.Collections.Generic;
using FeintApi.Serializers.Fields;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using static FeintApi.Helpers;

namespace FeintApi.Serializers
{

    public class BaseSerializer
    {

    }

    public class Serializer<T> : BaseField where T : class
    {

        protected object data;
        public object Data { get { return Instance == null ? data : ToRepresentation(Instance); } }
        public object Instance { get; protected set; }

        public string Json { get { return objectToJson(Data); } }
        public bool Many { get; protected set; }
        public Serializer()
        {

        }
        public Serializer(object instance = null, object data = null, bool many = false) : base()
        {
            if (instance == null && data == null)
                throw new NotSupportedException("Useless serializer, please provide instacne or data.");
            if (many && !(instance is IEnumerable<T>))
            {
                throw new NotSupportedException($"The instance should be IEnumerable of {typeof(T).Name}");
            }
            if (!many && !(instance is T))
                throw new NotSupportedException($"The instance should be IEnumerable of {typeof(T).Name}");
            this.data = data;
            Instance = instance;
            Many = many;
        }
        protected IEnumerable<BaseField> fields;
        public IEnumerable<BaseField> Fields
        {
            get
            {
                if (fields == null)
                    fields = getFieldsMember().Select(mi => createFieldObject(mi)).ToList();
                return fields;
            }
        }

        protected BaseField createFieldObject(MemberInfo info)
        {
            Type type;
            if (info.MemberType == MemberTypes.Property)
                type = ((PropertyInfo)info).PropertyType;
            else
                type = ((FieldInfo)info).FieldType;
            var field = (BaseField)Activator.CreateInstance(type);
            field.Bind(info.Name, this);
            return field;
        }

        protected IEnumerable<MemberInfo> getFieldsMember()
        {
            Type thisType = this.GetType();
            var fields = thisType.GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                                .Where(mi => typeof(BaseField).IsAssignableFrom(mi.FieldType)).Select(x => (MemberInfo)x);
            var properties = thisType.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                                    .Where(mi => typeof(BaseField).IsAssignableFrom(mi.PropertyType)).Select(x => (MemberInfo)x);
            var memberInfos = fields.Concat(properties).ToList();
            return memberInfos;
        }

        protected object getFieldByName(Object instance, string name)
        {
            var type = instance.GetType();
            var members = type.GetMember(name);
            if (members.Length < 1)
                throw new NotSupportedException($"There is no {name} field inside {instance}");
            var memberInfo = members[0];
            if (memberInfo.MemberType == MemberTypes.Field)
                return ((FieldInfo)memberInfo).GetValue(instance);

            if (memberInfo.MemberType == MemberTypes.Property)
                return ((PropertyInfo)memberInfo).GetValue(instance);
            return null;
        }

        protected void setFieldByName(Object instance, string name, object value)
        {
            var type = instance.GetType();
            var memberInfo = type.GetMember(name)[0];
            if (memberInfo.MemberType == MemberTypes.Field)
                ((FieldInfo)memberInfo).SetValue(instance, value);
            else if (memberInfo.MemberType == MemberTypes.Property)
                ((PropertyInfo)memberInfo).SetValue(instance, value);
        }

        public override object ToRepresentation(object instance)
        {
            if (instance is IEnumerable<T>)
                return new ListSerializer<T>(this, (IEnumerable<T>)instance).Data;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var field in Fields)
            {
                dict.Add(field.Name, field.ToRepresentation(getFieldByName(instance, field.Name)));
            }
            return dict;
        }

        public override object ToInternalValue(object data)
        {
            throw new NotImplementedException();
        }
    }

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
