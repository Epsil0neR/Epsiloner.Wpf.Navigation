using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Epsiloner.Wpf.Attributes;

namespace Epsiloner.Wpf.Navigation
{
    /// <summary>
    /// Window which is suitable with <see cref="Navigation"/> module.
    /// </summary>
    public class ShellBase : Window
    {
        private static readonly DependencyPropertyKey NavigationTargetTypePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(NavigationTargetType), typeof(Type), typeof(ShellBase),
                new PropertyMetadata());


        /// <summary>
        /// Control which must be presented in view to show target.
        /// </summary>
        public ContentPresenter ContentPresenter { get; }

        public Type NavigationTargetType
        {
            get { return (Type)GetValue(NavigationTargetTypePropertyKey.DependencyProperty); }
            private set { SetValue(NavigationTargetTypePropertyKey, value); }
        }

        public new virtual string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        public ShellBase()
        {
            if (typeof(ShellBase) == GetType())
                throw new Exception("ShellBase cannot be used without inheriting.");

            ContentPresenter = new ContentPresenter()
            {
                ContentTemplateSelector = ViewForAttribute.DataTemplateSelector
            };
        }

        /// <summary>
        /// Sets shell content to specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Data to use for content.</param>
        /// <param name="navigationTargetType">Type of current navigation target.</param>
        /// <returns></returns>
        internal INavigatableView SetContent(object data, Type navigationTargetType)
        {
            INavigatableView rv = Dispatcher.CheckAccess()
                ? SetContentAndGetNavigatable(data, navigationTargetType)
                : Dispatcher.Invoke(() => SetContentAndGetNavigatable(data, navigationTargetType), DispatcherPriority.Normal);

            return rv;
        }

        /// <summary>
        /// Sets shell content to specified <param name="data" /> and returns <see cref="INavigatableView"/> which has been created for content.
        /// </summary>
        /// <param name="data">Data to use for content.</param>
        /// <param name="navigationTargetType">Type of current navigation target.</param>
        /// <returns></returns>
        private INavigatableView SetContentAndGetNavigatable(object data, Type navigationTargetType)
        {
            NavigationTargetType = navigationTargetType;
            BeforeApplyTemplate(data);
            ContentPresenter.Content = data;
            ContentPresenter.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(ContentPresenter) == 1)
            {
                var rv = VisualTreeHelper.GetChild(ContentPresenter, 0);
                (rv as UIElement)?.Focus();
                return rv as INavigatableView;
            }

            return null;
        }

        protected virtual void BeforeApplyTemplate(object data) { }
    }
}