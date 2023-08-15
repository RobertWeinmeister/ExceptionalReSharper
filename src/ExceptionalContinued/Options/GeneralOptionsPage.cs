using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using ReSharper.ExceptionalContinued.Settings;

namespace ReSharper.ExceptionalContinued.Options
{
    [OptionsPage(Pid, Name, typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = ExceptionalOptionsPage.Pid,
                 Sequence = 0.0)]
    public class GeneralOptionsPage : BeSimpleOptionsPage
    {
        private const string Name = "General";
        private const string Pid  = "Exceptional::General";

        public GeneralOptionsPage(Lifetime lifetime, OptionsPageContext optionsPageContext,
                                  OptionsSettingsSmartContext optionsSettingsSmartContext,
                                  bool wrapInScrollablePanel = false) : base(lifetime, optionsPageContext,
                                                                             optionsSettingsSmartContext,
                                                                             wrapInScrollablePanel)
        {
            CreateCheckboxInspectPublic(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
            CreateDocumentationSection(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        private void CreateCheckboxInspectPublic(Lifetime                       lifetime,
                                                 IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime, "Exceptional::General::DelegateInvocationsMayThrowSystemException");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key
                                                                                 .DelegateInvocationsMayThrowExceptions));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.DelegateInvocationsMayThrowExceptions,
                                                              a.New);
                                             });
            _ = AddBoolOption((ExceptionalSettings key) => key.DelegateInvocationsMayThrowExceptions,
                              OptionsLabels.General.DelegateInvocationsMayThrowSystemException);
        }

        private void CreateCheckboxIsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions(
            Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime,
                                   "Exceptional::General::IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key
                                                                                 .IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions,
                                                              a.New);
                                             });
            _ =
                AddBoolOption((ExceptionalSettings key) => key.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions,
                              OptionsLabels.General.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions);
        }

        private void CreateCheckboxIsDocumentationOfExceptionSubtypeSufficientForThrowStatements(
            Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime,
                                   "Exceptional::General::IsDocumentationOfExceptionSubtypeSufficientForThrowStatements");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key
                                                                                 .IsDocumentationOfExceptionSubtypeSufficientForThrowStatements));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements,
                                                              a.New);
                                             });
            _ =
                AddBoolOption((ExceptionalSettings key) => key.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements,
                              OptionsLabels.General.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements);
        }

        private void CreateDocumentationSection(Lifetime                       lifetime,
                                                IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            _ = AddHeader(OptionsLabels.General.DocumentationOfThrownExceptionsSubtypeHeader);
            CreateCheckboxIsDocumentationOfExceptionSubtypeSufficientForThrowStatements(lifetime,
                storeOptionsTransactionContext);
            CreateCheckboxIsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions(lifetime,
                storeOptionsTransactionContext);
        }
    }
}