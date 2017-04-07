using System;
using System.Collections.Generic;
using FeintSDK.Exceptions;

namespace FeintApi.Serializers
{
    public class ValidationException : HttpException
    {
        public IEnumerable<ValidationException> Exceptions { get; protected set; }
        public string FieldName { get; protected set; }
        public ValidationException(string message) : base(message)
        {

        }

        public ValidationException(IEnumerable<ValidationException> exceptions) : base()
        {
            Exceptions = exceptions;
        }

        public ValidationException(string fieldName, IEnumerable<ValidationException> exceptions) : base()
        {
            FieldName = fieldName;
            Exceptions = exceptions;
        }
    }
}