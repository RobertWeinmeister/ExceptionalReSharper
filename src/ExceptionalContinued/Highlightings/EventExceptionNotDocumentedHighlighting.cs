using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Highlightings
{
    [RegisterConfigurableSeverity(
                                     Id,
                                     Constants.CompoundName,
                                     HighlightingGroupIds.BestPractice,
                                     "Exceptional.EventExceptionNotDocumented",
                                     "Exceptional.EventExceptionNotDocumented",
                                     Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class EventExceptionNotDocumentedHighlighting : ExceptionNotDocumentedHighlighting
    {
        #region constants

        public new const string Id = "EventExceptionNotDocumented";

        #endregion

        #region constructors and destructors

        /// <summary>Initializes a new instance of the <see cref="EventExceptionNotDocumentedHighlighting" /> class. </summary>
        /// <param name="thrownException">The thrown exception. </param>
        internal EventExceptionNotDocumentedHighlighting(ThrownExceptionModel thrownException) : base(thrownException)
        {
        }

        #endregion

        #region properties

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message => Resources.HighlightEventNotDocumentedExceptions;

        #endregion
    }
}