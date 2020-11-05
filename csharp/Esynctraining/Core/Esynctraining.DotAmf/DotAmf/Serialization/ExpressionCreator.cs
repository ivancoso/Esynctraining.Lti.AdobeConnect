using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DotAmf.Serialization.TypeAdapters;

namespace DotAmf.Serialization
{
    internal sealed class ExpressionCreator
    {
        private readonly Func<Dictionary<string, object>, object> _creator;


        public ExpressionCreator(Type type, IEnumerable<PropertyDescriptor> properties)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (properties == null)
                throw new ArgumentNullException("properties");

            var newExpression = Expression.New(type);
            var dictParam = Expression.Parameter(typeof(Dictionary<string, object>), "d");

            var list = new List<MemberBinding>(properties.Count());
            foreach (var propertyInfo in properties)
            {
                Expression call = Expression.Call(
                    typeof(DictionaryExtension),
                    "GetValue", 
                    new[] { propertyInfo.Property.PropertyType },
                    new Expression[]
                    {
                        dictParam,
                        Expression.Constant(propertyInfo.ContractPropertyName)
                    });

                MemberBinding mb = Expression.Bind(propertyInfo.Property, call);
                list.Add(mb);
            }

            var ex = Expression.Lambda<Func<Dictionary<string, object>, object>>(
                Expression.MemberInit(newExpression, list),
                new[] { dictParam });

            _creator = ex.Compile();
        }


        public object Create(Dictionary<string, object> props)
        {
            if (props == null)
                throw new ArgumentNullException("props");

            return _creator(props);
        }

    }

    internal static class DictionaryExtension
    {
        private static readonly List<BaseTypeAdapter> adapters = new List<BaseTypeAdapter>();


        static DictionaryExtension()
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
                // TODO: add logging
            }
        }


        private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType, params Type[] exceptions)
        {
            var exceptionsList = exceptions != null ? new List<Type>(exceptions) : new List<Type>();
            exceptionsList.Add(baseType);
            return assembly.GetTypes().Where(t => !exceptionsList.Contains(t) && baseType.IsAssignableFrom(t));

        }
        
        private static Func<object[], object> CreateConstructorDelegate(ConstructorInfo method)
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

        // TODO: create GetAdaptedValue method??
        // TODO: create GetEnumValue method??
        public static TType GetValue<TType>(this Dictionary<string, object> d, string name)
        {
            object value;
            if (!d.TryGetValue(name, out value))
                return default(TType);

            if (value == null)
                return default(TType);

            var adapter = adapters.FirstOrDefault(x => x.BinderTypes.Contains(typeof(TType).TypeHandle));
            if (adapter != null)
            {
                return (TType)adapter.Adapt(typeof(TType).TypeHandle, value);
            }

            if (!value.GetType().TypeHandle.Equals(typeof(TType).TypeHandle) && !typeof(TType).IsEnum && (value is IConvertible))
            {
                return (TType)ChangeType(value, typeof(TType));
            }

            var array = value as object[];
            if ((array != null) && (array.Length == 0))
            {
                return default(TType);
            }

            return (TType)value;
        }

        private static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

    }


}
