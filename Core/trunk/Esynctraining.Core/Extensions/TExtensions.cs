namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// The generic extensions.
    /// </summary>
// ReSharper disable InconsistentNaming
    public static class TExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get cookie.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCookie(this CookieCollection collection, string name)
        {
            if (collection != null)
            {
                return (from Cookie c in collection where c.Name == name select c.Value).FirstOrDefault();
            }
            return null;
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Type t, bool inherit = false)
        {
            return t.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo t, bool inherit = false)
        {
            return t.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// The get cookie.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCookie(this string header, string name)
        {
            if (header != null)
            {
                var cookies = header.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return (from cookie in cookies where cookie.IndexOf("=", StringComparison.Ordinal) > 0 && cookie.Substring(0, cookie.IndexOf("=", StringComparison.Ordinal)) == name let index = cookie.IndexOf("=", StringComparison.Ordinal) select cookie.Substring(index + 1, cookie.Length - index - 1)).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// The get authentication cookie names.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public static IEnumerable<string> GetAuthCookieNames(this string header)
        {
            var list = new List<string>();
            if (header != null)
            {
                var cookies = header.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                list.AddRange(from cookie in cookies where cookie.IndexOf("=", StringComparison.Ordinal) > 0 select cookie.Substring(0, cookie.IndexOf("=", StringComparison.Ordinal)));
            }
            return list;
        }

        /// <summary>
        /// The deep clone.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <typeparam name="T">
        /// Type to be cloned
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> instance.
        /// </returns>
        public static T DeepClone<T>(this T a)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// If evaluator than result else default value.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <typeparam name="TInput">
        /// Type of input
        /// </typeparam>
        /// <typeparam name="TResult">
        /// Type of result
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public static TResult If<TInput, TResult>(this TInput o, Func<TInput, bool> evaluator, Func<TInput, TResult> result) where TInput : class
        {
            if (o == null)
            {
                return default(TResult);
            }

            return evaluator(o) ? result(o) : default(TResult);
        }

        /// <summary>
        /// If evaluator than result else default value.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <param name="affirmative">
        /// The affirmative.
        /// </param>
        /// <param name="negative">
        /// The negative.
        /// </param>
        /// <typeparam name="TInput">
        /// Type of input
        /// </typeparam>
        /// <typeparam name="TResult">
        /// Type of result
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public static TResult IfElse<TInput, TResult>(this TInput o, Func<TInput, bool> evaluator, Func<TInput, TResult> affirmative, Func<TInput, TResult> negative) where TInput : class
        {
            if (o == null)
            {
                return default(TResult);
            }

            return evaluator(o) ? affirmative(o) : negative(o);
        }

        /// <summary>
        /// The is default or null.
        /// </summary>
        /// <param name="o">
        /// The object.
        /// </param>
        /// <typeparam name="TInput">
        /// The input type
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDefaultOrNull<TInput>(this TInput? o) where TInput : struct
        {
            if (o.HasValue)
            {
                return o.Value.Equals(default(TInput));
            }

            return true;
        }

        /// <summary>
        /// The if.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <typeparam name="TInput">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TInput"/>.
        /// </returns>
        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
        {
            if (o == null)
            {
                return null;
            }

            return evaluator(o) ? o : null;
        }

        /// <summary>
        /// The is default or null.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static string TryResolveFileName(this string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var uri = new Uri(path);
                    return uri.Segments[uri.Segments.Length - 1];
                }
                catch (Exception)
                {
                    try
                    {
                        return Path.GetFileName(path);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The return.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <param name="failureValue">
        /// The failure value.
        /// </param>
        /// <typeparam name="TInput">
        /// The input
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The result
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) 
        {
            if (o == null)
            {
                return failureValue;
            }

            return evaluator(o);
        }

        /// <summary>
        /// The to enumerable.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <typeparam name="T">
        /// Type of enumerable
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> ToEnumerable<T>(this IQueryable<T> o)
        {
            if (o == null)
            {
                return null;
            }

            return o.ToList();
        }

        /// <summary>
        /// Checks if executor object is null. And if not execute function embed in it.
        /// </summary>
        /// <param name="o">
        /// The object.
        /// </param>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <typeparam name="TInput">
        /// The input
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The result
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator) 
        {
            if (o == null)
            {
                return default(TResult);
            }

            return evaluator(o);
        }

        #endregion
    }

}