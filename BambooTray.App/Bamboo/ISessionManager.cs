using System.Security;
using System.Threading.Tasks;

namespace BambooTray.App.Bamboo
{
    public interface ISessionManager
    {
        void LoadSession();

        Task<bool> CreateSession(string username, SecureString password);
    }
}