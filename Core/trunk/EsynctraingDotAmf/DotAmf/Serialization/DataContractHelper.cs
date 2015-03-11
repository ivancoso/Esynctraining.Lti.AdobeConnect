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
    using System.Collections.Concurrent;
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
        private static readonly DateTime _origin;
        private static readonly DateTime _originLocalTime;
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ExpressionCreator> _creators;


        static DataContractHelper()
        {
            _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            _originLocalTime = _origin.ToLocalTime();
            _creators = new ConcurrentDictionary<RuntimeTypeHandle, ExpressionCreator>();            
        }

        #region Public Methods and Operators

        /// <summary>
        /// Convert a <c>DateTime</c> to a UNIX timestamp in milliseconds.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        internal static double ConvertToTimestamp(DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                return (value - _originLocalTime).TotalSeconds * 1000;
            }
            return (value - _origin).TotalSeconds * 1000;
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
        internal static string GetContractAlias(Type type)
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
        /// Get data contract object's properties.
        /// </summary>
        /// <param name="instance">
        /// Object instance.
        /// </param>
        /// <param name="properties">
        /// Type's properties.
        /// </param>
        /// <returns>
        /// A set of property name-value pairs.
        /// </returns>
        internal static Dictionary<string, object> CollectPropertyValues(
            object instance, 
            IEnumerable<PropertyDescriptor> properties)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (properties == null)
                throw new ArgumentNullException("properties");

            var map = new Dictionary<string, object>(properties.Count());
            foreach (PropertyDescriptor property in properties)
            {
                map.Add(property.Name, property.GetValue(instance));
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
        public static IDictionary<string, PropertyInfo> GetContractProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var properties = from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |BindingFlags.SetProperty | BindingFlags.GetProperty)
                                  select property;

            //if (type.FullName == "EdugameCloud.Core.Domain.DTO.QuestionFromStoredProcedureDTO")
            //{
            //using (var log = System.IO.File.CreateText(@"C:\tmp\amf\" + type.FullName + "-" + Guid.NewGuid().ToString() + ".txt"))
            //{
            //    log.WriteLine(type.AssemblyQualifiedName);
            //    log.Write(string.Join(";", properties.Select(x => x.Name)));
            //}
            //}

            var result = new SortedDictionary<string, PropertyInfo>();

            foreach (PropertyInfo property in properties)
            {
                // Look for a data contract attribute first
                var contractAttribute = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault() as DataMemberAttribute;

                if (contractAttribute != null)
                {
                    string propertyName = !string.IsNullOrEmpty(contractAttribute.Name)
                        ? contractAttribute.Name
                        : property.Name;

                    result.Add(propertyName, property);
                }
            }

            return result;
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
        internal static Dictionary<object, object> GetEnumValues(Type type)
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
        /// <param name="type">Data contract type.</param>
        /// <param name="values">Values to use.</param>
        /// <param name="properties">Type's properties.</param>
        /// <returns>
        /// Type instance.
        /// </returns>
        internal static object InstantiateContract(
            Type type, 
            Dictionary<string, object> values, 
            IEnumerable<PropertyDescriptor> properties
            )
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            ExpressionCreator creator = _creators.GetOrAdd(type.TypeHandle, (typeHandle) => new ExpressionCreator(type, properties));
            return creator.Create(values);
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
        /// <param name="isInteger">
        /// The is Integer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsNumericType(Type type, TypeCode typecode, out bool isInteger)
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
