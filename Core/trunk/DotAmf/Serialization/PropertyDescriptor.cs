using System;
using System.Reflection;

namespace DotAmf.Serialization
{
    internal sealed class PropertyDescriptor
    {
        private readonly string _name;
        private readonly PropertyInfo _propInfo;
        private readonly Func<object, object> _getValue;


        public string Name { get { return _name; } }

        public PropertyInfo PropInfo { get { return _propInfo; } }


        public PropertyDescriptor(string name, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            _propInfo = property;

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
