using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using BambooTray.App.Bamboo;

namespace BambooTray.App
{
    public class LoginViewModel : ILoginViewModel
    {
        private readonly ISessionManager _sessionManager;

        public LoginViewModel(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            LoginCommand = new DelegateCommand(async obj => await Login(), CanLogin);
            CancelCommand = new DelegateCommand(obj => CloseView(), () => true);
        }

        public ICommand CancelCommand { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; } = new SecureString();
        public ICommand LoginCommand { get; set; }
        public event EventHandler Close;

        private bool CanLogin() => !string.IsNullOrEmpty(Username) && Password.Length > 0;

        private async Task Login()
        {
            bool success = await _sessionManager.CreateSession(Username, Password);
            if (success)
                CloseView();
            else
                ClearInput();
        }

        private void CloseView()
        {
            ClearInput();
            Close?.Invoke(this, EventArgs.Empty);
        }

        private void ClearInput()
        {
            Username = string.Empty;
            Password = new SecureString();
        }
    }
}