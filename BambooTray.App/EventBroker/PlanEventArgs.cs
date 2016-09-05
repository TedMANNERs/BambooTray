using System;
using BambooTray.App.Model;

namespace BambooTray.App.EventBroker
{
    public class PlanEventArgs : EventArgs
    {
        public PlanEventArgs(BambooPlan plan)
        {
            Plan = plan;
        }

        public BambooPlan Plan { get; set; }
    }
}