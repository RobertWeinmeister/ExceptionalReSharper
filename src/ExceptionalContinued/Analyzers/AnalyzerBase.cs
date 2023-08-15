using ReSharper.ExceptionalContinued.Models;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;

namespace ReSharper.ExceptionalContinued.Analyzers
{
    /// <summary>A base class for all analyzers.</summary>
    internal abstract class AnalyzerBase
    {
        #region methods

        /// <summary>Performs analyze of throw <paramref name="throwStatement" />.</summary>
        /// <param name="throwStatement">Throw statement model to analyze.</param>
        public virtual void Visit(ThrowStatementModel throwStatement)
        {
        }

        /// <summary>Performs analyze of throw <paramref name="throwExpression" />.</summary>
        /// <param name="throwExpression">Throw expression model to analyze.</param>
        public virtual void Visit(ThrowExpressionModel throwExpression)
        {
        }

        /// <summary>Performs analyze of <paramref name="catchClause" />.</summary>
        /// <param name="catchClause">Catch clause to analyze.</param>
        public virtual void Visit(CatchClauseModel catchClause)
        {
        }

        /// <summary>Performs analyze of <paramref name="exceptionDocumentation" />.</summary>
        /// <param name="exceptionDocumentation">Exception documentation to analyze.</param>
        public virtual void Visit(ExceptionDocCommentModel exceptionDocumentation)
        {
        }

        /// <summary>Performs analyze of <paramref name="thrownException" />.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public virtual void Visit(ThrownExceptionModel thrownException)
        {
        }

        #endregion
    }
}