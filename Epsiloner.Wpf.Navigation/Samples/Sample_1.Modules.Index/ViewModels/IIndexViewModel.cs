using System.Windows.Input;
using Epsiloner.Wpf.ViewModels;

namespace Sample_1.Modules.Index.ViewModels
{
    public interface IIndexViewModel : IViewModel
    {
        ICommand GoToDetailsCommand { get; }
    }
}