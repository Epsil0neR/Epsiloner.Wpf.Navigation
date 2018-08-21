using System.Windows;
using System.Windows.Threading;

namespace Sample_1
{
    public static class DomainExceptionHandler
    {
        public static void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled)
                return;

            e.Handled = true;
            //TODO: LogException(e.Exception);
            MessageBox.Show(e.Exception.ToString(), e.ToString());
        }
    }
}
