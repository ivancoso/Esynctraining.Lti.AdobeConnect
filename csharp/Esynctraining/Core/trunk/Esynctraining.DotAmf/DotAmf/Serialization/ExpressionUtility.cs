using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DotAmf.Serialization
{
    /// <summary>
    /// Represents methods to build expession trees.
    /// </summary>
    /// <remarks>This class doesn't use any expression caching logic inside.</remarks>
    internal static class ExpressionUtility
    {
        public static Expression<Func<object, object>> GetPropertyGetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            ParameterExpression param = Expression.Parameter(typeof(object), "instance");
            Expression converted = Expression.Convert(param, property.DeclaringType);
            Expression member = GetPropertyExpression(converted, property);

            return Expression.Lambda<Func<object, object>>(member, param);
        }


        private static Expression GetPropertyExpression(Expression expression, PropertyInfo property)
        {
            Expression member = Expression.Property(expression, property);
            return BoxIfNeeded(member);
        }

        private static Expression BoxIfNeeded(Expression member)
        {
            return (member.Type.IsValueType)
                ? Expression.Convert(member, typeof(object))
                : member;
        }

    }

}
