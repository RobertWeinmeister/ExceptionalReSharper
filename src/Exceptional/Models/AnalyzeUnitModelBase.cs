using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal abstract class AnalyzeUnitModelBase<T> : BlockModelBase<T>, IAnalyzeUnit where T : ITreeNode
    {
        protected AnalyzeUnitModelBase(IAnalyzeUnit analyzeUnit, T node) : base(analyzeUnit, node) =>
            DocumentationBlock = new DocCommentBlockModel(this, null);

        public override void Accept(AnalyzerBase analyzer)
        {
            DocumentationBlock?.Accept(analyzer);
            base.Accept(analyzer);
        }

        public DocCommentBlockModel DocumentationBlock { get; set; }

        public IPsiModule GetPsiModule() => Node.GetPsiModule();

        public bool IsInspectionRequired
        {
            get
            {
                var accessRightsOwner = Node as IAccessRightsOwner;
                if (accessRightsOwner == null)
                {
                    return false;
                }

                if (IsNamespaceIgnored())
                {
                    return false;
                }

                var inspectPublicMethods    = ServiceLocator.Settings.InspectPublicMethods;
                var inspectInternalMethods  = ServiceLocator.Settings.InspectInternalMethods;
                var inspectProtectedMethods = ServiceLocator.Settings.InspectProtectedMethods;
                var inspectPrivateMethods   = ServiceLocator.Settings.InspectPrivateMethods;
                var rights                  = accessRightsOwner.GetAccessRights();
                return (rights == AccessRights.PUBLIC    && inspectPublicMethods)    ||
                       (rights == AccessRights.INTERNAL  && inspectInternalMethods)  ||
                       (rights == AccessRights.PROTECTED && inspectProtectedMethods) ||
                       (rights == AccessRights.PRIVATE   && inspectPrivateMethods);
            }
        }

        ITreeNode IAnalyzeUnit.Node => Node;

        private bool IsNamespaceIgnored()
        {
            var test                 = ServiceLocator.Settings.IgnoredNamespaces;
            var namespaceDeclaration = Node.Root().Children<ICSharpNamespaceDeclaration>().First().QualifiedName;
            return false;

            //get list of namespaces
            //does it match with one?
        }
    }
}