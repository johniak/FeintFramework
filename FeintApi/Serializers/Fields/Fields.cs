using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FeintApi.Serializers.Fields
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FieldAttribute : Attribute
    {
        public bool IsReadOnly { get; set; }
        public bool IsWriteOnly { get; set; }
        public bool IsRequired { get; set; }
        public string Source { get; set; }
        public List<Validator> Validators  = new List<Validator>();
    }

    public abstract class BaseField
    {
        protected FieldAttribute attributeInstance;

        public FieldAttribute AttributeInstance
        {
            get
            {
                return attributeInstance ?? defaultAttributeInstance;
            }
            protected set { attributeInstance = value; }
        }

        public IEnumerable<Validator> Validators
        {
            get
            {
                return defaultValidators.Concat(AttributeInstance.Validators);
            }
        }

        protected IEnumerable<Validator> defaultValidators
        {
            get
            {
                return new[] { new BasicValidator() };
            }
        }

        protected FieldAttribute defaultAttributeInstance = new FieldAttribute() { };

        public BaseField Parent { get; set; }

        public abstract object ToRepresentation(object value);

        public abstract object ToInternalValue(object data);

        public string Name { get; set; }

        public BaseField()
        {

        }

        public virtual void Bind(string name, BaseField parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        public virtual object GetValue(Dictionary<string, object> data)
        {
            return data[this.Name];
        }

        public virtual object Validate(object data)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)data;
            List<ValidationException> exceptions = new List<ValidationException>();
            object value = null;
            bool empty = false;
            try
            {
                value = ToInternalValue(GetValue(dict));
            }
            catch (KeyNotFoundException)
            {
                empty = true;
            }
            catch (ValidationException ex)
            {
                exceptions.Add(ex);
                throw new ValidationException(Name, exceptions);
            }
            foreach (var validator in Validators)
            {
                try
                {
                    validator.Validate(this, value, empty);
                }
                catch (ValidationException ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
                throw new ValidationException(Name, exceptions);
            return value;
        }
    }

    public class Field<T> : BaseField
    {
        public Field() : base()
        {

        }

        public override object ToInternalValue(object data)
        {
            if (data == null)
                return null;
            try
            {
                return (T)data;
            }
            catch (InvalidCastException)
            {
                throw new ValidationException($"This is not a valid {typeof(T).Name} field.");
            }
        }

        public override object ToRepresentation(object value)
        {
            return value;
        }
    }
}