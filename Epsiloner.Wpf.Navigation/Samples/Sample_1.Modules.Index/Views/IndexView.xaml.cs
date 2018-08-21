using Epsiloner.Wpf.Attributes;
using Sample_1.Modules.Index.ViewModels;

namespace Sample_1.Modules.Index.Views
{
    [ViewFor(typeof(IIndexViewModel))]
    public partial class IndexView
    {
        public IndexView()
        {
            InitializeComponent();
        }
    }
}
