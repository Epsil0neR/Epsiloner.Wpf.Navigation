using Epsiloner.Wpf.Attributes;
using Sample_1.Modules.Details.ViewModels;

namespace Sample_1.Modules.Details.Views
{
    [ViewFor(typeof(IDetailsViewModel))]
    public partial class DetailsView
    {
        public DetailsView()
        {
            InitializeComponent();
        }
    }
}
