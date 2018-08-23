# Epsiloner.Wpf.Navigation

[![NuGet](https://img.shields.io/nuget/v/Epsiloner.Wpf.Navigation.svg)](https://www.nuget.org/packages/Epsiloner.Wpf.Navigation/)
[![NuGet](https://img.shields.io/nuget/dt/Epsiloner.Wpf.Navigation.svg)](https://www.nuget.org/packages/Epsiloner.Wpf.Navigation/)
[![GitHub](https://img.shields.io/github/license/Epsil0neR/Epsiloner.Wpf.Navigation.svg)](https://github.com/Epsil0neR/Epsiloner.Wpf.Navigation)
[![GitHub issues](https://img.shields.io/github/issues/Epsil0neR/Epsiloner.Wpf.Navigation.svg)](https://github.com/Epsil0neR/Epsiloner.Wpf.Navigation)

[Samples](https://github.com/Epsil0neR/Epsiloner.Wpf.Navigation/tree/master/Epsiloner.Wpf.Navigation/Samples)

Epsiloner.Wpf.Navigation is a framework similar to [Prism](https://prismlibrary.github.io/) for building loosely coupled, maintainable, testable XAML application in WPF with objects as parameters.
Provides a design pattern that are helpful in writing well-structured and maintainable applications.
It allows to have each page in separate project with dynamic registration of each page and ability to correctly navigate to each page by defining classes (that implements INavigationTarget) with corresponding parameters.

For example, Epsiloner.Wpf.Navigation allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Epsiloner.Wpf.Navigation is designed to perform navigation from any place, but most useful to use from ViewModels.

Epsiloner.Wpf.Navigation is a fully open source project.

**I appreciate any input from outside - ideas / wishes / bugs / bug fixes / etc..**