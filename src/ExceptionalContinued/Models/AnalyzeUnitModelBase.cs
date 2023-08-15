using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Analyzers;
using ReSharper.ExceptionalContinued.Utilities;

namespace ReSharper.ExceptionalContinued.Models
{
    internal abstract class AnalyzeUnitModelBase<T> : BlockModelBase<T>, IAnalyzeUnit where T : ITreeNode
    {
        protected AnalyzeUnitModelBase(IAnalyzeUnit analyzeUnit, T node) : base(analyzeUnit, node) =>
            DocumentationBlock = new DocCommentBlockModel(this, null);

        public override void Accept(AnalyzerBase analyzer)
        {
            if (IsNamespaceIgnored())
            {
                return;
            }

            DocumentationBlock?.Accept(analyzer);
            base.Accept(analyzer);
        }

        public DocCommentBlockModel DocumentationBlock { get; set; }

        public IPsiModule GetPsiModule() => Node.GetPsiModule();

        public bool IsInspectionRequired
        {
            get
            {
                if (!(Node is IAccessRightsOwner accessRightsOwner))
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
            var namesToIgnore =
                ServiceLocator.Settings.IgnoredNamespaces.Split(new[] {Environment.NewLine},
                                                                StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in namesToIgnore)
            {
                if (NamespaceChecker
                   .HasNamespaceMatch(Node.Root().Children<ICSharpNamespaceDeclaration>().First().QualifiedName, name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}