using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;

namespace ReSharper.ExceptionalContinued.Models
{
    internal class ThrownExceptionModel : ModelBase
    {
        #region member vars

        private bool? _isCaught;
        private bool? _isExceptionDocumented;
        private bool? _isExceptionOrSubtypeDocumented;
        private bool? _isThrownFromAnonymousMethod;
        private bool? _isWrongAccessor;

        #endregion

        #region constructors and destructors

        public ThrownExceptionModel(
            IAnalyzeUnit analyzeUnit,
            IExceptionsOriginModel exceptionsOrigin,
            IDeclaredType exceptionType,
            string exceptionDescription,
            bool isEventInvocationException,
            string exceptionAccessor) : base(analyzeUnit)
        {
            ExceptionType = exceptionType;
            ExceptionDescription = exceptionDescription;
            ExceptionsOrigin = exceptionsOrigin;
            ExceptionAccessor = exceptionAccessor;
            IsEventInvocationException = isEventInvocationException;
            CheckAccessorOverride(exceptionsOrigin, exceptionType);
        }

        #endregion

        #region methods

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
        }

        public bool IsException(IDeclaredType exceptionType)
        {
            if (ExceptionType == null)
            {
                return false;
            }
            if (exceptionType == null)
            {
                return false;
            }
            return ExceptionType.Equals(exceptionType);
        }

        /// <summary>Checks whether the thrown exception is the same as <paramref name="exceptionDocumentation" />.</summary>
        public bool IsException(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != ExceptionAccessor)
            {
                return false;
            }
            return IsException(exceptionDocumentation.ExceptionType);
        }

        public bool IsExceptionOrSubtype(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != ExceptionAccessor)
            {
                return false;
            }
            if (ExceptionType == null)
            {
                return false;
            }
            if (exceptionDocumentation.ExceptionType == null)
            {
                return false;
            }
            return ExceptionType.IsSubtypeOf(exceptionDocumentation.ExceptionType);
        }

        private void CheckAccessorOverride(IExceptionsOriginModel exceptionsOrigin, IDeclaredType exceptionType)
        {
            var doc = GetXmlDocId(exceptionsOrigin.Node);
            if (doc == null)
            {
                return;
            }
            var fullMethodName = Regex.Replace(doc.Substring(2), "(`[0-9]+)|(\\(.*?\\))", ""); // TODO: merge with other
            var overrides = ServiceLocator.Settings.GetExceptionAccessorOverrides();
            var ov = overrides.SingleOrDefault(o => o.FullMethodName == fullMethodName && o.GetExceptionType().Equals(exceptionType));
            if (ov != null)
            {
                ExceptionAccessor = ov.ExceptionAccessor;
            }
        }

        private static string GetXmlDocId(ITreeNode node)
        {
            switch (node)
            {
                case IReferenceExpression referenceExpression:
                {
                    var element = referenceExpression.Reference.Resolve().DeclaredElement;
                    if (referenceExpression.Parent is IElementAccessExpression expression)
                    {
                        var elementAccessReference = expression.ElementAccessReference;
                        var declaredElement = elementAccessReference.Resolve().DeclaredElement;
                        if (declaredElement is IXmlDocIdOwner xmlDocIdOwner)
                        {
                            return xmlDocIdOwner.XMLDocId;
                        }
                    }
                    if (element is IXmlDocIdOwner t)
                    {
                        return t.XMLDocId;
                    }
                    return null;
                }
                case IPropertyDeclaration propertyDeclaration:
                    return propertyDeclaration.DeclaredElement?.XMLDocId;
                case IIndexerDeclaration indexerDeclaration:
                    return indexerDeclaration.DeclaredElement?.XMLDocId;
                case IMethodDeclaration methodDeclaration when methodDeclaration.DeclaredElement != null:
                    return methodDeclaration.DeclaredElement.XMLDocId;
                default:
                    return null;
            }
        }

        private bool IsExceptionDocumentedInternal(Func<ExceptionDocCommentModel, bool> check)
        {
            var docCommentBlockNode = AnalyzeUnit.DocumentationBlock;
            if (docCommentBlockNode != null)
            {
                return IsWrongAccessor || docCommentBlockNode.DocumentedExceptions.Any(check);
            }
            return false;
        }

        private static bool IsParentAnonymousMethodExpression(ITreeNode parent)
        {
            while (parent != null)
            {
                if (parent is IAnonymousMethodExpression || parent is IAnonymousFunctionExpression)
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        #endregion

        #region properties

        /// <summary>Gets the document range of this object. </summary>
        public override DocumentRange DocumentRange => ExceptionsOrigin.DocumentRange;

        public string ExceptionAccessor { get; private set; }

        public string ExceptionDescription { get; set; }

        public IExceptionsOriginModel ExceptionsOrigin { get; }

        public IDeclaredType ExceptionType { get; }

        public string ShortName => ExceptionType.GetClrName().ShortName;

        public bool IsCaught
        {
            get
            {
                if (!_isCaught.HasValue)
                {
                    _isCaught = ExceptionType != null && ExceptionsOrigin.ContainingBlock.CatchesException(ExceptionType);
                }
                return _isCaught.Value;
            }
        }

        public bool IsEventInvocationException { get; set; }

        /// <summary>Gets a value indicating whether the exact same exception is documented. </summary>
        public bool IsExceptionDocumented
        {
            get
            {
                if (!_isExceptionDocumented.HasValue)
                {
                    _isExceptionDocumented = IsExceptionDocumentedInternal(IsException);
                }
                return _isExceptionDocumented.Value;
            }
        }

        /// <summary>Gets a value indicating whether the exact same exception or a subtype is documented. </summary>
        public bool IsExceptionOrSubtypeDocumented
        {
            get
            {
                if (!_isExceptionOrSubtypeDocumented.HasValue)
                {
                    _isExceptionOrSubtypeDocumented = IsExceptionDocumentedInternal(IsExceptionOrSubtype);
                }
                return _isExceptionOrSubtypeDocumented.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the thrown exception is coming from a re-throw statement.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a re-throw; otherwise, <c>false</c>.
        /// </value>
        public bool IsRethrow => (ExceptionsOrigin as ThrowStatementModel)?.IsRethrow ?? false;

        public bool IsThrownFromAnonymousMethod
        {
            get
            {
                if (!_isThrownFromAnonymousMethod.HasValue)
                {
                    var parent = ExceptionsOrigin.Node;
                    _isThrownFromAnonymousMethod = IsParentAnonymousMethodExpression(parent);
                }
                return _isThrownFromAnonymousMethod.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this exception is thrown from a throw statement.
        /// </summary>
        public bool IsThrownFromThrowStatement => ExceptionsOrigin is ThrowStatementModel;

        private bool IsWrongAccessor
        {
            get
            {
                if (_isWrongAccessor != null)
                {
                    return _isWrongAccessor.Value;
                }
                var parent = ExceptionsOrigin.Node.Parent;
                switch (ExceptionAccessor)
                {
                    // property
                    case "get" when parent is IAssignmentExpression && parent.FirstChild == ExceptionsOrigin.Node:
                    // indexer
                    case "set" when parent is IExpressionInitializer && parent.LastChild == ExceptionsOrigin.Node:
                        _isWrongAccessor = true;
                        break;
                    case "get" when parent is IElementAccessExpression:
                    {
                        parent = parent.Parent;
                        while (parent != null && parent.FirstChild == parent.LastChild)
                        {
                            parent = parent.Parent;
                        }
                        if (parent?.FirstChild?.Children().Contains(ExceptionsOrigin.Node) == true)
                        {
                            _isWrongAccessor = true;
                        }
                        break;
                    }
                    case "set" when parent is IElementAccessExpression:
                    {
                        parent = parent.Parent;
                        while (parent != null && parent.FirstChild == parent.LastChild)
                        {
                            parent = parent.Parent;
                        }
                        if (parent?.LastChild?.LastChild?.Children().Contains(ExceptionsOrigin.Node) == true)
                        {
                            _isWrongAccessor = true;
                        }
                        break;
                    }
                }
                if (_isWrongAccessor == null)
                {
                    _isWrongAccessor = false;
                }
                return _isWrongAccessor.Value;
            }
        }

        #endregion
    }
}