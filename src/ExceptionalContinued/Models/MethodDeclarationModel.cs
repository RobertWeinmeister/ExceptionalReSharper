using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.ExceptionalContinued.Models
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration" /></summary>
    internal sealed class MethodDeclarationModel : AnalyzeUnitModelBase<IMethodDeclaration>
    {
        #region constructors and destructors

        public MethodDeclarationModel(IMethodDeclaration methodDeclaration) : base(null, methodDeclaration)
        {
        }

        #endregion

        #region properties

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content => Node.Body;

        #endregion
    }
}