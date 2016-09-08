using System.Windows;

namespace BambooTray.App
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _loginViewModel.Password = PasswordBox.SecurePassword;
        }

        private void Window_Loaded(object obj, RoutedEventArgs e)
        {
            _loginViewModel = (LoginViewModel)DataContext;
            _loginViewModel.Close += (sender, args) => Close();
        }
    }
}