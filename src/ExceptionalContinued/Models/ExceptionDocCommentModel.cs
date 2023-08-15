using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharper.ExceptionalContinued.Analyzers;

namespace ReSharper.ExceptionalContinued.Models
{
    internal class ExceptionDocCommentModel : ModelBase
    {
        #region constructors and destructors

        public ExceptionDocCommentModel(DocCommentBlockModel documentationBlock, string exceptionType, string exceptionDescription, string accessor) : base(
            documentationBlock.AnalyzeUnit)
        {
            DocumentationBlock = documentationBlock;
            Accessor = accessor;
            ExceptionTypeName = exceptionType;
            ExceptionType = GetExceptionType(exceptionType);
            ExceptionDescription = exceptionDescription;
        }

        #endregion

        #region methods

        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
        }

        public DocumentRange GetMarkerRange()
        {
            var text = DocumentationBlock.Node.GetText();
            if (text.Contains(Constants.ExceptionDescriptionMarker))
            {
                var documentRange = DocumentationBlock.Node.GetDocumentRange();
                var textRange = documentRange.TextRange;
                var index = text.IndexOf(Constants.ExceptionDescriptionMarker, StringComparison.InvariantCulture);
                var startOffset = textRange.StartOffset + index;
                var endOffset = startOffset + 8;
                var newTextRange = new TextRange(startOffset, endOffset);
                return new DocumentRange(documentRange.Document, newTextRange);
            }
            return DocumentRange.InvalidRange;
        }

        private DocumentRange GetCommentRange()
        {
            var text = DocumentationBlock.Node.GetText();
            var documentRange = DocumentationBlock.DocumentRange;
            var textRange = documentRange.TextRange;

            // TODO: Improve range finding
            var tagStart = "<exception cref=\"";
            var xml = tagStart + ExceptionTypeName + "\"";
            if (Accessor != null)
            {
                xml += " accessor=\"" + Accessor + "\"";
            }
            var index = text.IndexOf(xml, StringComparison.InvariantCulture);
            var startOffset = textRange.StartOffset + index + tagStart.Length;
            var endOffset = startOffset + ExceptionTypeName.Length;
            var newTextRange = new TextRange(startOffset, endOffset);
            return new DocumentRange(documentRange.Document, newTextRange);
        }

        private IDeclaredType GetExceptionType(string exceptionType)
        {
            var exceptionReference = DocumentationBlock.References.Find(reference => reference.GetName().Equals(exceptionType));
            var psiModule = DocumentationBlock.Node.GetPsiModule();
            if (exceptionReference == null)
            {
                return TypeFactory.CreateTypeByCLRName(exceptionType, psiModule);
            }
            var resolveResult = exceptionReference.Resolve();
            var declaredType = resolveResult.DeclaredElement as ITypeElement;
            if (declaredType == null)
            {
                return TypeFactory.CreateTypeByCLRName(exceptionType, psiModule);
            }
            return TypeFactory.CreateType(declaredType);
        }

        #endregion

        #region properties

        public string Accessor { get; set; }

        public ThrownExceptionModel AssociatedExceptionModel { get; set; }

        public DocCommentBlockModel DocumentationBlock { get; }

        public override DocumentRange DocumentRange => GetCommentRange();

        public string ExceptionDescription { get; }

        public IDeclaredType ExceptionType { get; }

        public string ExceptionTypeName { get; }

        #endregion
    }
}