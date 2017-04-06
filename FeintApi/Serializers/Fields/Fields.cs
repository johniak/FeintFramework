using System;
using System.Collections.Generic;
using System.Reflection;

namespace FeintApi.Serializers.Fields
{
    public class Field<T> : IField
    {
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
        public string Source { get { return source ?? Name; } set { source = value; } }

        protected string source;
        public bool Many { get; set; }
        Dictionary<string, object> data;
        public T ExternalValue { get; protected set; }
        public object InternalValue { get; protected set; }
        public string Errors { get; protected set; }


        public Field(string name, T value)
        {
            this.ExternalValue = value;
            this.Name = name;
        }
        public Field(string name, Dictionary<String, object> data)
        {
            this.data = data;
            this.Name = name;
        }
        public Field(string name, T value, Dictionary<String, object> data)
        {
            this.InternalValue = value;
            this.ExternalValue = value;
            this.data = data;
            this.Name = name;
        }
        public Field(string name, Object value)
        {
            this.InternalValue = value;
            this.ExternalValue = convertToExternalValue(value);
            this.Name = name;
        }

        public Field(string name, Object value, Dictionary<String, object> data)
        {
            this.InternalValue = value;
            this.ExternalValue = convertToExternalValue(value);
            this.data = data;
            this.Name = name;
        }

        protected T convertToExternalValue(object internalValue)
        {
            if (!(internalValue is T))
                throw new NotImplementedException();
            return (T)internalValue;
        }

        protected object convertToInternalValue(object externalValue)
        {
            if (!(externalValue is T))
                throw new NotImplementedException();
            return (T)externalValue;
        }

        public bool Validate()
        {
            bool hasErrors = false;
            List<string> errors = new List<String>();
            if (Name == null)
                throw new NotSupportedException("Name cannot be null");
            if (this.IsRequired && !this.data.ContainsKey(this.Name))
            {
                errors.Add("This field is required.");
                hasErrors = true;
            }
            if (this.IsReadOnly && this.data.ContainsKey(this.Name))
            {
                errors.Add("This field is read only.");
                hasErrors = true;
            }
            var value = this.data[this.Name];
            try
            {
                InternalValue = convertToInternalValue(value);
                ExternalValue = convertToExternalValue(value);
            }
            catch (InvalidCastException)
            {
                errors.Add("Data is wrong type.");
                hasErrors = true;
            }
            return hasErrors;
        }
    }

    public interface IField
    {
        bool Validate();
    };
}