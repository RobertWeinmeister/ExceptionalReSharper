using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ReSharper.ExceptionalContinued.Highlightings
{
    /// <summary>Base class for all highlightings.</summary>
    /// <remarks>Provides default implementation.</remarks>
    public abstract class HighlightingBase : IHighlighting
    {
        #region explicit interfaces

        public DocumentRange CalculateRange()
        {
            throw new NotImplementedException();
        }

        public virtual string ErrorStripeToolTip => Message;

        public bool IsValid()
        {
            return true;
        }

        public virtual string ToolTip => Message;

        #endregion

        #region properties

        public virtual int NavigationOffsetPatch => 0;

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected abstract string Message { get; }

        #endregion
    }
}