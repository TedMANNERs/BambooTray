using System;
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
            _client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(60000) };
        }

        public async Task<IRestResponse<T>> GetAsync<T>(string url) where T : class
        {
            IRestResponse<T> restResponse = new RestResponse<T>();
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T resource = (T)serializer.Deserialize(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    restResponse = new RestResponse<T>(response.IsSuccessStatusCode, resource);
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