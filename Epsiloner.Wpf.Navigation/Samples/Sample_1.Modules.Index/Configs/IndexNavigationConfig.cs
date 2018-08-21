using Epsiloner.Wpf.Navigation;
using Sample_1.Modules.Index.ViewModels;
using Sample_1.NavigationTargets;
using System.Windows.Threading;

namespace Sample_1.Modules.Index.Configs
{
    public class IndexNavigationConfig : NavigationConfig<IndexNavigationTarget>
    {
        public override object GenerateDataForTarget(INavigationTarget target, Dispatcher dispatcher)
        {
            var t = (IndexNavigationTarget)target;
            var rv = new IndexViewModel(dispatcher);
            return rv;
        }
    }
}