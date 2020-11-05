namespace Esynctraining.Core.Utils
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The lambda property.
    /// </summary>
    public static class Lambda
    {
        #region Public Methods and Operators

        /// <summary>
        /// The property name.
        /// </summary>
        /// <param name="property">
        /// The property expression.
        /// </param>
        /// <typeparam name="T">
        /// Type of the property object container
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/> name of the property.
        /// </returns>
        public static string Property<T>(Expression<Func<T, object>> property)
        {
            var unaryExpression = property.Body as UnaryExpression;
            var memberExpression = (MemberExpression)(unaryExpression != null ? unaryExpression.Operand : property.Body);
            return memberExpression.Member.Name;
        }

        public static string Method<T>(Expression<Action<T>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            return GetMethod(expression.Body);
        }

        public static MethodInfo MethodInfo<T>(Expression<Action<T>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            return GetMethodInfo(expression.Body);
        }

        private static string GetMethod(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static MethodInfo GetMethodInfo(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMethodInfo(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }
        
        private static string GetMemberName(
                UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression =
                    (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand)
                        .Member.Name;
        }

        private static MethodInfo GetMethodInfo(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method;
            }

            return null;
        }

        #endregion
    }
}