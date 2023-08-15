using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.Logging;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Models;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;

namespace ReSharper.ExceptionalContinued.Contexts
{
    internal abstract class ProcessContext<T> : IProcessContext
        where T : IAnalyzeUnit
    {
        #region member vars

        private readonly Stack<CatchClauseModel> _catchClauseModelsStack;
        private readonly Stack<TryStatementModel> _tryStatementModelsStack;

        #endregion

        #region constructors and destructors

        protected ProcessContext()
        {
            _tryStatementModelsStack = new Stack<TryStatementModel>();
            _catchClauseModelsStack = new Stack<CatchClauseModel>();
            BlockModelsStack = new Stack<IBlockModel>();
        }

        #endregion

        #region explicit interfaces

        public virtual void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
        }

        public void EnterCatchClause(ICatchClause catchClauseNode)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (catchClauseNode == null)
            {
                return;
            }
            Logger.Assert(_tryStatementModelsStack.Count > 0, "[Exceptional] There is no try statement for catch declaration.");
            var tryStatementModel = _tryStatementModelsStack.Peek();
            var model = tryStatementModel.CatchClauses.Find(catchClauseModel => catchClauseModel.Node.Equals(catchClauseNode));
            Logger.Assert(model != null, "[Exceptional] Cannot find catch model!");
            _catchClauseModelsStack.Push(model);
            BlockModelsStack.Push(model);
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (tryStatement == null)
            {
                return;
            }
            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for try statement.");
            var model = new TryStatementModel(AnalyzeUnit, tryStatement);
            var blockModel = BlockModelsStack.Peek();
            blockModel.TryStatements.Add(model);
            model.ParentBlock = blockModel;
            _tryStatementModelsStack.Push(model);
            BlockModelsStack.Push(model);
        }

        public virtual void LeaveAccessor()
        {
        }

        public void LeaveCatchClause()
        {
            _catchClauseModelsStack.Pop();
            BlockModelsStack.Pop();
        }

        public void LeaveTryBlock()
        {
            _tryStatementModelsStack.Pop();
            BlockModelsStack.Pop();
        }

        IAnalyzeUnit IProcessContext.Model => Model;

        public void Process(IThrowStatement throwStatement)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (throwStatement == null)
            {
                return;
            }
            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for throw statement.");
            var containingBlockModel = BlockModelsStack.Peek();
            containingBlockModel.ThrownExceptions.Add(new ThrowStatementModel(AnalyzeUnit, throwStatement, containingBlockModel));
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (catchVariableDeclaration == null)
            {
                return;
            }
            Logger.Assert(_catchClauseModelsStack.Count > 0, "[Exceptional] There is no catch clause for catch variable declaration.");
            var catchClause = _catchClauseModelsStack.Peek();
            catchClause.Variable = new CatchVariableModel(AnalyzeUnit, catchVariableDeclaration);
        }

        public void Process(IReferenceExpression invocationExpression)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (invocationExpression == null)
            {
                return;
            }
            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for invocation statement.");
            var containingBlockModel = BlockModelsStack.Peek();
            containingBlockModel.ThrownExceptions.Add(new ReferenceExpressionModel(AnalyzeUnit, invocationExpression, containingBlockModel));
        }

        public void Process(IObjectCreationExpression objectCreationExpression)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (objectCreationExpression == null)
            {
                return;
            }
            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for invocation statement.");
            var containingBlockModel = BlockModelsStack.Peek();
            containingBlockModel.ThrownExceptions.Add(new ObjectCreationExpressionModel(AnalyzeUnit, objectCreationExpression, containingBlockModel));
        }

        public void Process(IDocCommentBlock docCommentBlockNode)
        {
            if (IsValid() == false)
            {
                return;
            }
            AnalyzeUnit.DocumentationBlock = new DocCommentBlockModel(AnalyzeUnit, docCommentBlockNode);
        }

        public void Process(IThrowExpression throwExpression)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (throwExpression == null)
            {
                return;
            }
            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for throw expression.");
            var containingBlockModel = BlockModelsStack.Peek();
            containingBlockModel.ThrownExceptions.Add(new ThrowExpressionModel(AnalyzeUnit, throwExpression, containingBlockModel));
        }

        public void RunAnalyzers()
        {
            if (IsValid() == false)
            {
                return;
            }
            foreach (var analyzerBase in ProvideAnalyzers())
            {
                AnalyzeUnit.Accept(analyzerBase);
            }
        }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
            AnalyzeUnit = analyzeUnit;
            Model = (T)analyzeUnit;
            BlockModelsStack.Push(AnalyzeUnit);
        }

        #endregion

        #region methods

        protected bool IsValid()
        {
            return AnalyzeUnit != null;
        }

        private static IEnumerable<AnalyzerBase> ProvideAnalyzers()
        {
            yield return new IsThrownExceptionDocumentedAnalyzer();
            yield return new IsDocumentedExceptionThrownAnalyzer();
            yield return new CatchAllClauseAnalyzer();
            yield return new IsThrowingSystemExceptionAnalyzer();
            yield return new HasInnerExceptionFromOuterCatchClauseAnalyzer();
        }

        #endregion

        #region properties

        protected IAnalyzeUnit AnalyzeUnit { get; private set; }

        protected Stack<IBlockModel> BlockModelsStack { get; }

        protected T Model { get; private set; }

        #endregion
    }
}