using System;
using System.Collections.Generic;
using FeintApi.Serializers.Fields;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using static FeintApi.Helpers;

namespace FeintApi.Serializers
{
    public class Serializer<T> : ISerializer where T : class
    {
        public Dictionary<string, object> Data
        {
            get
            {
                return getDataFromFields();
            }
            protected set
            {
                data = value;
            }
        }
        public string Json { get { return objectToJson(Data); } }
        Dictionary<string, object> data;
        protected Dictionary<string, object> context = new Dictionary<string, object>();
        protected T instance;
        protected bool many;
        protected bool isValidated = false;



        public Serializer(
          Dictionary<string, object> data = null,
          T instance = null,
          Dictionary<string, object> context = null,
          bool many = false)
        {
            this.Data = data;
            this.instance = instance;
            if (context != null)
                this.context = context;
            this.many = many;
        }

        private Serializer() { }

        protected IEnumerable<IField> fields
        {
            get
            {
                var members = getFieldsMember();
                return members.Select(mi => createFieldObject(mi));
            }
        }

        Dictionary<string, object> getDataFromFields()
        {
            if (data != null && isValidated)
                throw new NotSupportedException("Data is not validated");
            var dict = new Dictionary<string, object>();
            foreach (dynamic field in this.fields)
            {
                dict[field.Name] = field.ExternalValue;
            }
            return dict;
        }

        protected IField createFieldObject(MemberInfo info)
        {
            Type type;
            if (info.MemberType == MemberTypes.Property)
                type = ((PropertyInfo)info).PropertyType;
            else
                type = ((FieldInfo)info).FieldType;
            if (instance != null && data != null)
                return (IField)Activator.CreateInstance(type, info.Name, getFieldByName(instance, info.Name), data);
            if (data != null)
                return (IField)Activator.CreateInstance(type, info.Name, data);
            if (instance != null)
                return (IField)Activator.CreateInstance(type, info.Name, getFieldByName(instance, info.Name));
            return null;
        }

        protected IEnumerable<MemberInfo> getFieldsMember()
        {
            Type thisType = this.GetType();
            var fields = thisType.GetFields().ToList();//.Where(mi => typeof(IField).IsAssignableFrom(mi.FieldType)).Select(x => (MemberInfo)x);
            var properties = thisType.GetProperties().Where(mi => typeof(IField).IsAssignableFrom(mi.PropertyType)).Select(x => (MemberInfo)x);
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

    }
}

interface ISerializer { };