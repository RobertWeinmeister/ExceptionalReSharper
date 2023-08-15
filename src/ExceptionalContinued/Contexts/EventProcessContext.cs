using System;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Contexts
{
    internal class EventProcessContext : ProcessContext<EventDeclarationModel>
    {
        #region methods

        public override void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
            if (IsValid() == false)
            {
                return;
            }
            if (accessorDeclarationNode == null)
            {
                return;
            }
            var accessor = new AccessorDeclarationModel(AnalyzeUnit, accessorDeclarationNode, BlockModelsStack.Peek());
            Model.Accessors.Add(accessor);
            BlockModelsStack.Push(accessor);
        }

        public override void LeaveAccessor()
        {
            try
            {
                BlockModelsStack.Pop();
            }
            catch (InvalidOperationException ex)
            {
                // TODO: Handle the System.InvalidOperationException
            }
        }

        #endregion
    }
}