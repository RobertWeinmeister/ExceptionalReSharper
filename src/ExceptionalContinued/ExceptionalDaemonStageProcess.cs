using System;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.ExceptionalContinued
{
    /// <summary>This process is executed by the ReSharper's Daemon</summary>
    /// <remarks>
    /// The instance of this class is constructed each time the daemon needs to re highlight a given file.
    /// This object is short-lived. It executes the target highlighting logic.
    /// </remarks>
    public class ExceptionalDaemonStageProcess : CSharpDaemonStageProcessBase
    {
        #region member vars

        private readonly IHighlightingConsumer _consumer;

        #endregion

        #region constructors and destructors

        public ExceptionalDaemonStageProcess(ICSharpFile file, IPsiSourceFile psiSourceFile, IContextBoundSettingsStore settings) : base(
            ServiceLocator.Process,
            file)
        {
            _consumer = new FilteringHighlightingConsumer(psiSourceFile, file, settings);
        }

        #endregion

        #region methods

        public void AddHighlighting(IHighlighting highlighting, DocumentRange range)
        {
            _consumer.AddHighlighting(highlighting, range);
        }

        /// <exception cref="OperationCanceledException">The process has been cancelled. </exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public override void Execute(Action<DaemonStageResult> commiter)
        {
            var file = ServiceLocator.Process.SourceFile.GetTheOnlyPsiFile(CSharpLanguage.Instance) as ICSharpFile;
            if (file == null)
            {
                return;
            }
            var elementProcessor = new ExceptionalRecursiveElementProcessor(this);
            file.ProcessDescendants(elementProcessor);
            if (ServiceLocator.Process.InterruptFlag)
            {
                throw new OperationCanceledException();
            }
            commiter(new DaemonStageResult(_consumer.Highlightings));
        }

        #endregion
    }
}