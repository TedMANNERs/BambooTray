using System.Windows;

namespace BambooTray.App
{
    public static class ViewFactory
    {
        public static void CreateView<TView, TViewModel>() where TView : Window, new() where TViewModel : IViewModel
        {
            Application.Current.Dispatcher.Invoke(
                                                  () =>
                                                  {
                                                      TView view = new TView { DataContext = AppKernel.Get<TViewModel>() };
                                                      view.Show();
                                                  });
        }
    }
}