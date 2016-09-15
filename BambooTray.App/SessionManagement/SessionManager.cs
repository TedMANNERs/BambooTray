using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BambooTray.App.Bamboo;
using BambooTray.App.Model;
using BambooTray.App.View.Login;

namespace BambooTray.App.SessionManagement
{
    public class SessionManager : ISessionManager
    {
        private const string SessionFileName = "Session";
        private readonly IAuthenticator _authenticator;
        private readonly IBambooPlanUpdater _bambooPlanUpdater;
        private readonly ILoginDialogService _loginDialogService;
        private Session _session;

        public SessionManager(IAuthenticator authenticator, IBambooPlanUpdater bambooPlanUpdater, ILoginDialogService loginDialogService)
        {
            _authenticator = authenticator;
            _bambooPlanUpdater = bambooPlanUpdater;
            _loginDialogService = loginDialogService;
            SessionExpired += OnSessionExpired;
        }

        public bool HasValidSession { get; private set; }

        public async Task OpenSession()
        {
            if (File.Exists(SessionFileName))
            {
                using (FileStream stream = new FileStream(SessionFileName, FileMode.Open))
                using (TextReader reader = new StreamReader(stream))
                {
                    string sessionId = reader.ReadLine();
                    string seraphId = reader.ReadLine();
                    _session = new Session(sessionId, seraphId);
                }
            }
            else
                await RenewSession().ConfigureAwait(false);

            if (_session != null)
            {
                _bambooPlanUpdater.Start(_session, SessionExpired);
                HasValidSession = true;
            }
        }

        public void CloseSession()
        {
            _bambooPlanUpdater.Stop();
            HasValidSession = false;
        }

        private async Task<bool> RenewSession()
        {
            _session = null;
            do
            {
                LoginCredentials credentials = Application.Current.Dispatcher.Invoke(_loginDialogService.ShowDialog);
                if (credentials != null)
                    _session = await _authenticator.Authenticate(credentials).ConfigureAwait(false);
                else
                    return false;
            }
            while (_session == null);
            SaveSession();
            return true;
        }

        public event EventHandler SessionExpired;

        private async void OnSessionExpired(object sender, EventArgs args)
        {
            CloseSession();
            bool result = await RenewSession().ConfigureAwait(false);
            if (!result)
                return;

            _bambooPlanUpdater.Start(_session, SessionExpired);
            HasValidSession = true;
        }

        private void SaveSession()
        {
            using (FileStream stream = new FileStream(SessionFileName, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(_session.SessionId);
                writer.WriteLine(_session.SeraphId);
            }
        }
    }
}