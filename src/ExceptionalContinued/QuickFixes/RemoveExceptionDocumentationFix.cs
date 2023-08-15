using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.TextControl;
using ReSharper.ExceptionalContinued.Highlightings;

namespace ReSharper.ExceptionalContinued.QuickFixes
{
    [QuickFix]
    internal class RemoveExceptionDocumentationFix : SingleActionFix
    {
        #region constructors and destructors

        public RemoveExceptionDocumentationFix(ExceptionNotThrownHighlighting error)
        {
            Error = error;
        }

        #endregion

        #region methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var docCommentModel = Error.ExceptionDocumentation.AnalyzeUnit.DocumentationBlock;
            docCommentModel.RemoveExceptionDocumentation(Error.ExceptionDocumentation, progress);
            return null;
        }

        #endregion

        #region properties

        public override string Text => Resources.QuickFixRemoveExceptionDocumentation;

        private ExceptionNotThrownHighlighting Error { get; }

        #endregion
    }
}