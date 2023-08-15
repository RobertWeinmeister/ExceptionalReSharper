using System;
using System.Linq;
using JetBrains.Application.Shell;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.ExceptionalContinued.Utilities
{
    /// <summary>Aids in creating names for code elements.</summary>
    public static class NameFactory
    {
        #region methods

        public static string CatchVariableName(ITreeNode treeNode, IDeclaredType exceptionType)
        {
            try
            {
                var sourceFile = treeNode.GetSourceFile();
                var namingPolicyManager = new NamingPolicyManager(LanguageManager.Instance, treeNode.GetSolution());
                var nameParser = new NameParser(treeNode.GetSolution(), namingPolicyManager, new HostCulture());
                var nameSuggestionManager = new NameSuggestionManager(treeNode.GetSolution(), nameParser, namingPolicyManager);
                //var policy = namingPolicyManager.GetPolicy(NamedElementKinds.Locals, treeNode.Language, sourceFile);
                if (sourceFile == null)
                {
                    return string.Empty;
                }
                var namesCollection = nameSuggestionManager.CreateEmptyCollection(PluralityKinds.Single, treeNode.Language, true, sourceFile);
                var entryOptions = new EntryOptions
                {
                    PluralityKind = PluralityKinds.Single,
                    PredefinedPrefixPolicy = PredefinedPrefixPolicy.Preserve,
                    Emphasis = Emphasis.Good,
                    SubrootPolicy = SubrootPolicy.Decompose
                };
                namesCollection.Add(exceptionType, entryOptions);
                //var namesSuggestion = namesCollection.Prepare(policy.NamingRule, ScopeKind.Common, new SuggestionOptions());
                return namesCollection.GetRoots().FirstOrDefault()?.GetFinalPresentation() ?? string.Empty;
            }
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}