using Epsiloner.Helpers;
using Epsiloner.Wpf.Attributes;
using Epsiloner.Wpf.Navigation;
using Sample_1.Core;
using Sample_1.NavigationTargets;
using Sample_1.Resolvers;
using System;
using System.Windows;

namespace Sample_1
{
    public partial class App
    {
        /// <inheritdoc />
        public App()
        {
            //Navigation.Dispatcher = Dispatcher;
            //Navigation.SetShellResolver(new SingleThreadShellResolver());
            //Note: this sample also provides useful MultiThreadShellResolver which will put all windows
            // into separate dispatchers (and threads) except child windows and dialogs.S
            Navigation.SetShellResolver(new MultiThreadShellResolver());

            //TODO: Load all assemblies which can contain navigation configs
            //For example, you use this example to load assemblies dynamicall
            AppDomain.CurrentDomain.LoadAssemblies("Sample_1.Modules.*.dll");
            AppDomain.CurrentDomain.InitializeTypesFromAttribute().Wait();

            Navigation.AllWindowsClosed += NavigationOnAllWindowsClosed;
            Navigation.NavigationFailed += NavigationOnNavigationFailed;

            //React when view or config dynamically are loaded dynamically after all have been proeceeded.
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
            //Proceed all configs
            Navigation.RegisterAvailableConfigs(ConfigResolver);
            //Proceed all views
            ViewForAttribute.ProceedRelatedAssemblies();
        }

        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            ViewForAttribute.ProceedAssembly(args.LoadedAssembly);
            Navigation.RegisterAvailableConfigs(args.LoadedAssembly, ConfigResolver);
        }

        private void NavigationOnAllWindowsClosed(object sender, EventArgs e)
        {
            MessageBox.Show("All navigation windows closed.");

        }

        private void NavigationOnNavigationFailed(NavigationFailReason reason, INavigationTarget target)
        {
            MessageBox.Show(reason.ToString(), "Navigation failed.");

        }

        private INavigationConfig<INavigationTarget> ConfigResolver(Type type)
        {
            INavigationConfig<INavigationTarget> rv;
            NavigationConfigs.Configs.TryGetValue(type, out rv);
            return rv;
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var task = Navigation.Navigate(new IndexNavigationTarget());
            //you can also attach continuation to navigation task.
        }

    }
}
