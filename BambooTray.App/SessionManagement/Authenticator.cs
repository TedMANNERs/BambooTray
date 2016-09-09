using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using BambooTray.App.Configuration;
using BambooTray.App.Model;

namespace BambooTray.App.SessionManagement
{
    public class Authenticator : IAuthenticator
    {
        private readonly Configuration.Configuration _config;

        public Authenticator(IConfigurationManager configurationManager)
        {
            _config = configurationManager.Config;
        }

        public async Task<Model.Session> Authenticate(LoginCredentials credentials)
        {
            using (HttpClientHandler handle = new HttpClientHandler { UseCookies = false })
            using (HttpClient client = new HttpClient(handle))
            {
                HttpRequestMessage post = new HttpRequestMessage(HttpMethod.Post, $"{_config.BambooHostname}/userlogin.action")
                    {
                        Content = new FormUrlEncodedContent(
                            new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("os_username", credentials.Username),
                                    new KeyValuePair<string, string>("os_password", GetStringFromSecureString(credentials.Password)),
                                    new KeyValuePair<string, string>("os_cookie", "true")
                                })
                    };
                HttpResponseMessage postResponse = await client.SendAsync(post).ConfigureAwait(false);
                if (!postResponse.IsSuccessStatusCode)
                    return null;

                IEnumerable<string> enumerable = postResponse.Headers.GetValues("Set-Cookie");
                string sessionId = enumerable.First(x => x.Contains("JSESSIONID")).Split(';')[0];
                return new Model.Session(sessionId);
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