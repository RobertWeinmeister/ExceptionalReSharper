using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Contexts
{
    internal interface IProcessContext
    {
        #region methods

        void EnterAccessor(IAccessorDeclaration accessorDeclarationNode);

        void EnterCatchClause(ICatchClause catchClauseNode);

        void EnterTryBlock(ITryStatement tryStatement);

        void LeaveAccessor();

        void LeaveCatchClause();

        void LeaveTryBlock();

        void Process(IThrowStatement throwStatement);

        void Process(ICatchVariableDeclaration catchVariableDeclaration);

        void Process(IReferenceExpression invocationExpression);

        void Process(IObjectCreationExpression objectCreationExpression);

        void Process(IDocCommentBlock docCommentBlockNode);

        void Process(IThrowExpression throwExpression);

        void RunAnalyzers();

        void StartProcess(IAnalyzeUnit analyzeUnit);

        #endregion

        #region properties

        IAnalyzeUnit Model { get; }

        #endregion
    }
}