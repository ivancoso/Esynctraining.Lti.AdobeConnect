using System;
using System.Reflection;

namespace DotAmf.Serialization
{
    internal sealed class PropertyDescriptor
    {
        private readonly string _name;
        private readonly PropertyInfo _property;
        private readonly Func<object, object> _getValue;


        public string ContractPropertyName { get { return _name; } }

        public PropertyInfo Property { get { return _property; } }


        public PropertyDescriptor(string name, PropertyInfo property)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Name can't be empty", "name");
            if (property == null)
                throw new ArgumentNullException("property");

            _property = property;

            _name = name;
            _getValue = ExpressionUtility.GetPropertyGetter(property).Compile();
        }


        public object GetValue(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            return _getValue(instance);
        }

    }

}
