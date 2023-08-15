using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.ExceptionalContinued.Models
{
    /// <summary>
    /// Generates a description to use as the documentation of ArgumentNullException.
    /// </summary>
    internal sealed class ArgumentNullExceptionDescription
    {
        #region member vars

        /// <summary>
        /// Arguments of the thrown exception.
        /// </summary>
        private readonly ICollection<ICSharpArgument> _arguments;

        /// <summary>
        /// Tree representation of the thrown statement.
        /// </summary>
        private readonly IThrowStatement _statement;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullExceptionDescription" /> class.
        /// </summary>
        /// <param name="statement">The thrown statement.</param>
        public ArgumentNullExceptionDescription(IThrowStatement statement)
        {
            _statement = statement;
            _arguments = GetArguments();
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates a new <see cref="ArgumentNullExceptionDescription" /> from a <see cref="ThrownExceptionModel" />.
        /// </summary>
        /// <param name="exception">The exception model.</param>
        /// <returns>
        /// A new <see cref="ArgumentNullExceptionDescription" /> instance or <see langword="null" /> when the exception type is
        /// not an <see cref="System.ArgumentNullException" />
        /// </returns>
        public static ArgumentNullExceptionDescription CreateFrom(ThrownExceptionModel exception)
        {
            if ("ArgumentNullException".Equals(exception.ExceptionType.GetClrName().ShortName))
            {
                return new ArgumentNullExceptionDescription(exception.ExceptionsOrigin.Node as IThrowStatement);
            }
            return null;
        }

        /// <summary>
        /// Generates a description based on the <see cref="System.ArgumentNullException" />.
        /// </summary>
        /// <returns>
        /// A string description.
        /// </returns>
        public string GetDescription()
        {
            if (!IsOfType(@"ArgumentNullException"))
            {
                return string.Empty;
            }
            switch (_arguments.Count)
            {
                case 0:
                    return @"Thrown when the arguments are <see langword=""null""/>";
                case 1:
                    return $@"<paramref name=""{GetArgumentText("paramName")}""/> is <see langword=""null""/>";
                case 2:
                    if (HasParamAndMessageArguments())
                    {
                        return $@"{GetArgumentText("message")} <paramref name=""{GetArgumentText("paramName")}""/>";
                    }
                    return GetArgumentText("message");
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Extracts the argument value from literal or nameof expressions.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The argument value as a string.
        /// </returns>
        private static string ExtractArgumentValue(ICSharpArgument argument)
        {
            if (argument.Value is ICSharpLiteralExpression literal)
            {
                return literal.ConstantValue.Value?.ToString() ?? string.Empty;
            }
            if (argument.Value is IInvocationExpression expression && @"nameof".Equals(expression.InvocationExpressionReference.GetName()))
            {
                return expression.ConstantValue.Value?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets an argument by parameter name.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>
        /// The argument with that parameter name or <see langword="null" />.
        /// </returns>
        private ICSharpArgument GetArgumentByName(string paramName)
        {
            return _arguments.FirstOrDefault(a => paramName.Equals(GetParameterName(a)));
        }

        /// <summary>
        /// Gets the arguments of the exception.
        /// </summary>
        /// <returns>
        /// A collection of arguments.
        /// </returns>
        private ICollection<ICSharpArgument> GetArguments()
        {
            var expression = _statement.Exception as IObjectCreationExpression;
            if (expression != null)
            {
                return expression.ArgumentList.Arguments;
            }
            return new Collection<ICSharpArgument>();
        }

        /// <summary>
        /// Gets the argument text.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>
        /// The text of the argument with that parameter name.
        /// </returns>
        private string GetArgumentText(string paramName)
        {
            var argument = GetArgumentByName(paramName);
            return argument != null ? ExtractArgumentValue(argument) : string.Empty;
        }

        /// <summary>
        /// Gets the name of the parameter of the given argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The name of the parameter.
        /// </returns>
        private static string GetParameterName(ICSharpArgument argument)
        {
            return argument.MatchingParameter?.Element.ShortName ?? string.Empty;
        }

        /// <summary>
        /// Determines whether has parameter and message arguments.
        /// </summary>
        /// <returns>
        /// <c>true</c> if exception has parameter and message arguments; otherwise, <c>false</c>.
        /// </returns>
        private bool HasParamAndMessageArguments()
        {
            return GetArgumentByName(@"paramName") != null && GetArgumentByName(@"message") != null;
        }

        /// <summary>
        /// Determines whether the exception is of type specified by the CLR name.
        /// </summary>
        /// <param name="clrName">CLR name.</param>
        /// <returns>
        /// <c>true</c> if exception is of that type; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOfType(string clrName)
        {
            var expression = _statement.Exception as IObjectCreationExpression;
            return expression?.TypeName?.QualifiedName.Equals(clrName) == true;
        }

        #endregion
    }
}