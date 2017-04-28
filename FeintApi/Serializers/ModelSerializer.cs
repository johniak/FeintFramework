using System;
using System.Collections.Generic;
using FeintApi.Serializers.Fields;
using System.Reflection;
using System.Linq;
using static FeintApi.Helpers;
using FeintSDK;

namespace FeintApi.Serializers
{
    public class ModelSerializer<T> : Serializer<T> where T : BaseModel
    {
        public ModelSerializer(object instance = null, object data = null, bool many = false) : base(instance, data, many)
        {

        }
        public string[] ModelFields = new string[0];
        protected override IEnumerable<BaseField> getFields()
        {
            var baseFields = base.getFields();
            var modelFields = this.getFieldsFromModel();
            return baseFields.Concat(modelFields);
        }

        protected virtual IEnumerable<BaseField> getFieldsFromModel()
        {
            return getFieldsFromModelType(typeof(T));
        }
        protected virtual BaseField createFieldOfType(Type t, string name)
        {
            Type fieldType = typeof(Field<>);
            fieldType = fieldType.MakeGenericType(t);
            var field = (BaseField)Activator.CreateInstance(fieldType);
            field.Bind(name, this);
            return field;
        }
        protected virtual IEnumerable<BaseField> getFieldsFromModelType(Type modelType)
        {
            return ModelFields.Select(name => createFieldOfType(getTypeOfFieldOrProperty(modelType, name), name));
        }

        public override object Create(Dictionary<string, object> validatedDate)
        {
            var instance = (T)Activator.CreateInstance<T>();
            foreach (var field in validatedDate)
            {
                setFieldByName(instance, field.Key, field.Value);
            }
            instance.Save();
            return instance;
        }
        public override object Update(Dictionary<string, object> validatedDate, object obj)
        {
            var instance = (T)obj;
            foreach (var field in validatedDate)
            {
                setFieldByName(instance, field.Key, field.Value);
            }
            instance.Save();
            return instance;
        }
    }
}