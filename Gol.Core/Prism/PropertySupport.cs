using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Gol.Core.Prism
{
    /// <summary>
    ///     Provides support for extracting property information based on a property expression.
    /// </summary>
    public static class PropertySupport
    {
        /// <summary>
        ///     Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p =&gt; p.PropertyName)</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown if the <paramref name="propertyExpression" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     Thrown when the expression is:<br />
        ///     Not a <see cref="T:System.Linq.Expressions.MemberExpression" /><br />
        ///     The <see cref="T:System.Linq.Expressions.MemberExpression" /> does not represent a property.<br />
        ///     Or, the property is static.
        /// </exception>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (propertyExpression.Body is not MemberExpression body)
            {
                throw new ArgumentException();
            }

            var member = body.Member as PropertyInfo;

            if (member == null)
            {
                throw new ArgumentException();
            }

            var methodInfo = member.GetGetMethod(true);

            if (methodInfo is null || methodInfo.IsStatic)
            {
                throw new ArgumentException();
            }

            return body.Member.Name;
        }
    }
}