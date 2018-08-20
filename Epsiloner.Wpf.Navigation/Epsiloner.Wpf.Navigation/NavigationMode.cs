namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Indicates where navigation will occur.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Navigation occurs in first available <see cref="ShellBase"/> or creates new <see cref="ShellBase"/>.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Navigation depends on <see cref="INavigationConfig{T}.GetNavigationMode"/> method.
        /// </summary>
        DependsOnTarget = 1,

        /// <summary>
        /// Navigation occurs in new window.
        /// </summary>
        NewWindow = 2,

        /// <summary>
        /// Navigation occurs in new dialog window.
        /// </summary>
        DialogWindow = 3,

        /// <summary>
        /// Navigation occurs in new child window.
        /// </summary>
        ChildWindow = 4
    }
}