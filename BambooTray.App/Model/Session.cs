namespace BambooTray.App.Model
{
    public class Session
    {

        public Session(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; set; }
    }
}