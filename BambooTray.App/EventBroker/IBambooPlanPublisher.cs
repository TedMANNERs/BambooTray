using System;
using BambooTray.App.Model;

namespace BambooTray.App.EventBroker
{
    public interface IBambooPlanPublisher
    {
        void FirePlanChanged(BambooPlan plan);

        event EventHandler<PlanEventArgs> PlanChanged;
    }
}