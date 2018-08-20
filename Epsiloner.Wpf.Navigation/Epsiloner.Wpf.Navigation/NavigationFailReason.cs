using System.ComponentModel;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Reason why navigation failed.
    /// </summary>
    public enum NavigationFailReason
    {
        [Description("No matching configuration for navigation target found.")]
        NotConfigured,

        [Description("Not allowed to navigate to same target multiple times.")]
        NotAllowedToHaveMultiple,
    }
}