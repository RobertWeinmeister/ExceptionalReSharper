using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.ExceptionalContinued.Models.ExceptionsOrigins;

namespace ReSharper.ExceptionalContinued.Models
{
    /// <summary>Stores data about processed <see cref="IConstructorDeclaration" /></summary>
    internal sealed class ConstructorDeclarationModel : AnalyzeUnitModelBase<IConstructorDeclaration>
    {
        #region constructors and destructors

        /// <summary>Initializes a new instance of the <see cref="ConstructorDeclarationModel" /> class. </summary>
        /// <param name="constructorDeclaration">The constructor declaration. </param>
        public ConstructorDeclarationModel(IConstructorDeclaration constructorDeclaration) : base(null, constructorDeclaration)
        {
            if (constructorDeclaration.Initializer != null)
            {
                ThrownExceptions.Add(new ConstructorInitializerModel(this, constructorDeclaration.Initializer, this));
            }
            else
            {
                if (constructorDeclaration.DeclaredElement?.IsDefault != true)
                {
                    return;
                }
                var containingType = constructorDeclaration.DeclaredElement.GetContainingType();
                var baseClass = containingType?.GetSuperTypes().FirstOrDefault(t => !t.IsInterfaceType());
                var baseClassTypeElement = baseClass?.GetTypeElement();
                var defaultBaseConstructor = baseClassTypeElement?.Constructors.FirstOrDefault(c => c.IsDefault);
                if (defaultBaseConstructor != null)
                {
                    ThrownExceptions.Add(new ConstructorInitializerModel(this, defaultBaseConstructor, this));
                }
            }
        }

        #endregion

        #region properties

        /// <summary>Gets the content block of the object. </summary>
        public override IBlock Content => Node.Body;

        #endregion
    }
}