using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using ReSharper.ExceptionalContinued.Analyzers;

namespace ReSharper.ExceptionalContinued.Models.ExceptionsOrigins
{
    internal class ReferenceExpressionModel : ExpressionExceptionsOriginModelBase<IReferenceExpression>
    {
        #region member vars

        private IAssignmentExpression _assignment;
        private bool? _isEventInvocation;
        private bool? _isInvocation;
        private IEnumerable<ThrownExceptionModel> _thrownExceptions;

        #endregion

        #region constructors and destructors

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock) : base(
            analyzeUnit,
            invocationExpression,
            containingBlock)
        {
        }

        #endregion

        #region methods

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
            {
                thrownExceptionModel.Accept(analyzer);
            }
        }

        public bool IsExceptionValid(ExceptionDocCommentModel comment)
        {
            if (comment.Accessor.IsNullOrWhitespace())
            {
                return true;
            }
            if (IsInvocation == false)
            {
                return false;
            }
            if (_assignment != null)
            {
                var exceptionOrigin = comment.AssociatedExceptionModel.ExceptionsOrigin.Node.GetText().TrimFromStart("this.");
                if (_assignment.Dest.LastChild == null)
                {
                    return true;
                }
                var assignmentDestination = _assignment.Dest.LastChild.GetText();
                if (assignmentDestination.Contains(exceptionOrigin) && comment.Accessor == "get")
                {
                    return false;
                }
                if (assignmentDestination.Contains(exceptionOrigin) == false && comment.Accessor == "set")
                {
                    return false;
                }
            }
            return true;
        }

        private ThrownExceptionModel CreateThrownSystemException()
        {
            var psiModule = Node.GetPsiModule();
            var systemExceptionType = TypeFactory.CreateTypeByCLRName("System.Exception", psiModule);
            string accessor = null;
            if (ContainingBlock is AccessorDeclarationModel)
            {
                accessor = ((AccessorDeclarationModel)ContainingBlock).Node.NameIdentifier.Name;
            }
            var thrownException = new ThrownExceptionModel(AnalyzeUnit, this, systemExceptionType, "A delegate callback throws an exception.", true, accessor);
            return thrownException;
        }

        private bool IsInvocationInternal()
        {
            var parent = Node.Parent;
            while (parent != null)
            {
                if (parent == AnalyzeUnit.Node)
                {
                    return false;
                }
                switch (parent)
                {
                    case IAssignmentExpression _:
                    case ICSharpArgument _:
                    case IExpressionInitializer _:
                    case IAccessorDeclaration _:
                    {
                        var property = Node.Reference.Resolve().DeclaredElement as IProperty;
                        if (parent is IAssignmentExpression assignmentExpression)
                        {
                            _assignment = assignmentExpression;
                        }
                        if (property == null)
                        {
                            return false;
                        }
                        var psiModule = Node.GetPsiModule();
                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule);
                        return !property.Type.IsSubtypeOf(delegateType);
                    }
                    case IElementAccessExpression _:
                    case IInvocationExpression _:
                        return true;
                    default:
                        parent = parent.Parent;
                        break;
                }
            }
            return false;
        }

        #endregion

        #region properties

        /// <summary>Gets the document range of this block. </summary>
        public override DocumentRange DocumentRange
        {
            get
            {
                if (Node.Parent is IElementAccessExpression)
                {
                    return Node.Parent.GetExtendedDocumentRange();
                }

                //if (Node.Parent is IAssignmentExpression)
                //    return Node.Parent.GetExtendedDocumentRange();
                return Node.Reference.GetDocumentRange();
            }
        }

        /// <summary>Gets a value indicating whether this is an delegate invocation. </summary>
        public bool IsDelegateInvocation
        {
            get
            {
                if (!_isEventInvocation.HasValue)
                {
                    if (!(Node.Parent is IAssignmentExpression) && IsInvocation)
                    {
                        var psiModule = Node.GetPsiModule();
                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule);
                        var type = Node.GetExpressionType().ToIType();
                        if (type != null)
                        {
                            _isEventInvocation = type.IsSubtypeOf(delegateType);
                        }
                        else
                        {
                            _isEventInvocation = false;
                        }
                    }
                    else
                    {
                        _isEventInvocation = false;
                    }
                }
                return _isEventInvocation.Value;
            }
        }

        /// <summary>Gets a value indicating whether this is a method, event or property invocation. </summary>
        public bool IsInvocation
        {
            get
            {
                if (!_isInvocation.HasValue)
                {
                    _isInvocation = IsInvocationInternal();
                }
                return _isInvocation.Value;
            }
        }

        /// <summary>
        /// Gets a list of exceptions which may be thrown from this reference expression (empty if
        /// <see cref="IsInvocation" /> is false).
        /// </summary>
        public override IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get
            {
                if (_thrownExceptions == null)
                {
                    if (IsDelegateInvocation)
                    {
                        var thrownException = CreateThrownSystemException();
                        _thrownExceptions = new List<ThrownExceptionModel>
                        {
                            thrownException
                        };
                    }
                    else if (IsInvocation)
                    {
                        _thrownExceptions = ThrownExceptionsReader.Read(AnalyzeUnit, this, Node);
                    }
                    else
                    {
                        _thrownExceptions = new List<ThrownExceptionModel>();
                    }
                }
                return _thrownExceptions;
            }
        }

        #endregion
    }
}