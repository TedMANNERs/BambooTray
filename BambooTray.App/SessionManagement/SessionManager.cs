using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BambooTray.App.Bamboo;
using BambooTray.App.Model;

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

        public async Task OpenSession()
        {
            if (File.Exists(SessionFileName))
            {
                using (FileStream stream = new FileStream(SessionFileName, FileMode.Open))
                using (TextReader reader = new StreamReader(stream))
                {
                    _session = new Session(reader.ReadLine());
                    _bambooPlanUpdater.Start(_session, SessionExpired);
                }
            }
            else
                await GetNewSession().ConfigureAwait(false);
        }

        public void CloseSession()
        {
            _bambooPlanUpdater.Stop();
        }

        private async Task GetNewSession()
        {
            LoginCredentials credentials = _loginDialogService.ShowDialog();
            if (credentials != null)
            {
                _session = await _authenticator.Authenticate(credentials).ConfigureAwait(false);
                SaveSession();
            }
        }

        public event EventHandler SessionExpired;

        private async void OnSessionExpired(object sender, EventArgs args)
        {
            _bambooPlanUpdater.Stop();
            await Application.Current.Dispatcher.InvokeAsync(GetNewSession);
            _bambooPlanUpdater.Start(_session, SessionExpired);
        }

        private void SaveSession()
        {
            using (FileStream stream = new FileStream(SessionFileName, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(_session.SessionId);
            }
        }
    }
}