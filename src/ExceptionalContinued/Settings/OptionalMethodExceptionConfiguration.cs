using System;
using JetBrains.ReSharper.Psi;
using JetBrains.Util.Logging;
using ReSharper.ExceptionalContinued.Models;

namespace ReSharper.ExceptionalContinued.Settings
{
    public sealed class OptionalMethodExceptionConfiguration
    {
        #region member vars

        private IDeclaredType _exceptionType;
        private bool _exceptionTypeLoaded;

        #endregion

        #region constructors and destructors

        public OptionalMethodExceptionConfiguration(string fullMethodName, string exceptionType)
        {
            FullMethodName = fullMethodName;
            ExceptionType = exceptionType;
        }

        #endregion

        #region methods

        internal bool IsSuperTypeOf(ThrownExceptionModel thrownException)
        {
            var exceptionType = GetExceptionType();
            return exceptionType != null && thrownException.ExceptionType.IsSubtypeOf(exceptionType);
        }

        private IDeclaredType GetExceptionType()
        {
            if (_exceptionTypeLoaded)
            {
                return _exceptionType;
            }
            try
            {
                _exceptionType = TypeFactory.CreateTypeByCLRName(ExceptionType, ServiceLocator.StageProcess.PsiModule);
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("[Exceptional] Error loading excluded method exception '{0}'", ExceptionType), ex);
            }
            finally
            {
                _exceptionTypeLoaded = true;
            }
            return _exceptionType;
        }

        #endregion

        #region properties

        internal string ExceptionType { get; }

        internal string FullMethodName { get; }

        #endregion
    }
}