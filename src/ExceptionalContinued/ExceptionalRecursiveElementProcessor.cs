using System.Collections.Generic;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.ExceptionalContinued.Contexts;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued
{
    public class ExceptionalRecursiveElementProcessor : IRecursiveElementProcessor
    {
        #region member vars

        private readonly List<IDocCommentBlock> _eventComments = new List<IDocCommentBlock>();

        private IProcessContext _currentContext;

        #endregion

        #region constructors and destructors

        public ExceptionalRecursiveElementProcessor(CSharpDaemonStageProcessBase daemonProcess)
        {
            _currentContext = new NullProcessContext();
        }

        #endregion

        #region explicit interfaces

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return true;
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
            switch (element)
            {
                case IMethodDeclaration _:
                case IEventDeclaration _:
                case IAccessorOwnerDeclaration _:
                    _currentContext.RunAnalyzers();
                    break;
                case IAccessorDeclaration _:
                    //_currentContext.RunAnalyzers(_daemonProcess, _settings); // Already analyzed by accessor owner
                    _currentContext.LeaveAccessor();
                    break;
                case IConstructorDeclaration _:
                    _currentContext.RunAnalyzers();
                    break;
                case ITryStatement _:
                    _currentContext.LeaveTryBlock();
                    break;
                case ICatchClause _:
                    _currentContext.LeaveCatchClause();
                    break;
            }
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            switch (element)
            {
                case IThrowStatement statement:
                    _currentContext.Process(statement);
                    break;
                case ICatchVariableDeclaration declaration:
                    _currentContext.Process(declaration);
                    break;
                case IReferenceExpression expression:
                    _currentContext.Process(expression);
                    break;
                case IObjectCreationExpression creationExpression:
                    _currentContext.Process(creationExpression);
                    break;
                case IMethodDeclaration methodDeclaration:
                    _currentContext = new MethodProcessContext();
                    _currentContext.StartProcess(new MethodDeclarationModel(methodDeclaration));
                    break;
                case IConstructorDeclaration constructorDeclaration:
                    _currentContext = new ConstructorProcessContext();
                    _currentContext.StartProcess(new ConstructorDeclarationModel(constructorDeclaration));
                    break;
                case IEventDeclaration eventDeclaration:
                {
                    _currentContext = new EventProcessContext();
                    _currentContext.StartProcess(new EventDeclarationModel(eventDeclaration));
                    foreach (var doc in _eventComments)
                    {
                        _currentContext.Process(doc);
                    }
                    _eventComments.Clear();
                    break;
                }
                case IAccessorOwnerDeclaration accessorOwnerDeclaration:
                    _currentContext = new AccessorOwnerProcessContext();
                    _currentContext.StartProcess(new AccessorOwnerDeclarationModel(accessorOwnerDeclaration));
                    break;
                case IAccessorDeclaration declaration:
                    _currentContext.EnterAccessor(declaration);
                    break;
                case IDocCommentBlock block when _currentContext.Model == null || _currentContext.Model.Node == block.Parent:
                    _currentContext.Process(block);
                    break;
                case IDocCommentBlock block:
                    _eventComments.Add(block);
                    // HACK: Event documentation blocks are processed before event declaration, 
                    // other documentation blocks are processed after the associated element declaration
                    break;
                case IThrowExpression expression:
                    _currentContext.Process(expression);
                    break;
                case ITryStatement tryStatement:
                    _currentContext.EnterTryBlock(tryStatement);
                    break;
                case ICatchClause clause:
                    _currentContext.EnterCatchClause(clause);
                    break;
            }
        }

        public bool ProcessingIsFinished => ServiceLocator.Process.InterruptFlag;

        #endregion
    }
}