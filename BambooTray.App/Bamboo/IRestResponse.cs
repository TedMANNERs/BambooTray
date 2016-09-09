using System.Net;

namespace BambooTray.App.Bamboo
{
    public interface IRestResponse<out T>
    {
        bool IsSuccess { get; }
        HttpStatusCode StatusCode { get; set; }
        T Data { get; }
    }
}