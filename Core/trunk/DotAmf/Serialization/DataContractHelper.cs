// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="DataContractHelper.cs">
//   
// </copyright>
// <summary>
//   Data contract helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    using DotAmf.Serialization.TypeAdapters;

    /// <summary>
    ///     Data contract helper.
    /// </summary>
    public static class DataContractHelper
    {
        private static List<BaseTypeAdapter> adapters = new List<BaseTypeAdapter>(); 

        static DataContractHelper()
        {
            try
            {
                var args = new object[0];
                var typeAdaptersTypes = FindDerivedTypes(Assembly.GetExecutingAssembly(), typeof(BaseTypeAdapter), typeof(BaseTypeAdapter<>), typeof(BaseTypeAdapter<,>), typeof(BaseTypeAdapter<,,>));
                foreach (var adaptersType in typeAdaptersTypes)
                {
                    adapters.Add((BaseTypeAdapter)CreateConstructorDelegate(adaptersType.GetConstructor(new Type[0]))(args));
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region Public Methods and Operators

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType, params Type[] exceptions)
        {
            var exceptionsList = exceptions != null ? new List<Type>(exceptions) : new List<Type>();
            exceptionsList.Add(baseType);
            return assembly.GetTypes().Where(t => !exceptionsList.Contains(t) && baseType.IsAssignableFrom(t));

        }

        public static Func<object[], object> CreateConstructorDelegate(ConstructorInfo method)
        {
            var args = Expression.Parameter(typeof(object[]), "args");

            var parameters = new List<Expression>();

            var methodParameters = method.GetParameters().ToList();
            for (var i = 0; i < methodParameters.Count; i++)
            {
                parameters.Add(Expression.Convert(
                                   Expression.ArrayIndex(args, Expression.Constant(i)),
                                   methodParameters[i].ParameterType));
            }

            var call = Expression.Convert(Expression.New(method, parameters), typeof(object));

            Expression body = call;

            var callExpression = Expression.Lambda<Func<object[], object>>(body, args);
            var result = callExpression.Compile();

            return result;
        }

        /// <summary>
        /// Convert a <c>DateTime</c> to a UNIX timestamp in milliseconds.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ConvertToTimestamp(DateTime value)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            if (value.Kind != DateTimeKind.Utc)
            {
                origin = origin.ToLocalTime();
            }

            return (value - origin).TotalSeconds * 1000;
        }

        /// <summary>
        /// Get data contract type's alias.
        /// </summary>
        /// <param name="type">
        /// Data contract type.
        /// </param>
        /// <returns>
        /// Alias name.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Type is not a valid data contract.
        /// </exception>
        public static string GetContractAlias(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // Look for a data contract attribute
            var contractAttribute =
                type.GetCustomAttributes(typeof(DataContractAttribute), false).FirstOrDefault() as DataContractAttribute;

            if (contractAttribute != null)
            {
                return !string.IsNullOrEmpty(contractAttribute.Name)
                           ? contractAttribute.Name
                           : type.FullName ?? type.Name;
            }

            throw new ArgumentException(
                string.Format(Errors.DataContractUtil_GetContractAlias_InvalidContract, type.FullName), 
                "type");
        }

        /// <summary>
        /// Get fields of data contract type.
        /// </summary>
        /// <param name="type">
        /// Data contract type.
        /// </param>
        /// <returns>
        /// A set of name-field pairs.
        /// </returns>
        public static IEnumerable<KeyValuePair<string, FieldInfo>> GetContractFields(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            IEnumerable<FieldInfo> validFields = from field in type.GetFields()
                                                 where field.IsPublic && !field.IsStatic
                                                 select field;

            foreach (FieldInfo field in validFields)
            {
                // Look for a data contract attribute first
                var contractAttribute =
                    field.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault() as DataMemberAttribute;

                if (contractAttribute != null)
                {
                    string propertyName = !string.IsNullOrEmpty(contractAttribute.Name)
                                              ? contractAttribute.Name
                                              : field.Name;

                    yield return new KeyValuePair<string, FieldInfo>(propertyName, field);
                }
            }
        }

        /// <summary>
        /// Get contract members from a contract type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<Type> GetContractMembers(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            IEnumerable<Type> properties = from pair in GetContractProperties(type) select pair.Value.PropertyType;

            IEnumerable<Type> fields = from pair in GetContractFields(type) select pair.Value.FieldType;

            return properties.Concat(fields).Distinct();
        }

        /// <summary>
        /// Get data contract object's properties.
        /// </summary>
        /// <param name="instance">
        /// Object instance.
        /// </param>
        /// <returns>
        /// A set of property name-value pairs.
        /// </returns>
        public static Dictionary<string, object> GetContractProperties(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Type type = instance.GetType();

            IEnumerable<KeyValuePair<string, object>> fields = from data in GetContractFields(type)
                                                               select
                                                                   new KeyValuePair<string, object>(
                                                                   data.Key, 
                                                                   data.Value.GetValue(instance));

            IEnumerable<KeyValuePair<string, object>> properties = from data in GetContractProperties(type)
                                                                   select
                                                                       new KeyValuePair<string, object>(
                                                                       data.Key, 
                                                                       data.Value.GetValue(instance, null));

            IEnumerable<KeyValuePair<string, object>> contents = fields.Concat(properties);

            var map = new Dictionary<string, object>();

            foreach (var pair in contents)
            {
                map[pair.Key] = pair.Value;
            }

            return map;
        }

        /// <summary>
        /// Get data contract object's properties.
        /// </summary>
        /// <param name="instance">
        /// Object instance.
        /// </param>
        /// <param name="properties">
        /// Type's properties.
        /// </param>
        /// <param name="fields">
        /// Type's fields.
        /// </param>
        /// <returns>
        /// A set of property name-value pairs.
        /// </returns>
        public static Dictionary<string, object> GetContractProperties(
            object instance, 
            IEnumerable<KeyValuePair<string, PropertyInfo>> properties, 
            IEnumerable<KeyValuePair<string, FieldInfo>> fields)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            IEnumerable<KeyValuePair<string, object>> fieldValues = from data in fields
                                                                    select
                                                                        new KeyValuePair<string, object>(
                                                                        data.Key, 
                                                                        data.Value.GetValue(instance));

            IEnumerable<KeyValuePair<string, object>> propertiyValues = from data in properties
                                                                        select
                                                                            new KeyValuePair<string, object>(
                                                                            data.Key, 
                                                                            data.Value.GetValue(instance, null));

            IEnumerable<KeyValuePair<string, object>> contents = fieldValues.Concat(propertiyValues);

            var map = new Dictionary<string, object>();

            foreach (var pair in contents)
            {
                map[pair.Key] = pair.Value;
            }

            return map;
        }

        /// <summary>
        /// Get properties of data contract type.
        /// </summary>
        /// <param name="type">
        /// Data contract type.
        /// </param>
        /// <returns>
        /// A set of name-property pairs.
        /// </returns>
        public static IEnumerable<KeyValuePair<string, PropertyInfo>> GetContractProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            IEnumerable<PropertyInfo> validProperties = from property in type.GetProperties()
                                                        where property.CanWrite && property.CanRead
                                                        select property;

            foreach (PropertyInfo property in validProperties)
            {
                // Look for a data contract attribute first
                var contractAttribute =
                    property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault() as
                    DataMemberAttribute;

                if (contractAttribute != null)
                {
                    string propertyName = !string.IsNullOrEmpty(contractAttribute.Name)
                                              ? contractAttribute.Name
                                              : property.Name;

                    yield return new KeyValuePair<string, PropertyInfo>(propertyName, property);
                }
            }
        }

        /// <summary>
        /// Get enumeration type's values.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<object, object> GetEnumValues(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!type.IsEnum)
            {
                throw new ArgumentException("Type is not an enum.");
            }

            var result = new Dictionary<object, object>();
            Type enumType = Enum.GetUnderlyingType(type);

            bool isFlag = type.GetCustomAttributes(false).OfType<FlagsAttribute>().FirstOrDefault() != null;

            if (isFlag)
            {
                int maxEnumValue = 1 << (Enum.GetValues(type).Length - 1);
                IEnumerable<int> combinations = Enumerable.Range(0, maxEnumValue);

                foreach (int value in combinations)
                {
                    result[Enum.ToObject(type, value)] = value;
                }
            }
            else
            {
                foreach (object value in Enum.GetValues(type))
                {
                    result[value] = Convert.ChangeType(value, enumType);
                }
            }

            return result;
        }

        /// <summary>
        /// Instantiate a data contract object and populate it with provided properties.
        /// </summary>
        /// <param name="type">
        /// Data contract type.
        /// </param>
        /// <param name="values">
        /// Values to use.
        /// </param>
        /// <returns>
        /// Data contract instance.
        /// </returns>
        public static object InstantiateContract(Type type, IEnumerable<KeyValuePair<string, object>> values)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            object instance = Activator.CreateInstance(type);

            var fieldMap = from data in GetContractFields(type)
                           join prop in values on data.Key equals prop.Key
                           select new { field = data.Value, value = prop.Value };

            foreach (var pair in fieldMap)
            {
                pair.field.SetValue(instance, pair.value);
            }

            var propertyMap = from data in GetContractProperties(type)
                              join prop in values on data.Key equals prop.Key
                              select new { property = data.Value, value = prop.Value };

            foreach (var pair in propertyMap)
            {
                pair.property.SetValue(instance, pair.value, null);
            }

            return instance;
        }

        /// <summary>
        /// Instantiate a data contract object and populate it with provided properties.
        /// </summary>
        /// <param name="type">
        /// Data contract type.
        /// </param>
        /// <param name="values">
        /// Values to use.
        /// </param>
        /// <param name="properties">
        /// Type's properties.
        /// </param>
        /// <param name="fields">
        /// Type's fields.
        /// </param>
        /// <returns>
        /// Type instance.
        /// </returns>
        public static object InstantiateContract(
            Type type, 
            IEnumerable<KeyValuePair<string, object>> values, 
            IEnumerable<KeyValuePair<string, PropertyInfo>> properties, 
            IEnumerable<KeyValuePair<string, FieldInfo>> fields)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object instance = Activator.CreateInstance(type);

            var fieldMap = from data in fields
                           join prop in values on data.Key equals prop.Key
                           select new { field = data.Value, value = prop.Value };

            foreach (var pair in fieldMap)
            {
                pair.field.SetValue(instance, pair.value);
            }

            var propertyMap = from data in properties
                              join prop in values on data.Key equals prop.Key
                              select new { property = data.Value, value = prop.Value };

            foreach (var pair in propertyMap)
            {
                var adapter = adapters.FirstOrDefault(x => x.BinderTypes.Contains(pair.property.PropertyType)); 
                if (adapter != null)
                {
                    var adaptedValue = adapter.Adapt(pair.property.PropertyType, pair.value);
                    pair.property.SetValue(instance, adaptedValue, null);
                }
                else if (pair.value is object[] && ((object[])pair.value).Length == 0)
                {
                    pair.property.SetValue(instance, null, null);
                }
                else
                {
                    pair.property.SetValue(instance, pair.value, null);
                }
            }

            return instance;
        }

        /// <summary>
        /// Check if type is a valid data contract.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDataContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // Look for a data contract attribute
            var contractAttribute =
                type.GetCustomAttributes(typeof(DataContractAttribute), false).FirstOrDefault() as DataContractAttribute;

            return contractAttribute != null;
        }

        /// <summary>
        /// Check if type is a numeric type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="typecode">
        /// The typecode.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNumericType(Type type, TypeCode typecode)
        {
            bool isInteger;
            return IsNumericType(type, typecode, out isInteger);
        }

        /// <summary>
        /// Check if type is a numeric type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="typecode">
        /// The typecode.
        /// </param>
        /// <param name="isInteger">
        /// The is Integer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNumericType(Type type, TypeCode typecode, out bool isInteger)
        {
            isInteger = false;

            if (type == null)
            {
                return false;
            }

            switch (typecode)
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    isInteger = true;
                    return true;

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                case TypeCode.Object:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type subtype = Nullable.GetUnderlyingType(type);
                            return IsNumericType(subtype, Type.GetTypeCode(subtype), out isInteger);
                        }

                        return false;
                    }
            }

            return false;
        }

        #endregion
    }
}