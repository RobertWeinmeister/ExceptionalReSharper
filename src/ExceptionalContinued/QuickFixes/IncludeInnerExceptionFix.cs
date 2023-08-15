using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.QuickFixes
{
    [QuickFix]
    internal class IncludeInnerExceptionFix : SingleActionFix
    {
        #region constructors and destructors

        public IncludeInnerExceptionFix(ThrowFromCatchWithNoInnerExceptionHighlighting error)
        {
            Error = error;
        }

        #endregion

        #region methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            return Error.ThrowStatement != null ? ExecutePsiTransactionForStatement() : ExecutePsiTransactionForExpression();
        }

        private Action<ITextControl> ExecutePsiTransactionForExpression()
        {
            var throwExpressionModel = Error.ThrowExpression;
            var outerCatchClause = throwExpressionModel.FindOuterCatchClause();
            string variableName;
            if (outerCatchClause.Node is ISpecificCatchClause specificCatchClause)
            {
                variableName = specificCatchClause.ExceptionDeclaration?.DeclaredName;
            }
            else
            {
                variableName = NameFactory.CatchVariableName(outerCatchClause.Node, outerCatchClause.CaughtException);
            }
            if (outerCatchClause.Node is ISpecificCatchClause)
            {
                outerCatchClause.AddCatchVariable(variableName);
                throwExpressionModel.AddInnerException(variableName);
            }
            else
            {
                throwExpressionModel.AddInnerException(variableName);
                outerCatchClause.AddCatchVariable(variableName);
            }
            return null;
        }

        private Action<ITextControl> ExecutePsiTransactionForStatement()
        {
            var throwStatementModel = Error.ThrowStatement;
            var outerCatchClause = throwStatementModel.FindOuterCatchClause();
            string variableName;
            if (outerCatchClause.Node is ISpecificCatchClause specificCatchClause)
            {
                variableName = specificCatchClause.ExceptionDeclaration?.DeclaredName;
            }
            else
            {
                variableName = NameFactory.CatchVariableName(outerCatchClause.Node, outerCatchClause.CaughtException);
            }
            if (outerCatchClause.Node is ISpecificCatchClause)
            {
                outerCatchClause.AddCatchVariable(variableName);
                throwStatementModel.AddInnerException(variableName);
            }
            else
            {
                throwStatementModel.AddInnerException(variableName);
                outerCatchClause.AddCatchVariable(variableName);
            }
            return null;
        }

        #endregion

        #region properties

        public override string Text => Resources.QuickFixIncludeInnerException;

        private ThrowFromCatchWithNoInnerExceptionHighlighting Error { get; }

        #endregion
    }
}