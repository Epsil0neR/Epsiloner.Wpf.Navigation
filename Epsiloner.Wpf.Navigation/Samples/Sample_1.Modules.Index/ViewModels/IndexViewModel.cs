using Epsiloner.Wpf.Navigation;
using Epsiloner.Wpf.ViewModels;
using Sample_1.Core;
using Sample_1.NavigationTargets;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sample_1.Modules.Index.ViewModels
{
    public class IndexViewModel : ViewModel, IIndexViewModel
    {
        public Dispatcher Dispatcher { get; }

        public ICommand GoToDetailsCommand { get; }

        public IndexViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            GoToDetailsCommand = new RelayCommand(GoToDetails);
        }

        private void GoToDetails(object param)
        {
            var p = param as string;
            var target = new DetailsNavigationTarget(p);

            Navigation.Navigate(target);
        }
    }
}
