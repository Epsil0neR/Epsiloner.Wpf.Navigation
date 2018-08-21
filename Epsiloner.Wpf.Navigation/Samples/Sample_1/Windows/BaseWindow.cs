using Epsiloner.Wpf.Navigation;

namespace Sample_1.Windows
{
    public class BaseWindow : ShellBase
    {
        private string _title;

        public BaseWindow()
        {
        }

        public override string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Dispatcher.Invoke(() =>
                {
                    var title = string.IsNullOrWhiteSpace(_title) ? "Sample_1" : $"{_title} - Sample_1";
                    base.Title = title;
                });
            }
        }
    }
}
