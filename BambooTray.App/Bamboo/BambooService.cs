using System.Threading;
using System.Threading.Tasks;
using BambooTray.App.Bamboo.Resources;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.Model;

namespace BambooTray.App.Bamboo
{
    public class BambooService : IBambooService
    {
        private readonly IBambooClient _bambooClient;
        private readonly IBambooPlanPublisher _bambooPlanPublisher;
        private readonly Configuration.Configuration _config;
        private CancellationTokenSource _source;

        public BambooService(IConfigurationManager configurationManager, IBambooPlanPublisher bambooPlanPublisher, IBambooClient bambooClient)
        {
            _bambooPlanPublisher = bambooPlanPublisher;
            _bambooClient = bambooClient;
            _config = configurationManager.Config;
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            foreach (string plan in _config.Plans)
            {
                Task.Run(() => UpdatePlan(plan, _source.Token), _source.Token);
            }
        }

        public void Stop()
        {
            _source.Cancel();
        }

        private async Task UpdatePlan(string planKey, CancellationToken token)
        {
            Plan oldPlan = null;
            Result oldResult = null;

            while (!token.IsCancellationRequested)
            {
                IRestResponse<Plan> planResponse = await _bambooClient.GetAsync<Plan>($"{_config.BambooHostname}rest/api/latest/plan/{planKey}").ConfigureAwait(false);
                if (!planResponse.IsSuccess)
                    continue;

                oldPlan = planResponse.Data;
                if (!planResponse.Data.IsBuilding && oldResult != null)
                    continue;

                IRestResponse<Result> resultResponse = await _bambooClient.GetAsync<Result>($"{_config.BambooHostname}rest/api/latest/result/{planKey}/latest").ConfigureAwait(false);
                if (!resultResponse.IsSuccess)
                    continue;

                if (oldResult == null || !oldResult.Equals(resultResponse.Data))
                {
                    oldResult = resultResponse.Data;
                    BambooPlan newBambooPlan = new BambooPlan
                        {
                            BuildName = oldPlan.BuildName,
                            PlanKey = planKey,
                            BuildState = oldResult.BuildState,
                            ProjectKey = oldPlan.ProjectKey,
                            ProjectName = oldPlan.ProjectName,
                            IsBuilding = oldPlan.IsBuilding
                        };
                    _bambooPlanPublisher.FirePlanChanged(newBambooPlan);
                }
            }
        }
    }
}