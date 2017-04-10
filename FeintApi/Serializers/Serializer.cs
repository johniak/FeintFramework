using System;
using System.Collections.Generic;
using FeintApi.Serializers.Fields;
using System.Reflection;
using System.Linq;
using static FeintApi.Helpers;
using FeintSDK;

namespace FeintApi.Serializers
{
    public class Serializer<T> : BaseField, IWritable where T : class
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
            if (many && !(instance is IEnumerable<T>) && instance != null)
            {
                throw new NotSupportedException($"The instance should be IEnumerable of {typeof(T).Name}");
            }
            if (!many && !(instance is T) && instance != null)
                throw new NotSupportedException($"The instance should be {typeof(T).Name}");
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
                    fields = getFields();
                return fields;
            }
        }

        protected virtual IEnumerable<BaseField> getFields()
        {
            return getFieldsMember().Select(mi => createFieldObject(mi)).ToList();
        }

        protected virtual BaseField createFieldObject(MemberInfo info)
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
        /// Transform dict of internal data into the dict of native data
        public override object ToInternalValue(object data)
        {
            Dictionary<string, object> fieldDict = (Dictionary<string, object>)data;
            Dictionary<string, object> serializerDict = new Dictionary<string, object>();
            List<ValidationException> validationExceptions = new List<ValidationException>();
            foreach (var field in Fields)
            {
                try
                {
                    serializerDict[field.Name] = field.Validate(fieldDict);
                }
                catch (ValidationException ex)
                {
                    validationExceptions.Add(new ValidationException(ex.Exceptions));
                }
            }
            if (validationExceptions.Count > 0)
            {
                throw new ValidationException(validationExceptions);
            }
            return serializerDict;
        }

        public override object Validate(object data)
        {
            return ToInternalValue(data);
        }

        public virtual object Create(Dictionary<string, object> validatedDate)
        {
            throw new NotImplementedException();
        }

        public virtual object Update(Dictionary<string, object> validatedDate, object Instance)
        {
            throw new NotImplementedException();
        }

        public virtual object Save()
        {
            var objectData = Validate(this.Data);
            if (objectData is Dictionary<string, object>)
                throw new NotSupportedException("You have to overridde save method to create from other type than dictionary");
            var validatedData = (Dictionary<string, object>)Validate(this.Data);
            object instance = null;
            if (Instance == null)
            {
                instance = Create(validatedData);
            }
            else
            {
                instance = Update(validatedData, Instance);
            }
            Instance = instance;
            return instance;
        }

        public virtual void delete()
        {
            throw new NotImplementedException();
        }
    }
}
