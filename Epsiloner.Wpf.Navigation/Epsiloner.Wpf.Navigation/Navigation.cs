using Epsiloner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Provides navigation functionality.
    /// </summary>
    public static class Navigation
    {
        #region Events

        /// <summary>
        /// Indicates that navigation has completed and application can be closed.
        /// </summary>
        public static event EventHandler AllWindowsClosed;

        #endregion

        #region Fields
        private static readonly Dictionary<Type, List<INavigationConfig<INavigationTarget>>> _navitaionTargetToConfigs = new Dictionary<Type, List<INavigationConfig<INavigationTarget>>>();
        private static readonly List<ShellState> _states = new List<ShellState>();
        private static Task _navigationTask = Task.CompletedTask;

        private static IShellResolver _shellResolver;
        #endregion

        /// <summary>
        /// Raised if navigation is requested for target which is not registered.
        /// </summary>
        public static event Action<NavigationFailReason, INavigationTarget> NavigationFailed;

        #region Properties
        /// <summary>
        /// Dispatcher where all navigation will occur. If null - then functionality is not guaranteed.
        /// </summary>
        public static Dispatcher Dispatcher { get; set; }
        #endregion

        #region Registration

        /// <summary>
        /// Registers <paramref name="config"/> for specified <typeparam name="T"><see cref="INavigationTarget"/></typeparam> type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException">When <paramref name="config"/> is null.</exception>
        public static void RegisterTarget<T>(INavigationConfig<T> config) where T : INavigationTarget
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            InvokeOnDispatcher(() =>
            {
                RegisterTarget(typeof(T), (INavigationConfig<INavigationTarget>)config);
            });
        }

        /// <summary>
        /// Registers <paramref name="config"/> for specified <paramref name="type"/> which must inherit <see cref="INavigationTarget"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException">When <paramref name="config"/> is null.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">When <see cref="INavigationTarget"/> is not assignable from <paramref name="type"/>.</exception>
        public static void RegisterTarget(Type type, INavigationConfig<INavigationTarget> config)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(INavigationTarget).IsAssignableFrom(type))
                throw new ArgumentException(nameof(type));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            InvokeOnDispatcher(() =>
            {
                if (!_navitaionTargetToConfigs.ContainsKey(type))
                    _navitaionTargetToConfigs[type] = new List<INavigationConfig<INavigationTarget>>();

                _navitaionTargetToConfigs[type].Add(config);
            });
        }

        /// <summary>
        /// Registers all available configs in all loaded dependant assemblies.
        /// </summary>
        public static void RegisterAvailableConfigs(Func<Type, INavigationConfig<INavigationTarget>> configResolver)
        {
            if (configResolver == null)
                throw new ArgumentNullException(nameof(configResolver));

            var generic = typeof(INavigationConfig<>);
            var assemblies = generic.Assembly.GetDependentAssemblies();

            InvokeOnDispatcher(() =>
            {
                foreach (var assembly in assemblies)
                {
                    RegisterAvailableConfigs(assembly, configResolver);
                }
            });
        }

        /// <summary>
        /// Registers all available configs in speicifid <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="configResolver"></param>
        /// <exception cref="ArgumentNullException">When <paramref name="assembly"/> is null.</exception>
        public static void RegisterAvailableConfigs(Assembly assembly, Func<Type, INavigationConfig<INavigationTarget>> configResolver)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (configResolver == null)
                throw new ArgumentNullException(nameof(configResolver));

            var generic = typeof(INavigationConfig<>);
            var allTypes = assembly.GetTypes().ToList();
            var matches = allTypes.Where(x => x.IsClass && IsSubclassOfRawGeneric(generic, x)).ToList();

            InvokeOnDispatcher(() =>
            {
                foreach (var type in matches)
                {
                    var parameterizedGeneric = GetParameterizedGeneric(generic, type);
                    INavigationConfig<INavigationTarget> config = configResolver(type);

                    RegisterTarget(parameterizedGeneric.GenericTypeArguments.First(), config);
                }
            });
        }

        /// <summary>
        /// Gets all registered configs for speicified navigation target type.
        /// </summary>
        /// <typeparam name="T">Navigation target type</typeparam>
        /// <returns></returns>
        public static IEnumerable<INavigationConfig<INavigationTarget>> GetConfigs<T>() where T : INavigationTarget
        {
            var type = typeof(T);
            if (_navitaionTargetToConfigs.ContainsKey(type))
                return _navitaionTargetToConfigs[type].ToList();

            return null;
        }
        #endregion

        /// <summary>
        /// Checks if config for specified navigation target is registered.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsRegistered(INavigationTarget target)
        {
            return _navitaionTargetToConfigs.ContainsKey(target.GetType());
        }

        /// <summary>
        /// Sets shell resolver to use.
        /// </summary>
        /// <param name="shellResolver"></param>
        public static void SetShellResolver(IShellResolver shellResolver)
        {
            if (shellResolver == null)
                throw new ArgumentNullException(nameof(shellResolver));

            InvokeOnDispatcher(() => _shellResolver = shellResolver);
        }

        /// <summary>
        /// Tryies to navigate to speicified target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns>Task which represents current navigation.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="target"/> is null.</exception>
        /// <exception cref="InvalidOperationException">When shell resolver is not set. After executing <see cref="SetShellResolver"/> method, this exeption won't occur.</exception>
        /// <exception cref="InvalidOperationException">When <see cref="INavigationConfig{T}.GetNavigationMode"/> returns <see cref="NavigationMode.DependsOnTarget"/>.</exception>
        /// <exception cref="InvalidOperationException">When <see cref="INavigationConfig{T}.GenerateDataForTarget"/> returns null.</exception>
        public static Task Navigate<T>(T target) where T : INavigationTarget
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Task task;

            lock (_navigationTask)
            {
                _navigationTask = _navigationTask.ContinueWith(_ => NavigateInner(target).Wait(), TaskContinuationOptions.LongRunning);
                task = _navigationTask;
            }

            return task;
        }

        private static Task NavigateInner<T>(T target) where T : INavigationTarget
        {
            return InvokeOnDispatcherAsync(() =>
            {
                if (_shellResolver == null)
                    throw new InvalidOperationException("Shell resolver must be set before invoking Navigate method.");

                var type = target.GetType();
                if (!_navitaionTargetToConfigs.ContainsKey(type))
                {
                    NavigationFailed?.Invoke(NavigationFailReason.NotConfigured, target);
                    return;
                }

                //1. Find config
                var configs = _navitaionTargetToConfigs[type];
                // If registered more than 1 config, find config which matches target. If no found or only 1 config - use first navigation config.
                var config = (configs.Count > 1 ? configs.FirstOrDefault(x => x.MatchesTarget(target)) : null) ?? configs.First();
                var navMode = config.Mode;
                if (navMode == NavigationMode.DependsOnTarget)
                    navMode = config.GetNavigationMode(target);


                // Navigation mode after getting from config cannot be NavigationMode.DependsOnTarget
                if (navMode == NavigationMode.DependsOnTarget)
                    throw new InvalidOperationException("NavigationConfig.GetNavigationMode returned invalid value: NavigationMode.DependsOnTarget");

                ShellState shellState = null;
                bool existingShell = false;

                if (!config.AllowMultiple)
                {
                    var conflict = _states.FirstOrDefault(x => x.Config == config);
                    if (conflict != null)
                    {
                        //Check if config can handle navigation when already navigated.
                        try
                        {
                            if (config.TryHandleNavigation(target, conflict.Data))
                                return;
                        }
                        catch (NotImplementedException) { }
                        NavigationFailed?.Invoke(NavigationFailReason.NotAllowedToHaveMultiple, target);
                        return; // No futher navigation required.
                    }
                }

                //Only for default navigation mode can be searched existing shells.
                if (navMode == NavigationMode.Default)
                {
                    //Try find available shell
                    foreach (var state in _states)
                    {
                        // Check if config allows reuse shell
                        if (!state.Config.CanLeave(target))
                            continue;

                        //Only Default and NewWindow can be reused.
                        if (!(state.Mode == NavigationMode.Default || state.Mode == NavigationMode.NewWindow))
                            continue;


                        // On this line shell can be reused.
                        shellState = state;
                        existingShell = true;
                        break;
                    }
                }

                // Create shell data from config
                object shellData;
                ShellBase parent = null;
                if (!existingShell)
                {
                    // Check if need to set parent.
                    if ((navMode == NavigationMode.ChildWindow ||
                         navMode == NavigationMode.DialogWindow))
                    {
                        // Set parent window if shellState.Mode is Dialog or Child
                        if (_states.Any()) //TODO: Improve logic for selecting parent window.
                            parent = _states[0].Shell;
                    }

                    // Create new shell if not found available.
                    var shell = _shellResolver.CreateShell(parent);
                    shellState = new ShellState(shell);
                    shellState.ShellClosed += ShellStateOnShellClosed;
                    _states.Add(shellState);

                    shellData = config.GenerateDataForTarget(target, shellState.Shell.Dispatcher);
                    if (shellData == null)
                        throw new InvalidOperationException("Config returned null from generating object for target.");

                    // Set data in ShellBase
                    shellState.Load(navMode, config, target, shellData);

                    // Show shell.
                    if (shellState.Mode == NavigationMode.DialogWindow)
                        shellState.Shell.Dispatcher.InvokeAsync(() => shellState.Shell.ShowDialog());
                    else
                        shellState.Shell.Dispatcher.Invoke(() =>
                        {
                            shellState.Shell.Show();
                            shellState.Shell.Activate();
                        });
                }
                else
                {
                    shellData = config.GenerateDataForTarget(target, shellState.Shell.Dispatcher);
                    if (shellData == null)
                        throw new InvalidOperationException("Config returned null from generating object for target.");

                    // Unload current navigatable data.
                    shellState.Unload();

                    // Set data in ShellBase
                    shellState.Load(navMode, config, target, shellData);
                }

                //Navigation to target comleted.
                shellState.NavigationCompleted(parent);
            });
        }

        /// <summary>
        /// Handler for <see cref="Window.Closed"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private static void ShellStateOnShellClosed(object sender, EventArgs eventArgs)
        {
            var shellState = (ShellState)sender;
            // Shell won't be used after close - so we should dispose shell state together with shell.
            shellState.Dispose();

            InvokeOnDispatcher(async () =>
            {
                // Remove closed shell state
                _states.Remove(shellState);

                // Check if left only windows which does not block close from user input:
                if (_states.Any(x => x.Config?.BlockWindowClose != true))
                    return;

                // Manually dispose all shell states
                foreach (var state in _states)
                {
                    state.Dispose();
                }

                // And then notify that navigation has finished.
                await RaiseAllWindowsClosed();
            });
        }

        /// <summary>
        /// Raises <see cref="AllWindowsClosed"/> event.
        /// </summary>
        private static Task RaiseAllWindowsClosed()
        {
            return InvokeOnDispatcherAsync(() => AllWindowsClosed?.Invoke(null, EventArgs.Empty));
        }

        #region Private helpers

        /// <summary>
        /// Checks if <paramref name="toCheck"/> is sub-class of generic type <paramref name="generic"/>.
        /// </summary>
        /// <param name="generic">Generic type</param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (toCheck.IsClass)
            {
                var interfaces = toCheck.GetInterfaces();
                while (toCheck != null && toCheck != typeof(object))
                {
                    var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                    if (generic == cur)
                        return true;

                    toCheck = toCheck.BaseType;
                }
                return interfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == generic) != null;
            }

            return false;
        }

        private static Type GetParameterizedGeneric(Type generic, Type toCheck)
        {
            var test = new List<Type>();
            if (generic.IsClass)
                while (toCheck != null && toCheck != typeof(object))
                {
                    test.Add(toCheck);
                    var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                    if (generic == cur)
                        return test.Last();

                    toCheck = toCheck.BaseType;
                }
            if (generic.IsInterface)
            {
                var interfaces = toCheck.GetInterfaces();
                return interfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == generic);
            }
            return null;
        }

        /// <summary>
        /// Invokes specified action on dispatcher.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        private static void InvokeOnDispatcher(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (Dispatcher == null || Dispatcher.CheckAccess())
                action();
            else
                Dispatcher.Invoke(action);
        }

        private static async Task InvokeOnDispatcherAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (Dispatcher == null || Dispatcher.CheckAccess())
                action();
            else
                await Dispatcher.InvokeAsync(action);
        }
        #endregion

    }
}