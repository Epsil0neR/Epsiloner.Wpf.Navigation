using Epsiloner.Wpf.Navigation;

namespace Sample_1.NavigationTargets
{
    public class DetailsNavigationTarget : INavigationTarget
    {
        public string Param { get; set; }

        /// <inheritdoc />
        public DetailsNavigationTarget(string param)
        {
            Param = param;
        }
    }
}