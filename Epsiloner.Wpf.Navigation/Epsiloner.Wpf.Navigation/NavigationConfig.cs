using System;
using System.Windows.Threading;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Base navigation configuration for navigation target.
    /// </summary>
    /// <typeparam name="T">Navigation target type.</typeparam>
    public abstract class NavigationConfig<T> : INavigationConfig<T> where T : INavigationTarget
    {
        /// <inheritdoc />
        public virtual bool BlockWindowClose => false;

        /// <inheritdoc />
        public virtual bool CanLeave(INavigationTarget target) => true;

        /// <inheritdoc />
        public virtual NavigationMode Mode => NavigationMode.Default;

        /// <inheritdoc />
        public bool AllowMultiple => true;

        /// <inheritdoc />
        public abstract object GenerateDataForTarget(INavigationTarget target, Dispatcher dispatcher);

        /// <inheritdoc />
        public virtual NavigationMode GetNavigationMode(INavigationTarget target) => NavigationMode.Default;

        /// <inheritdoc />
        public virtual bool MatchesTarget(INavigationTarget target) => true;

        /// <inheritdoc />
        public bool TryHandleNavigation(INavigationTarget target, object data)
        {
            throw new NotImplementedException();
        }
    }
}
