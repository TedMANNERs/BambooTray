namespace BambooTray.App.Model
{
    public class Session
    {
        public Session(string sessionId, string seraphId)
        {
            SessionId = sessionId;
            SeraphId = seraphId;
        }

        public string SessionId { get; set; }
        public string SeraphId { get; set; }
    }
}