using Epsiloner.Wpf.Navigation;
using System;
using System.Collections.Generic;

namespace Sample_1.Core
{
    public static class NavigationConfigs
    {
        public static Dictionary<Type, INavigationConfig<INavigationTarget>> Configs { get; }
            = new Dictionary<Type, INavigationConfig<INavigationTarget>>();
    }
}
