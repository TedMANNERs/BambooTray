using System;
using System.IO;
using System.Threading.Tasks;
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
            {
                LoginCredentials credentials = _loginDialogService.ShowDialog();
                if (credentials != null)
                {
                    _session = await _authenticator.Authenticate(credentials);
                    SaveSession();
                }
            }
        }

        public void CloseSession()
        {
            _bambooPlanUpdater.Stop();
        }

        public event EventHandler SessionExpired;

        private async void OnSessionExpired(object sender, EventArgs args)
        {
            await OpenSession();
        }

        private void SaveSession()
        {
            using (FileStream stream = new FileStream(SessionFileName, FileMode.CreateNew))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(_session.SessionId);
            }
        }
    }
}