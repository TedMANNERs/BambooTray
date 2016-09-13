using BambooTray.App.SessionManagement;

namespace BambooTray.App
{
    public class MainViewModel : IMainViewModel
    {
        private readonly ISessionManager _sessionManager;

        public MainViewModel(ISessionManager sessionManager, IPopupViewModel popupViewModel)
        {
            _sessionManager = sessionManager;
            PopupViewModel = popupViewModel;
        }

        public IPopupViewModel PopupViewModel { get; set; }

        public void Close()
        {
            _sessionManager.CloseSession();
        }

        public void Load()
        {
            _sessionManager.OpenSession();
        }
    }
}