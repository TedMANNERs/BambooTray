using System;
using BambooTray.App.Model;

namespace BambooTray.App.Bamboo
{
    public interface IBambooPlanUpdater
    {
        void Start(Session session, EventHandler onSessionExpired);

        void Stop();
    }
}