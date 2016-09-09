using System.Threading.Tasks;
using BambooTray.App.Model;

namespace BambooTray.App.SessionManagement
{
    public interface IAuthenticator
    {
        Task<Model.Session> Authenticate(LoginCredentials credentials);
    }
}