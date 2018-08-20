using System;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Additional functionality when using <see cref="Navigation"/>. Should be used only on data objects.
    /// </summary>
    public interface INavigatable
    {
        /// <summary>
        /// Event to request navigation to close owned <see cref="ShellBase"/>.
        /// Supportable only if opened in <see cref="NavigationMode.DialogWindow"/> or <see cref="NavigationMode.ChildWindow"/> mode.
        /// </summary>
        event EventHandler RequestClose;

        /// <summary>
        /// Executed when navigation completed.
        /// </summary>
        void Navigated();
    }
}