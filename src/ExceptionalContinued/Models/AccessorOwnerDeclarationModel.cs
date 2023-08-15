using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.ExceptionalContinued.Analyzers;

namespace ReSharper.ExceptionalContinued.Models
{
    internal sealed class AccessorOwnerDeclarationModel : AnalyzeUnitModelBase<IAccessorOwnerDeclaration>
    {
        #region constructors and destructors

        public AccessorOwnerDeclarationModel(IAccessorOwnerDeclaration node) : base(null, node)
        {
            Accessors = new List<AccessorDeclarationModel>();
        }

        #endregion

        #region methods

        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var accessor in Accessors)
            {
                accessor.Accept(analyzer);
            }
            base.Accept(analyzer);
        }

        #endregion

        #region properties

        public List<AccessorDeclarationModel> Accessors { get; }

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content => null;

        public override IEnumerable<ThrownExceptionModel> UncaughtThrownExceptions
        {
            get
            {
                return Accessors.SelectMany(m => m.UncaughtThrownExceptions);
            }
        }

        #endregion
    }
}