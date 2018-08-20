namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Additional functionality when using <see cref="Navigation"/>. Should be used only on views and controls that are used to display data.
    /// </summary>
    public interface INavigatableView
    {
        /// <summary>
        /// This method is invoked after navigation to that view completed.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="parent">[Nullable] Exists if navigation mode was <see cref="NavigationMode.DialogWindow"/> or <see cref="NavigationMode.ChildWindow"/>.</param>
        void Navigated(ShellBase owner, ShellBase parent);

        void Unloading(ShellBase owner);
    }
}