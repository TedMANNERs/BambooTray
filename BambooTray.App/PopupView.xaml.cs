using System.Windows;
using System.Windows.Input;

namespace BambooTray.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PopupView : Window
    {
        public PopupView()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            IPopupViewModel popupViewModel = (PopupViewModel)DataContext;
            popupViewModel.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
