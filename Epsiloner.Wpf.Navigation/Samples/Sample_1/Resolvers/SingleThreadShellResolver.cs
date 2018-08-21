using System;
using System.Collections.Generic;
using System.Linq;
using Epsiloner.Wpf.Navigation;
using Sample_1.Windows;

namespace Sample_1.Resolvers
{
    public class SingleThreadShellResolver : IShellResolver
    {
        private readonly IList<ShellBase> _shells = new List<ShellBase>();

        public SingleThreadShellResolver()
        {
        }


        public ShellBase CreateShell(ShellBase parent)
        {
            var rv = CreateShell(!_shells.Any());
            if (parent != null)
                rv.Owner = parent;
            return rv;
        }

        public ShellBase CreateCustomShell(Func<ShellBase> creationAction, ShellBase parent)
        {
            return creationAction();
        }

        private ShellBase CreateShell(bool createMain)
        {

            var shell = new MainWindow();
            shell.Closed += ShellOnClosed;
            _shells.Add(shell);

            return shell;
        }

        private void ShellOnClosed(object sender, EventArgs e)
        {
            var shell = sender as ShellBase;
            if (shell == null)
                return;

            // Remove handlers from shell
            shell.Closed -= ShellOnClosed;

            _shells.Remove(shell);
            if (_shells.Any())
                return;

            var app = System.Windows.Application.Current;
            if (app == null)
                return;

            if (app.Dispatcher.CheckAccess())
                app?.Shutdown();
            else
                app.Dispatcher.InvokeAsync(() => app.Shutdown());
        }
    }
}