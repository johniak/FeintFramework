using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Feint.FeintORM
{
    /// <summary>
    /// Lazy ForeginKey property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBForeignKey<T> where T:DBModel
    {
        public T Value
        {
            get
            {
                if(_value==null){
                    var values=DBModel.Find<T>().Where().Eq("Id",Id).Execute();
                    if (values.Count > 0)
                        _value = values[0];
                }
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        private T _value;
		public Int64 Id{ get; private set; }

        public String Representative { get { return Value.ToString(); } }

        private DBForeignKey()
        {
          
        }
        public static implicit operator DBForeignKey<T>(T d)
        {
            var ret=new DBForeignKey<T>();
            ret._value=d;
            return ret;
        }
        
        public static implicit operator T(DBForeignKey<T> d)
        {
            return d.Value;
        }
        
    }
}
