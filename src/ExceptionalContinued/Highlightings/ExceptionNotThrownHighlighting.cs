using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Highlightings
{
    [RegisterConfigurableSeverity(
                                     Id,
                                     Constants.CompoundName,
                                     HighlightingGroupIds.BestPractice,
                                     "Exceptional.ExceptionNotThrown",
                                     "Exceptional.ExceptionNotThrown",
                                     Severity.WARNING)]
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class ExceptionNotThrownHighlighting : ExceptionNotThrownOptionalHighlighting
    {
        #region constants

        public new const string Id = "ExceptionNotThrown";

        #endregion

        #region constructors and destructors

        /// <summary>Initializes a new instance of the <see cref="ExceptionNotThrownHighlighting" /> class. </summary>
        /// <param name="exceptionDocumentation">The exception documentation. </param>
        internal ExceptionNotThrownHighlighting(ExceptionDocCommentModel exceptionDocumentation) : base(exceptionDocumentation)
        {
        }

        #endregion

        #region properties

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message =>
            string.Format(Resources.HighlightNotThrownDocumentedExceptions, ExceptionDocumentation.ExceptionType.GetClrName().ShortName);

        #endregion
    }
}