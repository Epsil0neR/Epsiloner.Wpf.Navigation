using Epsiloner.Wpf.Navigation;
using Epsiloner.Wpf.ViewModels;
using Sample_1.Core;
using Sample_1.NavigationTargets;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sample_1.Modules.Details.ViewModels
{
    public interface IDetailsViewModel : IViewModel
    {
        string Param { get; set; }

        ICommand GoToIndexCommand { get; }
    }

    public class DetailsViewModel : ViewModel, IDetailsViewModel
    {
        private string _param;
        public Dispatcher Dispatcher { get; }

        public string Param
        {
            get { return _param; }
            set { Set(ref _param, value); }
        }

        /// <inheritdoc />
        public ICommand GoToIndexCommand { get; }

        public DetailsViewModel(Dispatcher dispatcher, string param)
        {
            Dispatcher = dispatcher;
            Param = param;

            GoToIndexCommand = new RelayCommand(GoToIndex);
        }

        private void GoToIndex(object obj)
        {
            var t = new IndexNavigationTarget();
            Navigation.Navigate(t);
        }
    }
}
