using System;
using Appccelerate.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.EventBroker
{
    public class BambooPlanPublisher : IBambooPlanPublisher
    {
        public void FirePlanChanged(BambooPlan plan)
        {
            PlanChanged?.Invoke(this, new PlanEventArgs(plan));
        }

        [EventPublication("topic://BambooTray/PlanChanged")]
        public event EventHandler<PlanEventArgs> PlanChanged;
    }
}