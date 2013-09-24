using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FeintSDK
{

    public class Form
    {
        public bool IsValid { get { return _IsValid; } private set { _IsValid = value; } }
        private bool _IsValid = true;



        public static T FromFormData<T>(Dictionary<String, String> content) where T : Form
        {
            T instance = (T)Activator.CreateInstance(typeof(T));
            var fields = getFields(typeof(T));
            foreach (var f in fields)
            {
                FormField attribute = (FormField)f.GetCustomAttribute(typeof(FormField), false);
                bool isValid;
                if (content.ContainsKey(f.Name))
                {
                    isValid = attribute.IsValid(content[f.Name]);
                    if (isValid)
                    {
                        f.SetValue(instance, attribute.GetValue(content[f.Name]));
                    }
                }
                else
                {
                    isValid = attribute.IsValid(null);
                }
                instance.IsValid = instance.IsValid && isValid;
            }
            return instance;
        }

        private static IEnumerable<PropertyInfo> getFields(Type t)
        {

            foreach (var p in t.GetProperties())
            {
                if (p.GetCustomAttributes(typeof(FormField), true).Length > 0)
                    yield return p;
            }
        }



        public override string ToString()
        {
            return "";
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class FormField : Attribute
    {
        public bool Requierd { get; set; }
        public FormField()
        {
            this.Requierd = true;
        }

        public abstract bool IsValid(object o);
        public abstract dynamic GetValue(object o);
    }

    public class CharField : FormField
    {
        public int MinLenght { get; set; }
        public int MaxLenght { get; set; }
        public string RegexPattern { get; set; }

        public CharField()
            : base()
        {
            MinLenght = -1;
            MaxLenght = -1;
        }

        
        public override bool IsValid(object o)
        {
            if (o == null && Requierd)
                return false;
            if (o == null && !Requierd)
                return true;
            String field = o.ToString();
            if (MaxLenght != -1 && field.Length > MaxLenght)
                return false;
            if (MinLenght != -1 && field.Length < MinLenght)
                return false;
            if (RegexPattern != null)
            {
                Match m = Regex.Match(field, RegexPattern);
                if (!m.Success)
                    return false;
            }
            return true;
        }

        public override dynamic GetValue(object o)
        {
            return o.ToString();
        }
    }

    public class IntegerField : FormField
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string RegexPattern { get; set; }

        public IntegerField()
            : base()
        {
            MinValue = Int32.MinValue;
            MaxValue = Int32.MaxValue;
        }

        public override bool IsValid(object o)
        {
            if (o == null && Requierd)
                return false;
            if (o == null && !Requierd)
                return true;
            String field = o.ToString();
            int value;
            try
            {
                value = Int32.Parse(field);
                if (value < MinValue)
                    return false;
                if (value > MaxValue)
                    return false;
                if (RegexPattern != null)
                {
                    Match m = Regex.Match(field, RegexPattern);
                    if (!m.Success)
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public override dynamic GetValue(object o)
        {
            return Int32.Parse(o.ToString()); ;
        }
    }


    public class DateTimeField : FormField
    {
        public string DataTimeFormatString { get; set; }
        public string RegexPattern { get; set; }

        public DateTimeField()
            : base()
        {
        }

        public override bool IsValid(object o)
        {
            if (o == null && Requierd)
                return false;
            if (o == null && !Requierd)
                return true;
            String field = o.ToString();
            if (DataTimeFormatString == null)
                return false;
            try
            {
                DateTime.ParseExact(field, DataTimeFormatString, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (RegexPattern != null)
            {
                Match m = Regex.Match(field, RegexPattern);
                if (!m.Success)
                    return false;
            }
            return true;
        }
        public override dynamic GetValue(object o)
        {
            return DateTime.ParseExact(o.ToString(), DataTimeFormatString, CultureInfo.InvariantCulture);
        }
    }
}
