using Epsiloner.Wpf.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Sample_1.Windows;

namespace Sample_1.Resolvers
{
    public class MultiThreadShellResolver : IShellResolver
    {
        /// <summary>
        /// Dictionary of threads to owned shell bases.
        /// Required for spectating when to shutdown thread.
        /// </summary>
        private static readonly Dictionary<Thread, List<ShellBase>> _dictionary = new Dictionary<Thread, List<ShellBase>>();
        private bool _mainCreated = false;

        public MultiThreadShellResolver()
        {
        }

        public ShellBase CreateShell(ShellBase parent)
        {
            var mainCreated = _mainCreated;
            _mainCreated = true;
            return CreateCustomShell(() => CreateShell(!mainCreated), parent);
        }

        private void ShellOnClosed(object sender, EventArgs eventArgs)
        {
            var shell = sender as ShellBase;
            if (shell == null)
                return;

            // Remove handlers from shell
            shell.Closed -= ShellOnClosed;

            var dispatcher = shell.Dispatcher;
            var thread = dispatcher.Thread;
            var list = _dictionary[thread];
            list.Remove(shell);

            // Check if thread has no more opened windows.
            if (list.Count == 0)
            {
                _dictionary.Remove(thread);
                // Shutdown empty thread which won't be used anymore by application.
                dispatcher.UnhandledException -= DomainExceptionHandler.OnUnhandledDispatcherException;
                dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                ShutdownIfAllClosed();
            }
        }

        private void ShutdownIfAllClosed()
        {
            if (_dictionary.Any())
                return;

            var app = System.Windows.Application.Current;
            if (app == null)
                return;

            if (app.Dispatcher.CheckAccess())
                app?.Shutdown();
            else
                app.Dispatcher.InvokeAsync(() => app.Shutdown());
        }

        private ShellBase CreateShell(bool createMain)
        {
            var shell = new MainWindow();
            return shell;
        }

        public ShellBase CreateCustomShell(Func<ShellBase> creationAction, ShellBase parent)
        {
            var t = this;
            ShellBase shell = null;
            if (parent != null)
            {
                parent.Dispatcher.Invoke(() =>
                {
                    shell = creationAction();
                    shell.Owner = parent;

                    // Register newly created shell base for existing thread.
                    _dictionary[shell.Dispatcher.Thread].Add(shell);
                });
            }
            else
            {
                var mre = new ManualResetEventSlim();
                var thread = new Thread(() =>
                {
                    shell = creationAction();
                    mre.Set();
                    Dispatcher.CurrentDispatcher.UnhandledException += DomainExceptionHandler.OnUnhandledDispatcherException;
                    Dispatcher.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
                thread.Name = "Window thread";
                mre.Wait();

                // Register new thread with newly created shell base.
                _dictionary[thread] = new List<ShellBase>() { shell };
            }

            shell.Closed += ShellOnClosed;

            return shell;
        }
    }
}
