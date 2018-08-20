using System;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Provides functionality to create shell.
    /// </summary>
    public interface IShellResolver
    {
        /// <summary>
        /// Craetes new shell.
        /// </summary>
        /// <param name="parent">Parent window for new shell.</param>
        /// <returns></returns>
        ShellBase CreateShell(ShellBase parent);

        /// <summary>
        /// Creates custom shell in same way it creates new shell.
        /// </summary>
        /// <param name="creationAction"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        ShellBase CreateCustomShell(Func<ShellBase> creationAction, ShellBase parent);
    }
}