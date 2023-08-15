using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB.Tree;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.Models
{
    using IBlock = JetBrains.ReSharper.Psi.CSharp.Tree.IBlock;
    using IThrowStatement = JetBrains.ReSharper.Psi.CSharp.Tree.IThrowStatement;
    using ITryStatement = JetBrains.ReSharper.Psi.CSharp.Tree.ITryStatement;

    internal sealed class CatchClauseModel : BlockModelBase<ICatchClause>
    {
        #region constructors and destructors

        public CatchClauseModel(ICatchClause catchClauseNode, IBlockModel tryStatementModel, IAnalyzeUnit analyzeUnit) : base(analyzeUnit, catchClauseNode)
        {
            ParentBlock = tryStatementModel;
            IsCatchAll = GetIsCatchAll();
        }

        #endregion

        #region methods

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
            base.Accept(analyzer);
        }

        public void AddCatchVariable(string variableName)
        {
            if (Node is IGeneralCatchClause)
            {
                if (HasVariable)
                {
                    return;
                }
                if (string.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(Node, CaughtException);
                }
                var codeFactory = new CodeElementFactory(GetElementFactory());
                var newCatch = codeFactory.CreateSpecificCatchClause(null, Node.Body, variableName);
                if (newCatch == null)
                {
                    return;
                }
                Node.ReplaceBy(newCatch);
                Node = newCatch;
                Variable = new CatchVariableModel(AnalyzeUnit, newCatch.ExceptionDeclaration);
            }
            else
            {
                if (HasVariable)
                {
                    return;
                }
                if (string.IsNullOrEmpty(variableName))
                {
                    variableName = NameFactory.CatchVariableName(Node, CaughtException);
                }
                var specificNode = (ISpecificCatchClause)Node;
                var exceptionType = (IUserDeclaredTypeUsage)specificNode.ExceptionTypeUsage;
                var exceptionTypeName = exceptionType.TypeName.NameIdentifier.Name;
                var tempTry = GetElementFactory().CreateStatement("try {} catch($0 $1) {}", exceptionTypeName, variableName) as ITryStatement;
                if (!(tempTry?.Catches[0] is ISpecificCatchClause tempCatch))
                {
                    return;
                }
                var resultVariable = specificNode.SetExceptionDeclaration(tempCatch.ExceptionDeclaration);
                Variable = new CatchVariableModel(AnalyzeUnit, resultVariable);
            }
        }

        public bool Catches(IDeclaredType exception)
        {
            if (exception == null)
            {
                return false;
            }
            return Node?.ExceptionType != null && exception.IsSubtypeOf(Node.ExceptionType);
        }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        public override bool CatchesException(IDeclaredType exception)
        {
            return ParentBlock.ParentBlock.CatchesException(exception); // Warning: ParentBlock of CatchClause is TryStatement and not the method!
        }

        private static bool ContainsRethrowStatement(IBlock body)
        {
            var statements = body.Statements;
            return Enumerable.OfType<IThrowStatement>(statements).Any();
        }

        private bool GetIsCatchAll()
        {
            if (Node.ExceptionType == null)
            {
                return false;
            }
            var hasConditionalClause = Node.Filter != null;
            var rethrows = ContainsRethrowStatement(Node.Body);
            var isSystemException = Node.ExceptionType.GetClrName().ShortName == "Exception";
            return isSystemException && !hasConditionalClause && !rethrows;
        }

        #endregion

        #region properties

        public IDeclaredType CaughtException => Node.ExceptionType;

        public override IBlock Content => Node.Body;

        public override DocumentRange DocumentRange => Node.CatchKeyword.GetDocumentRange();

        public bool HasVariable => Variable != null;

        /// <summary>Gets a value indicating whether this is a catch clause which catches System.Exception. </summary>
        public bool IsCatchAll { get; }

        public bool IsExceptionTypeSpecified => Node is ISpecificCatchClause;

        public CatchVariableModel Variable { get; set; }

        #endregion
    }
}