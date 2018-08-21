## Sample_1

Shows how to use **Epsiloner.Wpf.Navigation** in WPF application where each module is hosted in separate project (C# class library).

1. In **App.xaml** (not in App.xaml.cs) remove property MainWindow. *Navigation module will handle it*
2. In App costructor (**App.xaml.cs):
   1. Set dispatcher property for Navigation. Navigation will appear only on that thread. 
   
      **Required** if using SingleThreadShellResolver - when you use only 1 Dispatcher.
      
      **Optional** if using MultiThreadShellResolver - in that case all navigation will occur only on 1 Dispatcher. Note: in that case it is recommended to use Dispatcher that will not own any windows.
      
   2. If modules are in separate projects, make sure these projects are loaded into memory. (Note: Sample_1.Modules.Index and Sample_1.Modules.Details are referenced by Sample_1, but they are not used directly in code. It just to have their .dll in output folder.)
   
      App.xaml.cs - *AppDomain.CurrentDomain.LoadAssemblies("Sample_1.Modules.***.dll");* loads all projects in output directory that matches search criterias.
      
   3. Register navigation configs. In sample it is done via **Navigation.RegisterAvailableConfigs(ConfigResolver);** which registers configs declared in currently loaded asemblies. You can proceed also a single assembly or register by passing instance of INavigationConfig.
   
   4. In sample views are bound to view models via attribute **ViewForAttribute** and MainWindow also uses template selctor from that attribute.
      
      To proceed all views, we need to call **ViewForAttribute.ProceedRelatedAssemblies();**
      
   5. OnStartup we need to start navigation - in sample we navigating to IndexNavigationTarget.
   
   
   [TBD]
