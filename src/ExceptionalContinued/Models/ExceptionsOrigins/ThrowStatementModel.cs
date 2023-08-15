using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.Models.ExceptionsOrigins
{
    internal class ThrowStatementModel : TreeElementModelBase<IThrowStatement>, IExceptionsOriginModel
    {
        #region member vars

        private readonly ThrownExceptionModel _thrownException;

        #endregion

        #region constructors and destructors

        /// <summary>Initializes a new instance of the <see cref="ThrowStatementModel" /> class. </summary>
        /// <param name="analyzeUnit">The analyze unit.</param>
        /// <param name="throwStatement">The throw statement.</param>
        /// <param name="containingBlock">The containing block.</param>
        public ThrowStatementModel(IAnalyzeUnit analyzeUnit, IThrowStatement throwStatement, IBlockModel containingBlock) : base(analyzeUnit, throwStatement)
        {
            ContainingBlock = containingBlock;
            var exceptionType = GetExceptionType();
            var exceptionDescription = GetThrownExceptionMessage(throwStatement);
            string accessor = null;
            if (containingBlock is AccessorDeclarationModel model)
            {
                accessor = model.Node.NameIdentifier.Name;
            }
            _thrownException = new ThrownExceptionModel(analyzeUnit, this, exceptionType, exceptionDescription, false, accessor);
        }

        #endregion

        #region explicit interfaces

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
            _thrownException.Accept(analyzer);
        }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; }

        /// <summary>Gets the document range of the throw statement which is highlighted. </summary>
        public override DocumentRange DocumentRange =>
            // if we have exceptiontype then highlight the type
            Node.Exception?.GetDocumentRange() ?? Node.ThrowKeyword.GetDocumentRange();

        // otherwise highlight the throw keyword
        /// <summary>Gets the node. </summary>
        ITreeNode IExceptionsOriginModel.Node => Node;

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName, Node);
            var block = codeElementFactory.CreateBlock(Node);
            tryStatement.SetTry(block);
            Node.ReplaceBy(tryStatement);
            return true;
        }

        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get
            {
                return new List<ThrownExceptionModel>(new[] { _thrownException });
            }
        }

        #endregion

        #region methods

        public TextRange[] AddInnerException(string variableName)
        {
            var ranges = new List<TextRange>();
            if (!(Node.Exception is IObjectCreationExpression objectCreationExpressionNode))
            {
                return new TextRange[0];
            }
            if (objectCreationExpressionNode.Arguments.Count == 0)
            {
                var messageExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.Node).CreateExpressionAsIs("\"See the inner exception for details.\"");
                var messageArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.Node).CreateArgument(ParameterKind.VALUE, messageExpression);
                messageArgument = objectCreationExpressionNode.AddArgumentAfter(messageArgument, null);
                ranges.Add(messageArgument.GetDocumentRange().TextRange);
            }
            else
            {
                var lastArgument = objectCreationExpressionNode.ArgumentList.Arguments.Last();
                var innerExceptionExpression = CSharpElementFactory.GetInstance(AnalyzeUnit.Node).CreateExpressionAsIs(variableName);
                var innerExpressionArgument = CSharpElementFactory.GetInstance(AnalyzeUnit.Node).CreateArgument(ParameterKind.VALUE, innerExceptionExpression);
                innerExpressionArgument = objectCreationExpressionNode.AddArgumentAfter(innerExpressionArgument, lastArgument);
                ranges.Add(innerExpressionArgument.GetDocumentRange().TextRange);
            }
            return ranges.ToArray();
        }

        /// <summary>Searches for the nearest containing catch clause. </summary>
        /// <returns>The catch clause. </returns>
        public CatchClauseModel FindOuterCatchClause()
        {
            var outerBlock = ContainingBlock;
            while (outerBlock != null && outerBlock is CatchClauseModel == false)
            {
                outerBlock = outerBlock.ParentBlock;
            }
            return outerBlock as CatchClauseModel;
        }

        public bool IsInnerExceptionPassed(string variableName)
        {
            var objectCreationExpressionNode = Node.Exception as IObjectCreationExpression;
            if (objectCreationExpressionNode == null)
            {
                return false;
            }
            if (objectCreationExpressionNode.Arguments.Count < 1)
            {
                return false;
            }
            return objectCreationExpressionNode.Arguments.Any(a => a.GetText().Equals(variableName) || a.GetText().Split(':').Last().Trim().Equals(variableName));
        }

        /// <summary>Checks whether this throw statement throws given <paramref name="exceptionType" />.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            return _thrownException.IsException(exceptionType);
        }

        private IDeclaredType GetExceptionType()
        {
            if (Node?.Exception != null)
            {
                return Node.Exception.GetExpressionType() as IDeclaredType;
            }
            return FindOuterCatchClause().CaughtException; // Node.Exception == null when this is a "throw;" statement
        }

        private static string GetThrownExceptionMessage(IThrowStatement throwStatement)
        {
            if (!(throwStatement.Exception is IObjectCreationExpression expression))
            {
                return string.Empty;
            }
            var arguments = expression.Arguments;
            if (arguments.Count <= 0)
            {
                return string.Empty;
            }
            if (!(arguments[0].Value is ICSharpLiteralExpression literal) || literal.Literal == null)
            {
                return string.Empty;
            }
            if (literal.Literal.Parent is ICSharpLiteralExpression exp && exp.ConstantValue.Value != null)
            {
                return exp.ConstantValue.Value.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region properties

        public bool IsDirectExceptionInstantiation => Node.Exception is IObjectCreationExpression;

        /// <summary>Gets a value indicating whether the throw statement is a rethrow statement. </summary>
        public bool IsRethrow => Node.Exception == null;

        #endregion
    }
}