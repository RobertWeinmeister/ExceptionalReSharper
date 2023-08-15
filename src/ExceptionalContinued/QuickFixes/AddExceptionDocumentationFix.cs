using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReSharper.ExceptionalContinued.Highlightings;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.QuickFixes
{
    //[QuickFix(null, BeforeOrAfter.Before)]
    [QuickFix]
    internal class AddExceptionDocumentationFix : SingleActionFix
    {
        #region constructors and destructors

        public AddExceptionDocumentationFix(ExceptionNotDocumentedOptionalHighlighting error)
        {
            Error = error;
        }

        #endregion

        #region methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = Error.ThrownException.AnalyzeUnit;
            var insertedExceptionModel = methodDeclaration.DocumentationBlock.AddExceptionDocumentation(Error.ThrownException, progress);
            return insertedExceptionModel == null ? null : MarkInsertedDescription(solution, insertedExceptionModel);
        }

        private static Action<ITextControl> MarkInsertedDescription(ISolution solution, ExceptionDocCommentModel insertedExceptionModel)
        {
            var exceptionCommentRange = insertedExceptionModel.GetMarkerRange();
            if (exceptionCommentRange == DocumentRange.InvalidRange)
            {
                return null;
            }
            var copyExceptionDescription = string.IsNullOrEmpty(insertedExceptionModel.ExceptionDescription) || insertedExceptionModel.ExceptionDescription.Contains("[MARKER]");
            var exceptionDescription = copyExceptionDescription ? "Condition" : insertedExceptionModel.ExceptionDescription.Trim();
            var nameSuggestionsExpression = new NameSuggestionsExpression(new[] { exceptionDescription });
            var field = new TemplateField("name", nameSuggestionsExpression, 0);
            var fieldInfo = new HotspotInfo(field, exceptionCommentRange);
            return textControl =>
            {
                var hotspotSession = Shell.Instance.GetComponent<LiveTemplatesManager>().CreateHotspotSessionAtopExistingText(
                    solution,
                    DocumentRange.InvalidRange,
                    textControl,
                    LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                    fieldInfo);
                hotspotSession.ExecuteAndForget();
            };
        }

        #endregion

        #region properties

        public override string Text => string.Format(Resources.QuickFixInsertExceptionDocumentation, Error.ThrownException.ExceptionType.GetClrName().ShortName);

        private ExceptionNotDocumentedOptionalHighlighting Error { get; }

        #endregion
    }
}