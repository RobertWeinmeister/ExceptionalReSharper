using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Analyzers;

namespace ReSharper.ExceptionalContinued.Models.ExceptionsOrigins
{
    internal sealed class ConstructorInitializerModel : TreeElementModelBase<ITreeNode>,
                                                        IExceptionsOriginModel
    {
        #region constructors and destructors

        public ConstructorInitializerModel(IAnalyzeUnit analyzeUnit, IConstructorInitializer node, IBlockModel containingBlock) : base(analyzeUnit, node)
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = ThrownExceptionsReader.Read(analyzeUnit, this, node.Reference.Resolve().DeclaredElement);
        }

        public ConstructorInitializerModel(
            ConstructorDeclarationModel analyzeUnit,
            IDeclaredElement constructor,
            IBlockModel containingBlock) : base(analyzeUnit, analyzeUnit.Node.TypeName)
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = ThrownExceptionsReader.Read(analyzeUnit, this, constructor);
        }

        #endregion

        #region explicit interfaces

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownException in ThrownExceptions)
            {
                thrownException.Accept(analyzer);
            }
            base.Accept(analyzer);
        }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; }

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            throw new NotSupportedException();
        }

        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }

        #endregion
    }
}