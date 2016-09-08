using System.Windows;

namespace BambooTray.App
{
    public static class ViewCreator
    {
        public static void ShowView<TView, TViewModel>() where TView : Window, new() where TViewModel : IViewModel
        {
            TView view = new TView { DataContext = AppKernel.Get<TViewModel>() };
            view.Show();
        }
    }
}