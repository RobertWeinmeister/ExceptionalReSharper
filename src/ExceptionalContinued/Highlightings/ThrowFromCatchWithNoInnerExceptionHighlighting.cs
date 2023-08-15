using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;

namespace ReSharper.ExceptionalContinued.Highlightings
{
    [RegisterConfigurableSeverity(
                                     Id,
                                     Constants.CompoundName,
                                     HighlightingGroupIds.BestPractice,
                                     "Exceptional.ThrowFromCatchWithNoInnerException",
                                     "Exceptional.ThrowFromCatchWithNoInnerException",
                                     Severity.WARNING)]
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ThrowFromCatchWithNoInnerExceptionHighlighting : HighlightingBase
    {
        #region constants

        public const string Id = "ThrowFromCatchWithNoInnerException";

        #endregion

        #region constructors and destructors

        /// <summary>Initializes a new instance of the <see cref="ThrowFromCatchWithNoInnerExceptionHighlighting" /> class. </summary>
        /// <param name="throwStatement">The throw statement. </param>
        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowStatementModel throwStatement)
        {
            ThrowStatement = throwStatement;
        }

        /// <summary>Initializes a new instance of the <see cref="ThrowFromCatchWithNoInnerExceptionHighlighting" /> class. </summary>
        /// <param name="throwExpression">The throw expression. </param>
        internal ThrowFromCatchWithNoInnerExceptionHighlighting(ThrowExpressionModel throwExpression)
        {
            ThrowExpression = throwExpression;
        }

        #endregion

        #region properties

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message => Resources.HighlightThrowingFromCatchWithoutInnerException;

        /// <summary>Gets the throw expression. </summary>
        internal ThrowExpressionModel ThrowExpression { get; }

        /// <summary>Gets the throw statement. </summary>
        internal ThrowStatementModel ThrowStatement { get; }

        #endregion
    }
}