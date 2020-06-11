using System.Collections.Generic;

namespace Feint.GraphQL
{
    public abstract class ScalarType<T> : BaseType
    {

        public abstract dynamic Corece(T value);

        public abstract T ParseLiteral(dynamic ast);

    }
    public static class BaseTypeExtension{
        public static bool IsScalar(this BaseType type){
            var scalarType = typeof(ScalarType<>);
            var currentType = type.GetType();
            return currentType.BaseType.IsGenericType && currentType.BaseType.GetGenericTypeDefinition() == scalarType;
        }
    }

    public class StringType : ScalarType<string>
    {
        public new string Name { get => "String"; }
        public override dynamic Corece(string value)
        {
            return value;
        }

        public override string ParseLiteral(dynamic ast)
        {
            return ast;
        }
    }

    public class IntType : ScalarType<int>
    {
        public new string Name { get => "Int"; }
        public override dynamic Corece(int value)
        {
            return value;
        }

        public override int ParseLiteral(dynamic ast)
        {
            return ast;
        }
    }

    public class FloatType : ScalarType<float>
    {
        public new string Name { get => "Float"; }
        public override dynamic Corece(float value)
        {
            return value;
        }

        public override float ParseLiteral(dynamic ast)
        {
            return ast;
        }
    }

    public class BooleanType : ScalarType<bool>
    {
        public new string Name { get => "Boolean"; }
        public override dynamic Corece(bool value)
        {
            return value;
        }

        public override bool ParseLiteral(dynamic ast)
        {
            return ast;
        }
    }
}