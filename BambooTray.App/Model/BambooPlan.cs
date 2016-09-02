using BambooTray.App.Bamboo.Resources;

namespace BambooTray.App.Model
{
    public class BambooPlan
    {
        public string PlanKey { get; set; }
        public BuildState BuildState { get; set; }
        public string ProjectKey { get; set; }
        public string ProjectName { get; set; }
        public string BuildName { get; set; }
    }
}