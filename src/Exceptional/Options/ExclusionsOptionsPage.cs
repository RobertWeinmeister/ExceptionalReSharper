using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.Rd.Base;
using JetBrains.ReSharper.UnitTesting.Analysis.nUnit;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Options
{
    [OptionsPage(Pid, Name, typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = ExceptionalOptionsPage.Pid, Sequence = 0.0)]
    public class ExclusionsOptionsPage : BeSimpleOptionsPage
    {
        public const string Name = "Exclusions";
        public const string Pid = "Exceptional::Exclusions";

        public ExclusionsOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            bool wrapInScrollablePanel = false) : base(lifetime, optionsPageContext, optionsSettingsSmartContext, wrapInScrollablePanel)
        {
            _ = AddText(Resources.Options_Exclusions_Description);
            CreateRichTextExceptionTypesAsHint(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        private void CreateRichTextExceptionTypesAsHint(Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<string> property = new Property<string>(lifetime, "Exceptional::ExceptionTypesAsHintForMethodsOrProperties::ExceptionTypes");
            property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) => key.OptionalMethodExceptions2));
            property.Change.Advise(
                                   lifetime,
                                   a =>
                                   {
                                       if(!a.HasNew)
                                       {
                                           return;
                                       }
                                       storeOptionsTransactionContext.SetValue((ExceptionalSettings key) => key.OptionalMethodExceptions2, a.New);
                                   });
            var textControl = BeControls.GetTextControl(isReadonly: false);
            textControl.Text.SetValue(property.GetValue());
            textControl.Text.Change.Advise(
                                           lifetime,
                                           str =>
                                           {
                                               storeOptionsTransactionContext.SetValue((ExceptionalSettings key) => key.OptionalMethodExceptions2, str);
                                           });
            AddControl(textControl);
        }
    }
}