using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BambooTray.App.Bamboo
{
    public class BambooClient : IBambooClient, IDisposable
    {
        private readonly HttpClient _client;
        private bool _disposed;

        public BambooClient()
        {
            HttpClientHandler handler = new HttpClientHandler { UseCookies = false };
            _client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(60000) };
        }

        public async Task<IRestResponse<T>> GetAsync<T>(string url, string sessionId) where T : class
        {
            IRestResponse<T> restResponse = new RestResponse<T>();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Cookie", sessionId);
                HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T resource = (T)serializer.Deserialize(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    restResponse = new RestResponse<T>(response.IsSuccessStatusCode, resource);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewCreator.ShowView<LoginView, LoginViewModel>();
                }
            }
            catch (HttpRequestException)
            {
                //ignore
            }
            catch (TaskCanceledException)
            {
                //ignore
            }

            return restResponse;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }
    }
}