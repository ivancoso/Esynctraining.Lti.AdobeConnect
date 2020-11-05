using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Esynctraining.ImportExport
{
    /// <summary>
    /// Represents methods to build expession trees.
    /// </summary>
    /// <remarks>This class doesn't use any expression caching logic inside.</remarks>
    public static class ExpressionUtility
    {
        public static string GetPropertyName<TContainer>(Expression<Func<TContainer, object>> getter)
        {
            //Check.Argument.IsNotNull(getter, "getter");

            var property = GetProperty(getter);
            return property.Name;
        }

        public static Expression<Action<TContainer, object>> GetPropertySetter<TContainer>(Expression<Func<TContainer, object>> getter)
        {
            //Check.Argument.IsNotNull(getter, "getter");

            var property = GetProperty(getter);
            if (!property.CanWrite)
                throw new ArgumentException("Property is not writable.");

            ParameterExpression instance = Expression.Parameter(typeof(TContainer), "instance");
            ParameterExpression parameter = Expression.Parameter(typeof(object), "param");

            var casting = Expression.Convert(parameter, property.PropertyType);

            return Expression.Lambda<Action<TContainer, object>>(
                Expression.Call(instance, property.GetSetMethod(), casting),
                new ParameterExpression[] { instance, parameter });
        }


        private static PropertyInfo GetProperty<TContainer, TProperty>(Expression<Func<TContainer, TProperty>> getter)
        {
            if (getter == null)
                throw new ArgumentNullException("getter");

            MemberExpression memberEx;
            if (getter.Body.NodeType == ExpressionType.Convert)
            {
                memberEx = ((UnaryExpression)getter.Body).Operand as MemberExpression;
            }
            else
            {
                memberEx = getter.Body as MemberExpression;
            }

            if (memberEx == null)
                throw new ArgumentException("Body is not a member-expression.");

            var property = memberEx.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Member is not a property.");
            return property;
        }

        private static Expression BoxIfNeeded(Expression member)
        {
            return (member.Type.IsValueType)
                ? Expression.Convert(member, typeof(object))
                : member;
        }

    }

}
