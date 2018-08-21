using System.Diagnostics;
using Epsiloner.Wpf.Attributes;
using Epsiloner.Wpf.Navigation;
using Sample_1.Modules.Index.ViewModels;

namespace Sample_1.Modules.Index.Views
{
    [ViewFor(typeof(IIndexViewModel))]
    public partial class IndexView : INavigatableView
    {
        public IndexView()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public void Navigated(ShellBase owner, ShellBase parent)
        {
            Debugger.Break();
        }

        /// <inheritdoc />
        public void Unloading(ShellBase owner)
        {
            Debugger.Break();
        }
    }
}
