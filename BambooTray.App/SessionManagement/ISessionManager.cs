using System.Threading.Tasks;

namespace BambooTray.App.SessionManagement
{
    public interface ISessionManager
    {
        Task OpenSession();

        void CloseSession();
    }
}