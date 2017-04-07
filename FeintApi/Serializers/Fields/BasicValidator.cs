using System;

namespace FeintApi.Serializers.Fields
{
    public class BasicValidator : Validator
    {
        public void Validate(BaseField field, object value, bool empty = false)
        {
            if (empty && field.AttributeInstance.IsRequired)
                throw new ValidationException("This field is required.");
            if(!empty && field.AttributeInstance.IsReadOnly)
                throw new ValidationException("This field is read only.");
        }
    }
}