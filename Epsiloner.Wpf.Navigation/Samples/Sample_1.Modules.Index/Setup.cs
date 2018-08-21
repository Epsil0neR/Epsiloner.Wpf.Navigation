using Sample_1.Core;
using Sample_1.Modules.Index.Configs;

namespace Sample_1.Modules.Index
{
    public static class Setup
    {
        static Setup()
        {
            var t = typeof(IndexNavigationConfig);
            var c = new IndexNavigationConfig();
            NavigationConfigs.Configs.Add(t, c);
        }
    }
}
