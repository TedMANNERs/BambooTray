using System.Threading.Tasks;

namespace BambooTray.App.Bamboo
{
    public interface IBambooClient
    {
        Task<IRestResponse<T>> GetAsync<T>(string url, string sessionId) where T : class;
    }
}