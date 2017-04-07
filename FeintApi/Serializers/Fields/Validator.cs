namespace FeintApi.Serializers.Fields
{
    public interface Validator
    {
        void Validate(BaseField Field, object value, bool empty = false);
    }
}