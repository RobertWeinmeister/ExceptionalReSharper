using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Analyzers
{
    /// <summary>Analyzes a catch clause and checks if it is not catch-all clause.</summary>
    internal sealed class CatchAllClauseAnalyzer : AnalyzerBase
    {
        /// <summary>Performs analyze of <paramref name="catchClause" />.</summary>
        /// <param name="catchClause">Catch clause to analyze.</param>
        public override void Visit(CatchClauseModel catchClause)
        {
            if (!catchClause.AnalyzeUnit.IsInspectionRequired)
            {
                return;
            }

            if (catchClause.IsCatchAll)
            {
                ServiceLocator.StageProcess.AddHighlighting(new CatchAllClauseHighlighting(),
                                                            catchClause.DocumentRange);
            }
        }
    }
}