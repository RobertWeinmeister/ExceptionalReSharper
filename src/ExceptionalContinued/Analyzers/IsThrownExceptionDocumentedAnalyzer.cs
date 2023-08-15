using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;
using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Models;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;
using ReSharper.ExceptionalContinued.Settings;

namespace ReSharper.ExceptionalContinued.Analyzers
{
    /// <summary>Analyzes throw statements and checks that exceptions thrown outside are documented.</summary>
    internal sealed class IsThrownExceptionDocumentedAnalyzer : AnalyzerBase
    {
        #region methods

        /// <summary>Performs analyze of <paramref name="thrownException" />.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public override void Visit(ThrownExceptionModel thrownException)
        {
            if (thrownException == null)
            {
                return;
            }
            if (!thrownException.AnalyzeUnit.IsInspectionRequired || thrownException.IsCaught || thrownException.IsExceptionDocumented)
            {
                return;
            }
            var isOptional = IsSubtypeDocumented(thrownException) || IsThrownExceptionSubclassOfOptionalException(thrownException)
                                                                  || IsThrownExceptionThrownFromExcludedMethod(thrownException)
                                                                  || thrownException.IsThrownFromAnonymousMethod;
            if (thrownException.IsEventInvocationException)
            {
                if (ServiceLocator.Settings.DelegateInvocationsMayThrowExceptions)
                {
                    var highlighting = new EventExceptionNotDocumentedHighlighting(thrownException);
                    ServiceLocator.StageProcess.AddHighlighting(highlighting, thrownException.DocumentRange);
                }
            }
            else
            {
                var highlighting = isOptional
                    ? new ExceptionNotDocumentedOptionalHighlighting(thrownException)
                    : new ExceptionNotDocumentedHighlighting(thrownException);
                ServiceLocator.StageProcess.AddHighlighting(highlighting, thrownException.DocumentRange);
            }
        }

        private bool IsSubtypeDocumented(ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement)
            {
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements && thrownException.IsExceptionOrSubtypeDocumented)
                {
                    return true;
                }
            }
            else
            {
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions
                 && thrownException.IsExceptionOrSubtypeDocumented)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsThrownExceptionSubclassOfOptionalException(ThrownExceptionModel thrownExceptionModel)
        {
            var optionalExceptions = ServiceLocator.Settings.GetOptionalExceptions();
            if (thrownExceptionModel.IsThrownFromThrowStatement)
            {
                return optionalExceptions.Any(
                    e => e.ReplacementType != OptionalExceptionReplacementType.InvocationOnly
                      && thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
            }
            return optionalExceptions.Any(
                e => e.ReplacementType != OptionalExceptionReplacementType.ThrowOnly && thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
        }

        private static bool IsThrownExceptionThrownFromExcludedMethod(ThrownExceptionModel thrownException)
        {
            if (!(thrownException.ExceptionsOrigin is ReferenceExpressionModel parent))
            {
                return false;
            }
            var node = parent.Node;
            var (declaredElement, _) = node.Reference.Resolve();
            {
                if (!(declaredElement is IXmlDocIdOwner element) || !(declaredElement is IMethod) && !(declaredElement is IProperty))
                {
                    return false;
                }
                // remove generic placeholders ("`1") and method signature ("(...)")
                var fullMethodName = Regex.Replace(element.XMLDocId.Substring(2), "(`+[0-9]+)|(\\(.*?\\))", ""); // TODO: merge with other
                var excludedMethods = ServiceLocator.Settings.GetOptionalMethodExceptions();
                return excludedMethods.Any(t => t.FullMethodName == fullMethodName && t.IsSuperTypeOf(thrownException));
            }
        }

        #endregion
    }
}