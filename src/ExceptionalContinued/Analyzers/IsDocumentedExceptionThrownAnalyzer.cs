using System.Linq;
using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Analyzers
{
    /// <summary>Analyzes an exception documentation and checks if it is thrown from the documented element.</summary>
    internal sealed class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
        #region methods

        /// <summary>Performs analyze of <paramref name="exceptionDocumentation" />.</summary>
        /// <param name="exceptionDocumentation">Exception documentation to analyze.</param>
        public override void Visit(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation == null)
            {
                return;
            }
            if (IsAbstractOrInterfaceMethod(exceptionDocumentation))
            {
                return;
            }
            if (!exceptionDocumentation.AnalyzeUnit.IsInspectionRequired)
            {
                return;
            }
            if (IsDocumentedExceptionThrown(exceptionDocumentation))
            {
                return;
            }
            var isOptional = IsDocumentedExceptionOrSubtypeThrown(exceptionDocumentation);
            var highlighting = isOptional
                ? new ExceptionNotThrownOptionalHighlighting(exceptionDocumentation)
                : new ExceptionNotThrownHighlighting(exceptionDocumentation);
            ServiceLocator.StageProcess.AddHighlighting(highlighting, exceptionDocumentation.DocumentRange);
        }

        private static bool IsAbstractOrInterfaceMethod(ModelBase exceptionDocumentation)
        {
            var declaredElement = (exceptionDocumentation.AnalyzeUnit as MethodDeclarationModel)?.Node.DeclaredElement;
            return declaredElement?.IsAbstract == true;
        }

        private static bool IsDocumentedExceptionOrSubtypeThrown(ExceptionDocCommentModel exceptionDocumentation)
        {
            return exceptionDocumentation.AnalyzeUnit.UncaughtThrownExceptions.Any(m => ThrowsExceptionOrSubtype(exceptionDocumentation, m));
        }

        private static bool IsDocumentedExceptionThrown(ExceptionDocCommentModel exceptionDocumentation)
        {
            return exceptionDocumentation.AnalyzeUnit.UncaughtThrownExceptions.Any(m => m.IsException(exceptionDocumentation));
        }

        private static bool ThrowsExceptionOrSubtype(ExceptionDocCommentModel exceptionDocumentation, ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement)
            {
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements)
                {
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation);
                }
            }
            else
            {
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions)
                {
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation);
                }
            }
            return false;
        }

        #endregion
    }
}