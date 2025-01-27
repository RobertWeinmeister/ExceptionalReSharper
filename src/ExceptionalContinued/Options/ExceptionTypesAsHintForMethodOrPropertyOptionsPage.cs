﻿using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.Rd.Base;
using JetBrains.Util;
using ReSharper.ExceptionalContinued.Settings;

namespace ReSharper.ExceptionalContinued.Options
{
    [OptionsPage(Pid, Name, typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = ExceptionalOptionsPage.Pid, Sequence = 3.0)]
    public class ExceptionTypesAsHintForMethodOrPropertyOptionsPage : BeSimpleOptionsPage
    {
        #region constants

        public const string Name = "Optional Exceptions (Methods or Properties)";
        public const string Pid = "Exceptional::ExceptionTypesAsHintForMethodsOrProperties";

        #endregion

        #region constructors and destructors

        public ExceptionTypesAsHintForMethodOrPropertyOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            bool wrapInScrollablePanel = true) : base(lifetime, optionsPageContext, optionsSettingsSmartContext, wrapInScrollablePanel)
        {
            AddText(OptionsLabels.ExceptionTypesAsHintForMethodOrProperty.Description);
            CreateCheckboxUsePredefined(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
            AddButton(OptionsLabels.ExceptionTypesAsHintForMethodOrProperty.ShowPredefined, ShowPredefined);
            AddSpacer();
            AddText(OptionsLabels.ExceptionTypesAsHintForMethodOrProperty.Note);
            CreateRichTextExceptionTypesAsHint(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        #endregion

        #region methods

        private void CreateCheckboxUsePredefined(Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property = new Property<bool>(lifetime, "Exceptional::ExceptionTypesAsHintForMethodsOrProperties::UsePredefined");
            property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) => key.UseDefaultOptionalMethodExceptions2));
            property.Change.Advise(
                lifetime,
                a =>
                {
                    if (!a.HasNew)
                    {
                        return;
                    }
                    storeOptionsTransactionContext.SetValue((ExceptionalSettings key) => key.UseDefaultOptionalMethodExceptions2, a.New);
                });
            AddBoolOption(
                (ExceptionalSettings key) => key.UseDefaultOptionalMethodExceptions2,
                OptionsLabels.ExceptionTypesAsHintForMethodOrProperty.UsePredefined);
        }

        private void CreateRichTextExceptionTypesAsHint(Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<string> property = new Property<string>(lifetime, "Exceptional::ExceptionTypesAsHintForMethodsOrProperties::ExceptionTypes");
            property.SetValue(storeOptionsTransactionContext.GetValue((ExceptionalSettings key) => key.OptionalMethodExceptions2));
            property.Change.Advise(
                lifetime,
                a =>
                {
                    if (!a.HasNew)
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

        private void ShowPredefined()
        {
            var content = ExceptionalSettings.DefaultOptionalMethodExceptions;
            MessageBox.ShowInfo(content);
        }

        #endregion
    }
}