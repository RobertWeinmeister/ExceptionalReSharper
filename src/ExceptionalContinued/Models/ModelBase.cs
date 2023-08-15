using JetBrains.DocumentModel;
using ReSharper.ExceptionalContinued.Analyzers;

namespace ReSharper.ExceptionalContinued.Models
{
    internal abstract class ModelBase
    {
        #region constructors and destructors

        protected ModelBase(IAnalyzeUnit analyzeUnit)
        {
            AnalyzeUnit = analyzeUnit;
        }

        #endregion

        #region methods

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public virtual void Accept(AnalyzerBase analyzer)
        {
        }

        #endregion

        #region properties

        /// <summary>Gets the analyze unit. </summary>
        public IAnalyzeUnit AnalyzeUnit { get; }

        /// <summary>Gets the document range of this object. </summary>
        public abstract DocumentRange DocumentRange { get; }

        #endregion
    }
}