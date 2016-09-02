namespace BambooTray.App.Bamboo
{
    public interface IRestResponse<out T>
    {
        bool IsSuccess { get; }
        T Data { get; }
    }
}