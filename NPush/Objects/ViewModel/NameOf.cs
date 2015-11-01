using System;
using System.Linq.Expressions;
using NoelPush.Objects.ViewModel.Itron.Mcn.Utils.Extension;

namespace NoelPush.Objects.ViewModel
{
    /// <summary>
    /// Utility class to do some name retreival from the T class
    /// </summary>
    /// <typeparam name="T">The class to fetch the name from</typeparam>
    public static class NameOf<T>
    {
        /// <summary>
        /// Get the class name as string without the generic part
        /// </summary>
        /// <returns>The class name</returns>
        public static string Class()
        {
            return typeof(T).Name.RemoveGenericPart();
        }

        /// <summary>
        /// Get the property name as string
        /// </summary>
        /// <typeparam name="TProp">The property type</typeparam>
        /// <param name="expression">The expression used to get the property</param>
        /// <returns>The property name</returns>
        /// <exception cref="ArgumentException">'expression' should be a member expression</exception>
        public static string Property<TProp>(Expression<Func<T, TProp>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("'expression' should be a member expression");

            return body.Member.Name;
        }

        /// <summary>
        /// Get the method name as string
        /// </summary>
        /// <param name="expression">The expression used to get the method</param>
        /// <returns>The method name</returns>
        /// <exception cref="ArgumentException">'expression' should be a method call expression</exception>
        public static string Method(Expression<Action<T>> expression)
        {
            var body = expression.Body as MethodCallExpression;
            if (body == null)
                throw new ArgumentException("'expression' should be a method call expression");

            return body.Method.Name;
        }

        /// <summary>
        /// Get the method name as string
        /// </summary>
        /// <param name="expression">The expression used to get the method</param>
        /// <returns>The method name</returns>
        /// <exception cref="ArgumentException">'expression' should be a method call expression</exception>
        public static string Method<TProp>(Expression<Func<T, TProp>> expression)
        {
            var body = expression.Body as MethodCallExpression;
            if (body == null)
                throw new ArgumentException("'expression' should be a method call expression");

            return body.Method.Name;
        }
    }
}

