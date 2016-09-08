using System.Collections.Generic;
using System.Linq;
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

        public BambooService(
            IConfigurationManager configurationManager,
            IBambooPlanPublisher bambooPlanPublisher,
            IBambooClient bambooClient)
        {
            _bambooPlanPublisher = bambooPlanPublisher;
            _bambooClient = bambooClient;
            _config = configurationManager.Config;
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            Task.Run(() => UpdatePlan(_source.Token), _source.Token);
        }

        public void Stop()
        {
            _source.Cancel();
        }

        private async Task UpdatePlan(CancellationToken token)
        {
            Dictionary<string, Result> oldResults = new Dictionary<string, Result>();

            while (!token.IsCancellationRequested)
            {
                Plans plans = await GetFavouritePlans().ConfigureAwait(false);
                if (plans != null)
                {
                    foreach (Plan plan in plans.PlanList)
                    {
                        if (plan.IsBuilding && oldResults.Any(x => x.Key == plan.PlanKey.Key))
                            continue;

                        Result newResult = await GetLatestBuild(plan.PlanKey.Key).ConfigureAwait(false);
                        if (newResult == null)
                            continue;

                        if (oldResults.Any(x => x.Key == plan.PlanKey.Key) && newResult.Equals(oldResults[plan.PlanKey.Key]))
                            continue;

                        oldResults[plan.PlanKey.Key] = newResult;
                        BambooPlan newBambooPlan = new BambooPlan
                            {
                                BuildName = plan.BuildName,
                                PlanKey = plan.PlanKey.Key,
                                BuildState = newResult.BuildState,
                                ProjectKey = plan.ProjectKey,
                                ProjectName = plan.ProjectName,
                                IsBuilding = plan.IsBuilding
                            };
                        _bambooPlanPublisher.FirePlanChanged(newBambooPlan);
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private Task<Plans> GetFavouritePlans()
        {
            return GetResource<Plans>($"{_config.BambooHostname}rest/api/latest/plan/?favourite&expand=plans.plan");
        }

        private Task<Result> GetLatestBuild(string planKey)
        {
            return GetResource<Result>($"{_config.BambooHostname}rest/api/latest/result/{planKey}/latest");
        }

        private async Task<T> GetResource<T>(string url) where T : class
        {
            IRestResponse<T> resultResponse = await _bambooClient.GetAsync<T>(url, SessionManager.SessionId).ConfigureAwait(false);
            return resultResponse.IsSuccess
                ? resultResponse.Data
                : null;
        }
    }
}