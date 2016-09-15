using System.Windows;

namespace BambooTray.App.View.Login
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private ILoginViewModel _loginViewModel;

        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            _loginViewModel.Password = PasswordBox.SecurePassword;
        }

        private void WindowLoaded(object obj, RoutedEventArgs e)
        {
            _loginViewModel = (LoginViewModel)DataContext;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}