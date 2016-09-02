using System.Windows;
using Ninject;

namespace BambooTray.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppKernel.Instance.Kernel.Load<AppModule>();
            Current.MainWindow = AppKernel.Get<PopupView>();
            Current.MainWindow.DataContext = AppKernel.Get<IPopupViewModel>();
            Current.MainWindow.Show();
        }
    }
}
