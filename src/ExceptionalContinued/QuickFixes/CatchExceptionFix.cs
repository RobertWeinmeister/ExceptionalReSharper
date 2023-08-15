using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.TextControl;
using ReSharper.ExceptionalContinued.Highlightings;

namespace ReSharper.ExceptionalContinued.QuickFixes
{
    [QuickFix]
    internal class CatchExceptionFix : SingleActionFix
    {
        #region constructors and destructors

        public CatchExceptionFix(ExceptionNotDocumentedOptionalHighlighting error)
        {
            Error = error;
        }

        #endregion

        #region methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var exceptionsOriginModel = Error.ThrownException.ExceptionsOrigin;
            var nearestTryBlock = exceptionsOriginModel.ContainingBlock.FindNearestTryStatement();
            if (nearestTryBlock == null)
            {
                exceptionsOriginModel.SurroundWithTryBlock(Error.ThrownException.ExceptionType);
            }
            else
            {
                nearestTryBlock.AddCatchClause(Error.ThrownException.ExceptionType);
            }
            return null;
        }

        #endregion

        #region properties

        public override string Text => string.Format(Resources.QuickFixCatchException, Error.ThrownException.ExceptionType.GetClrName().ShortName);

        private ExceptionNotDocumentedOptionalHighlighting Error { get; }

        #endregion
    }
}