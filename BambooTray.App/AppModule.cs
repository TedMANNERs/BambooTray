using BambooTray.App.Bamboo;
using BambooTray.App.Configuration;
using BambooTray.App.EventBroker;
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
            Bind<IBambooService>().To<BambooService>().InSingletonScope();
            Bind<IBambooClient>().To<BambooClient>();

            Bind<IPopupViewModel>().To<PopupViewModel>().OnActivation(x => x.Load()).RegisterOnEventBroker(EventBrokerName);
            Bind<ILoginViewModel>().To<LoginViewModel>();

            Bind<ISessionManager>().To<SessionManager>().InSingletonScope();
        }
    }
}