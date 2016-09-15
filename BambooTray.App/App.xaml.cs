using System.Windows;
using BambooTray.App.View;
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
            Current.MainWindow = AppKernel.Get<MainView>();
            Current.MainWindow.DataContext = AppKernel.Get<IMainViewModel>();
            Current.MainWindow.Show();
        }
    }
}
