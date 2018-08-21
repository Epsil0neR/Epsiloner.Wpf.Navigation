using Epsiloner.Wpf.Navigation;
using Sample_1.Modules.Details.ViewModels;
using Sample_1.NavigationTargets;
using System.Windows.Threading;

namespace Sample_1.Modules.Details.Configs
{
    public class DetailsNavigationConfig : NavigationConfig<DetailsNavigationTarget>
    {
        public override object GenerateDataForTarget(INavigationTarget target, Dispatcher dispatcher)
        {
            var t = (DetailsNavigationTarget)target;
            var rv = new DetailsViewModel(dispatcher, t.Param);
            return rv;
        }
    }
}