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

        public async Task<Session> Authenticate(LoginCredentials credentials)
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

                IEnumerable<string> seraphHeaders = postResponse.Headers.GetValues("X-Seraph-LoginReason");
                string xSeraphLoginReason = seraphHeaders.First();

                if (!postResponse.IsSuccessStatusCode || xSeraphLoginReason != XSeraphLoginReason.Ok)
                    return null;

                IList<string> sessionHeaders = postResponse.Headers.GetValues("Set-Cookie").ToList();
                string sessionId = sessionHeaders.First(x => x.Contains("JSESSIONID")).Split(';')[0];
                string seraphId = sessionHeaders.First(x => x.Contains("seraph")).Split(';')[0];
                return new Session(sessionId, seraphId);
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

    internal static class XSeraphLoginReason
    {
        public const string Ok = "OK";
        public const string AuthenticatedFailed = "AUTHENTICATED_FAILED";
        public const string AuthenticatedDenied = "AUTHENTICATED_DENIED";
    }
}