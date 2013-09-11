using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FeintSDK
{

    public class Form
    {
        PropertyInfo[] properties;
        public string Method { get; set; }
        public string Action { get; set; }

        public Form(Type t)
        {
            properties = t.GetProperties();
        }

        public override string ToString()
        {
            return "";
        }
    }

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public abstract class FormField :Attribute
	{
		public bool Requierd;
		public FormField(bool requierd)
		{
			this.Requierd = requierd;
		}

		public virtual bool IsValid (object o);
	}

	public class CharField :FormField
	{
		public int Lenght=-1;
		public string RegexPattern;
		public CharField(int lenght):base(true)
		{
			this.Lenght = lenght;
		}
		public CharField(string regexPattern):base(true)
		{
			this.RegexPattern = regexPattern;
		}
		public CharField(int lenght,string regexPattern):base(true)
		{
			this.RegexPattern = regexPattern;
			this.Lenght = lenght;
		}
		public CharField(int lenght,bool requierd):base(requierd)
		{
			this.Lenght = lenght;
		}
		public CharField(string regexPattern,bool requierd):base(requierd)
		{
			this.RegexPattern = regexPattern;
		}
		public CharField(int lenght,string regexPattern,bool requierd):base(requierd)
		{
			this.RegexPattern = regexPattern;
			this.Lenght = lenght;
		}
		public override bool IsValid (object o)
		{
			String field = o.ToString ();
			if (o == null && Requierd)
				return false;
			if (Lenght != -1 && field.Length < Lenght)
				return false;
			if (RegexPattern != null) 
			{
				Match m = Regex.Match (field, RegexPattern);
				if (!m.Success)
					return false;
			}
			return true;
		}
	}
}
