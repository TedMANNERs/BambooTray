namespace BambooTray.App.Bamboo
{
    public class RestResponse<T> : IRestResponse<T> where T : class
    {
        public RestResponse(bool isSuccess, T data)
        {
            IsSuccess = isSuccess;
            Data = data;
        }

        public RestResponse()
        {
        }

        public bool IsSuccess { get; }
        public T Data { get; }
    }
}