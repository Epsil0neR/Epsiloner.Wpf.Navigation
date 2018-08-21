using Sample_1.Core;
using Sample_1.Modules.Details.Configs;

namespace Sample_1.Modules.Details
{
    public static class Setup
    {
        static Setup()
        {
            var t = typeof(DetailsNavigationConfig);
            var c = new DetailsNavigationConfig();
            NavigationConfigs.Configs.Add(t, c);
        }
    }
}
