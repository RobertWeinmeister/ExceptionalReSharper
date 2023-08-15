using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharper.ExceptionalContinued.Highlightings
{
    [RegisterConfigurableSeverity(
                                     Id,
                                     Constants.CompoundName,
                                     HighlightingGroupIds.BestPractice,
                                     "Exceptional.CatchAllClause",
                                     "Exceptional.CatchAllClause",
                                     Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        #region constants

        public const string Id = "CatchAllClause";

        #endregion

        #region properties

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message => string.Format(Resources.HighlightCatchAllClauses);

        #endregion
    }
}