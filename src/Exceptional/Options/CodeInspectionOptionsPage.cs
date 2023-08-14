using JetBrains.IDE.UI.Extensions;
using JetBrains.Rd.Base;

namespace ReSharper.Exceptional.Options
{
    using JetBrains.Application.Settings;
    using JetBrains.Application.UI.Options;
    using JetBrains.Application.UI.Options.OptionsDialog;
    using JetBrains.DataFlow;
    using JetBrains.IDE.UI.Options;
    using JetBrains.Lifetimes;
    using Settings;

    [OptionsPage("Exceptional::CodeInspection", "Code Inspection", typeof(UnnamedThemedIcons.ExceptionalSettings),
                 ParentId = ExceptionalOptionsPage.Pid, Sequence = 1.0)]
    public class CodeInspectionOptionsPage : BeSimpleOptionsPage
    {
        public CodeInspectionOptionsPage(Lifetime lifetime, OptionsPageContext optionsPageContext,
                                         OptionsSettingsSmartContext optionsSettingsSmartContext,
                                         bool wrapInScrollablePanel = false) : base(lifetime, optionsPageContext,
            optionsSettingsSmartContext, wrapInScrollablePanel)
        {
            _ = AddHeader(Resources.Options_CodeInspection_AccessibilityLevels_Header);
            CreateCheckboxInspectPublic(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
            CreateCheckboxInspectInternal(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
            CreateCheckboxInspectProtected(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
            CreateCheckboxInspectPrivate(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);

            _ = AddHeader(Resources.Options_CodeInspection_ExcludedNamespaces_Header);
            _ = AddText(Resources.Options_CodeInspection_ExcludedNamespaces_Text);
            _ = AddText(Resources.Options_CodeInspection_ExcludedNamespaces_Explanation);
            CreateRichTextNamespacesToIgnore(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        private void CreateRichTextNamespacesToIgnore(Lifetime                       lifetime,
                                                      IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<string> property = new Property<string>(lifetime, "Exceptional::CodeInspection::Namespaces");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key.ExcludedNamespaces));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.ExcludedNamespaces,
                                                              a.New);
                                             });
            var textControl = BeControls.GetTextControl(isReadonly: false);
            textControl.Text.SetValue(property.GetValue());
            textControl.Text.Change.Advise(lifetime,
                                           str =>
                                           {
                                               storeOptionsTransactionContext
                                                  .SetValue((ExceptionalSettings key) => key.ExcludedNamespaces, str);
                                           });
            AddControl(textControl);
        }

        private void CreateCheckboxInspectInternal(Lifetime                       lifetime,
                                                   IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime, "Exceptional::CodeInspection::InspectInternalMethodsAndProperties");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key.InspectInternalMethods));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.InspectInternalMethods,
                                                              a.New);
                                             });
            _ = AddBoolOption((ExceptionalSettings key) => key.InspectInternalMethods,
                              OptionsLabels.InspectionLevel.InspectInternalMethodsAndProperties);
        }

        private void CreateCheckboxInspectPrivate(Lifetime                       lifetime,
                                                  IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime, "Exceptional::CodeInspection::InspectPrivateMethodsAndProperties");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key.InspectPrivateMethods));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.InspectPrivateMethods,
                                                              a.New);
                                             });
            _ = AddBoolOption((ExceptionalSettings key) => key.InspectPrivateMethods,
                              OptionsLabels.InspectionLevel.InspectPrivateMethodsAndProperties);
        }

        private void CreateCheckboxInspectProtected(Lifetime                       lifetime,
                                                    IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime, "Exceptional::CodeInspection::InspectProtectedMethodsAndProperties");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key.InspectProtectedMethods));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.InspectProtectedMethods,
                                                              a.New);
                                             });
            _ = AddBoolOption((ExceptionalSettings key) => key.InspectProtectedMethods,
                              OptionsLabels.InspectionLevel.InspectProtectedMethodsAndProperties);
        }

        private void CreateCheckboxInspectPublic(Lifetime                       lifetime,
                                                 IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property =
                new Property<bool>(lifetime, "Exceptional::CodeInspection::InspectPublicMethodsAndProperties");
            _ = property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) =>
                                                                              key.InspectPublicMethods));
            property.Change.Advise(lifetime, a =>
                                             {
                                                 if (!a.HasNew)
                                                 {
                                                     return;
                                                 }

                                                 storeOptionsTransactionContext
                                                    .SetValue((ExceptionalSettings key) => key.InspectPublicMethods,
                                                              a.New);
                                             });
            _ = AddBoolOption((ExceptionalSettings key) => key.InspectPublicMethods,
                              OptionsLabels.InspectionLevel.InspectPublicMethodsAndProperties);
        }
    }
}