using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.Models.ExceptionsOrigins
{
    internal abstract class ExpressionExceptionsOriginModelBase<T> : TreeElementModelBase<T>,
                                                                     IExceptionsOriginModel
        where T : ICSharpExpression
    {
        #region constructors and destructors

        protected ExpressionExceptionsOriginModelBase(IAnalyzeUnit analyzeUnit, T node, IBlockModel containingBlock) : base(analyzeUnit, node)
        {
            ContainingBlock = containingBlock;
        }

        #endregion

        #region explicit interfaces

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; }

        /// <summary>Gets the node. </summary>
        ITreeNode IExceptionsOriginModel.Node => Node;

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var containingStatement = Node.GetContainingStatement();
            if (containingStatement?.LastChild == null)
            {
                return false;
            }
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName, Node);
            var spaces = GetElementFactory().CreateWhitespaces(Environment.NewLine);
            LowLevelModificationUtil.AddChildAfter(containingStatement.LastChild, spaces[0]);
            var block = codeElementFactory.CreateBlock(containingStatement);
            tryStatement.SetTry(block);
            containingStatement.ReplaceBy(tryStatement);
            return true;
        }

        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        public abstract IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }

        #endregion
    }
}