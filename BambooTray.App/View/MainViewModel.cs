using System.Windows.Input;
using BambooTray.App.SessionManagement;
using BambooTray.App.View.Popup;

namespace BambooTray.App.View
{
    public class MainViewModel : IMainViewModel
    {
        private readonly ISessionManager _sessionManager;

        public MainViewModel(ISessionManager sessionManager, IPopupViewModel popupViewModel)
        {
            _sessionManager = sessionManager;
            PopupViewModel = popupViewModel;
            LoginCommand = new DelegateCommand(obj => Login(), () => !_sessionManager.HasValidSession);
        }

        public ICommand LoginCommand { get; set; }
        public IPopupViewModel PopupViewModel { get; set; }

        public void Load()
        {
            _sessionManager.OpenSession();
        }

        public void Close()
        {
            _sessionManager.CloseSession();
        }

        private void Login()
        {
            _sessionManager.OpenSession();
        }
    }
}