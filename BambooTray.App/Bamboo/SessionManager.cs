using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using BambooTray.App.Configuration;

namespace BambooTray.App.Bamboo
{
    public class SessionManager : ISessionManager
    {
        private const string SessionFileName = "Session";
        private readonly IBambooService _bambooService;
        private readonly Configuration.Configuration _config;

        public SessionManager(IConfigurationManager configurationManager, IBambooService bambooService)
        {
            _bambooService = bambooService;
            _config = configurationManager.Config;
        }

        public static string SessionId { get; private set; }

        public void LoadSession()
        {
            if (File.Exists(SessionFileName))
            {
                using (FileStream stream = new FileStream(SessionFileName, FileMode.Open))
                using (TextReader reader = new StreamReader(stream))
                {
                    SessionId = reader.ReadLine();
                    _bambooService.Start();
                }
            }
            else
            {
                ViewFactory.CreateView<LoginView, LoginViewModel>();
            }
        }

        public async Task<bool> CreateSession(string username, SecureString password)
        {
            using (HttpClientHandler handle = new HttpClientHandler { UseCookies = false })
            using (HttpClient client = new HttpClient(handle))
            {
                HttpRequestMessage post = new HttpRequestMessage(HttpMethod.Post, $"{_config.BambooHostname}/userlogin.action")
                    {
                        Content = new FormUrlEncodedContent(
                            new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("os_username", username),
                                    new KeyValuePair<string, string>("os_password", GetStringFromSecureString(password)),
                                    new KeyValuePair<string, string>("os_cookie", "true")
                                })
                    };
                HttpResponseMessage postResponse = await client.SendAsync(post).ConfigureAwait(false);
                if (!postResponse.IsSuccessStatusCode)
                    return false;

                IEnumerable<string> enumerable = postResponse.Headers.GetValues("Set-Cookie");
                SessionId = enumerable.First(x => x.Contains("JSESSIONID")).Split(';')[0];
                SaveSession();
                return true;
            }
        }

        private void SaveSession()
        {
            using (FileStream stream = new FileStream(SessionFileName, FileMode.CreateNew))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(SessionId);
            }
        }

        private static string GetStringFromSecureString(SecureString password)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}