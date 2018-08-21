using Epsiloner.Wpf.Navigation;
using Epsiloner.Wpf.ViewModels;
using Sample_1.Core;
using Sample_1.NavigationTargets;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sample_1.Modules.Index.ViewModels
{
    public class IndexViewModel : ViewModel, IIndexViewModel, INavigatable
    {
        public Dispatcher Dispatcher { get; }

        public ICommand GoToDetailsCommand { get; }
        public ICommand RequestCloseCommand { get; }

        public IndexViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            GoToDetailsCommand = new RelayCommand(GoToDetails);
            RequestCloseCommand = new RelayCommand(o => RequestClose?.Invoke(this, EventArgs.Empty));
        }

        private void GoToDetails(object param)
        {
            var p = param as string;
            var target = new DetailsNavigationTarget(p);

            Navigation.Navigate(target);
        }

        /// <inheritdoc />
        public event EventHandler RequestClose;

        /// <inheritdoc />
        public void Navigated()
        {
            MessageBox.Show("Navigated to " + nameof(IndexViewModel));
        }
    }
}
