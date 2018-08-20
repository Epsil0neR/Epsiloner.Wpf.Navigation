using System;
using System.Windows.Threading;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Navigation configuration for navigation target.
    /// </summary>
    /// <typeparam name="T">Navigation target type.</typeparam>
    /// <remarks>Type parameter is required for registration.</remarks>
    // ReSharper disable once UnusedTypeParameter - used by Navigation module.
    public interface INavigationConfig<out T> where T : INavigationTarget
    {
        /// <summary>
        /// Indicates if window can be closed by CLOSE button or other shortkey (Except when application exits)
        /// </summary>
        bool BlockWindowClose { get; }

        /// <summary>
        /// Indicates if navigation can be changed to another. (Except when window is closing)
        /// </summary>
        /// <param name="target">Target where trying to navigate.</param>
        bool CanLeave(INavigationTarget target);

        /// <summary>
        /// Indicates which mode will be used.
        /// If <see cref="NavigationMode.DependsOnTarget"/>, then will be invoked <see cref="GetNavigationMode"/> for each target.
        /// </summary>
        NavigationMode Mode { get; }

        /// <summary>
        /// Indicates if simultaneously can be opened multiple targets.
        /// </summary>
        bool AllowMultiple { get; }


        /// <summary>
        /// Generates data for specified <see cref="target"/>.
        /// </summary>
        /// <param name="target">Target to which data should be generated.</param>
        /// <param name="dispatcher">Dispatcher to use inside data.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Navigation module requires not null value.</exception>
        object GenerateDataForTarget(INavigationTarget target, Dispatcher dispatcher);

        /// <summary>
        /// Gets navigation mode for target. Required only if <see cref="Mode"/> is <see cref="NavigationMode.DependsOnTarget"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        NavigationMode GetNavigationMode(INavigationTarget target);
        /// <summary>
        /// Checks if <paramref name="target" /> is suitable.
        /// </summary>
        /// <param name="target">Navigation target</param>
        /// <returns></returns>
        bool MatchesTarget(INavigationTarget target);

        /// <summary>
        /// Last hope. Asks config to try to navigate to specified <paramref name="target"/>. Invoked only when <see cref="AllowMultiple"/> and <see cref="CanLeave"/> is set to <see cref="bool.False"/>.
        /// </summary>
        /// <param name="target">Navigation target</param>
        /// <param name="data">Current data used by navigation. Previously gerenated by this config using <see cref="GenerateDataForTarget"/> method.</param>
        /// <returns><see cref="bool.True"/> if config will perform inner navigation, <see cref="bool.False"/> otherwise.</returns>
        /// <remarks><exception cref="NotImplementedException"> will result in same like <see cref="bool.False"/>.</exception></remarks>
        bool TryHandleNavigation(INavigationTarget target, object data);
    }
}