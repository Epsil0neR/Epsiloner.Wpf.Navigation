# Sample_1

Shows how to use **Epsiloner.Wpf.Navigation** in WPF application where each module is hosted in separate project (C# class library).

1. In **App.xaml** (not in App.xaml.cs) remove property MainWindow. *Navigation module will handle it*

2. In App costructor (**App.xaml.cs**):
   1. Set **Dispatcher** property for Navigation. Navigation will appear only on that thread. 

      ***Required*** if using SingleThreadShellResolver - when you use only 1 Dispatcher.

      ***Optional*** if using MultiThreadShellResolver - in that case all navigation will occur only on 1 Dispatcher. Note: in that case it is recommended to use Dispatcher that will not own any windows.

   2. If modules are in separate projects, make sure these projects are loaded into memory. 

      (***Note:*** Sample_1.Modules.Index and Sample_1.Modules.Details are referenced by Sample_1, but they are not used directly in code. It just to have their .dll in output folder to load them during runtime.)

      **App.xaml.cs - AppDomain.CurrentDomain.LoadAssemblies("Sample_1.Modules.*.dll");** - loads all projects in output directory that matches search criteria.

   3. Register navigation configs. 

      1. In Sample_1 in constructor it is done via **Navigation.RegisterAvailableConfigs(ConfigResolver);** which registers configs declared in currently loaded assemblies. You can proceed also a single assembly or register by passing instance of INavigationConfig.
      2. In Sample_1 in CurrentDomainOnAssemblyLoad method (handler when assembly loaded) proceeds only one assembly at a time.
      3. In module **Sample_1.Modules.Index Setup constructor** manually registers navigation configuration for IndexNavigationTarget.

   4. In sample views are bound to view models via attribute **ViewForAttribute** and MainWindow also uses template selctor from that attribute.

      To proceed all views, we need to call **ViewForAttribute.ProceedRelatedAssemblies();**

   5. OnStartup we need to start navigation - in sample we navigating to IndexNavigationTarget.

## Modules

Modules (aka pages) in Sample_1 are declared in separate project each:
   - Sample_1.Modules.Index
   - Sample_1.Modules.Details

Both structure is similar:
   - **ViewModels** - contains view models used only by this module
   - **Views** - contains views and other controls used only by this module
   - **Configs** - contains INavigationConfig implementation where described configuration for IndexNavigationTarget or DetailsNavigationTarget. These configs also generates ViewModel on requiest.
   - **Setup.cs** - static class which constructor is invoked when assembly is loaded.

### Differences

   - **Index module - Setup.cs** - registers config directly, when Setup in Details module only registers it for ConfigResolver. (see **Note 1**)
   - **Index module - IndexViewModel.cs** - implements optional interface for data - INavigatable which provides additional functionality for ViewModel, like request to close window(shell) and method Navigated() which is invoked once navigation completed. (occurs before similar method invoked in View)
   - **Index module - IndexView.xaml.cs** - implements optional interface for view - INavigatableView which provides additional functionality for View like:
      - Navigated(owner shell, parent shell) method - invoked once navigation completed (occurs after similar method invoked in ViewModel). Gives oportunity to edit window properties, like Width, Height, Title, etc..
      - Unloading(owner shell) method - invoked when leaving view (navigating to another view, closing window, etc..) to reset custom modifications made to window - like set WindowStyle to previous value - in case that view should be shown without header and buttons.

## Notes

### Note 1 - No IoC in sample

Sample_1 has no IoC, so method ConfigResolver in App.xaml.cs is made just for example. In real world there should be IoC resolve method. (Tested with Unity)

### Note 2 - Multiple configs for same target

It possible to register multiple configs for same target. **Please avoid that at least in first releases, because it is not tested yet.**

### Note 3 - Recommendation for code structure

If application is designed to have modules(aka pages) in separate projects, then it is strongly recommended to handle **AppDomain.CurrentDomain.AssemblyLoad** event and try to proceed new things like navigation configs, views, etc..




