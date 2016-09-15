using BambooTray.App.Model;

namespace BambooTray.App.View.Login
{
    public class LoginDialogService : ILoginDialogService
    {
        private readonly ILoginViewModel _loginViewModel;

        public LoginDialogService(ILoginViewModel loginViewModel)
        {
            _loginViewModel = loginViewModel;
        }

        public LoginCredentials ShowDialog()
        {
            LoginView view = new LoginView { DataContext = _loginViewModel };
            bool? result = view.ShowDialog();
            return result.HasValue && result.Value
                ? new LoginCredentials { Username = _loginViewModel.Username, Password = _loginViewModel.Password }
                : null;
        }
    }
}