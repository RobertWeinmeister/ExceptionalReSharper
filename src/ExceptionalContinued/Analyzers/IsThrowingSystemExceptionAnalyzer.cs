using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Analyzers
{
    /// <summary>
    /// Analyzes whether a throw statement throws System.Exception.
    /// </summary>
    internal sealed class IsThrowingSystemExceptionAnalyzer : AnalyzerBase
    {
        #region methods

        /// <summary>
        /// Performs analyze of <paramref name="thrownException" />.
        /// </summary>
        /// <param name="thrownException">
        /// Thrown exception to analyze.
        /// </param>
        public override void Visit(ThrownExceptionModel thrownException)
        {
            // add a squiggle if the throwing a Exception (new Exception())
            // throw; statements are ignored
            if (!thrownException.IsThrownFromThrowStatement || thrownException.ShortName != "Exception" || thrownException.IsRethrow)
            {
                return;
            }
            var highlight = new ThrowingSystemExceptionHighlighting();
            var range = thrownException.DocumentRange;
            ServiceLocator.StageProcess.AddHighlighting(highlight, range);
        }

        #endregion
    }
}