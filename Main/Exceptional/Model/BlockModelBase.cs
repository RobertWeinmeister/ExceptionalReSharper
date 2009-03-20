using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class BlockModelBase<T> : TreeElementModelBase<T>, IBlockModel where T : ITreeNode
    {
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        protected BlockModelBase(IAnalyzeUnit analyzeUnit, T node) 
            : base(analyzeUnit, node)
        {
            TryStatementModels = new List<TryStatementModel>();
            ThrowStatementModels = new List<ThrowStatementModel>();
        }

        public virtual bool CatchesException(IDeclaredType exception)
        {
            return false;
        }

        public virtual IDeclaredType GetCatchedException()
        {
            return null;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var tryStatementModel in this.TryStatementModels)
            {
                tryStatementModel.Accept(analyzerBase);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }
        }

        public virtual IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in this.ThrowStatementModels)
                {
                    foreach (var thrownExceptionModel in throwStatementModel.ThrownExceptions)
                    {
                        if (thrownExceptionModel.IsCatched == false)
                        {
                            yield return thrownExceptionModel;
                        }
                    }
                }

                for (var i = 0; i < this.TryStatementModels.Count; i++)
                {
                    IBlockModel tryStatementModel = this.TryStatementModels[i];
                    foreach (var model in tryStatementModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }
    }
}