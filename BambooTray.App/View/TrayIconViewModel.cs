using System.Windows;
using System.Windows.Input;
using BambooTray.App.SessionManagement;
using BambooTray.App.View.Popup;

namespace BambooTray.App.View
{
    public class TrayIconViewModel : ITrayIconViewModel
    {
        private readonly ISessionManager _sessionManager;

        public TrayIconViewModel(ISessionManager sessionManager, IPopupViewModel popupViewModel)
        {
            _sessionManager = sessionManager;
            PopupViewModel = popupViewModel;
            LoginCommand = new DelegateCommand(obj => Login(), () => !_sessionManager.HasValidSession);
            ExitCommand = new DelegateCommand(obj => Close(), () => true);
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public IPopupViewModel PopupViewModel { get; set; }

        public void Load()
        {
            _sessionManager.OpenSession();
        }

        public void Close()
        {
            _sessionManager.CloseSession();
            Application.Current.Shutdown();
        }

        private void Login()
        {
            _sessionManager.OpenSession();
        }
    }
}