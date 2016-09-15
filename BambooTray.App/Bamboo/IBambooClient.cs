using System.Threading.Tasks;
using BambooTray.App.Model;

namespace BambooTray.App.Bamboo
{
    public interface IBambooClient
    {
        Task<IRestResponse<T>> GetAsync<T>(string url, Session session) where T : class;
    }
}