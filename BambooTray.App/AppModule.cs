using BambooTray.App.Bamboo;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
using BambooTray.App.SessionManagement;
using Ninject.Extensions.AppccelerateEventBroker;
using Ninject.Modules;

namespace BambooTray.App
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            const string EventBrokerName = "BambooTrayEventBroker";
            this.AddGlobalEventBroker(EventBrokerName);

            Bind<IConfigurationManager>().To<ConfigurationManager>().InSingletonScope().OnActivation(x => x.Load());
            Bind<IBambooPlanPublisher>().To<BambooPlanPublisher>().RegisterOnEventBroker(EventBrokerName);
            Bind<IBambooPlanUpdater>().To<BambooPlanUpdater>().InSingletonScope();
            Bind<IBambooClient>().To<BambooClient>();

            Bind<IMainViewModel>().To<MainViewModel>().OnActivation(x => x.Load());
            Bind<IPopupViewModel>().To<PopupViewModel>().RegisterOnEventBroker(EventBrokerName);
            Bind<ILoginViewModel>().To<LoginViewModel>();

            Bind<ISessionManager>().To<SessionManager>();
            Bind<IAuthenticator>().To<Authenticator>();
            Bind<ILoginDialogService>().To<LoginDialogService>();
        }
    }
}