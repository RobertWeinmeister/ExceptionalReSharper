using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.ExceptionalContinued.Models
{
    internal abstract class TreeElementModelBase<T> : ModelBase
        where T : ITreeNode
    {
        #region constructors and destructors

        protected TreeElementModelBase(IAnalyzeUnit analyzeUnit, T node) : base(analyzeUnit)
        {
            Node = node;
        }

        #endregion

        #region methods

        protected CSharpElementFactory GetElementFactory()
        {
            return CSharpElementFactory.GetInstance(AnalyzeUnit.Node);
        }

        #endregion

        #region properties

        public override DocumentRange DocumentRange => Node.GetDocumentRange();

        /// <summary>Gets or sets the node. </summary>
        public T Node { get; protected set; }

        #endregion
    }
}