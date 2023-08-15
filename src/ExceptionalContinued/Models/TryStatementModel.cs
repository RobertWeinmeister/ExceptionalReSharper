using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.Models
{
    /// <summary>Describes a try statement. </summary>
    internal sealed class TryStatementModel : BlockModelBase<ITryStatement>
    {
        #region constructors and destructors

        public TryStatementModel(IAnalyzeUnit analyzeUnit, ITryStatement tryStatement) : base(analyzeUnit, tryStatement)
        {
            CatchClauses = GetCatchClauses();
        }

        #endregion

        #region methods

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer base. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            base.Accept(analyzer);
            foreach (var catchClauseModel in CatchClauses)
            {
                catchClauseModel.Accept(analyzer);
            }
        }

        /// <summary>Adds a catch clause to the try statement. </summary>
        /// <param name="exceptionType">The exception type in the added catch clause. </param>
        public void AddCatchClause(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var variableName = NameFactory.CatchVariableName(Node, exceptionType);
            var catchClauseNode = codeElementFactory.CreateSpecificCatchClause(exceptionType, null, variableName);
            Node.AddCatchClause(catchClauseNode);
        }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        public override bool CatchesException(IDeclaredType exception)
        {
            return CatchClauses.Any(catchClauseModel => catchClauseModel.Catches(exception)) || ParentBlock.CatchesException(exception);
        }

        /// <summary>Finds the nearest parent try statement which encloses this block. </summary>
        /// <returns>The try statement. </returns>
        public override TryStatementModel FindNearestTryStatement()
        {
            return this;
        }

        private List<CatchClauseModel> GetCatchClauses()
        {
            return Node.Catches.Select(c => new CatchClauseModel(c, this, AnalyzeUnit)).ToList();
        }

        #endregion

        #region properties

        /// <summary>Gets the list of catch clauses of the try statement. </summary>
        public List<CatchClauseModel> CatchClauses { get; }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content => Node.Try;

        /// <summary>Gets the list of not caught thrown exceptions. </summary>
        public override IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get
            {
                foreach (var throwStatementModel in base.UncaughtThrownExceptions)
                {
                    yield return throwStatementModel;
                }
                foreach (var model in CatchClauses.SelectMany(m => m.UncaughtThrownExceptions))
                {
                    yield return model;
                }
            }
        }

        #endregion
    }
}