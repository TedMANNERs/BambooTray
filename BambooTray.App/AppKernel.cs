using System;
using Ninject;

namespace BambooTray.App
{
    public class AppKernel : IDisposable
    {
        private static AppKernel _instance;

        private AppKernel()
        {
            Kernel = new StandardKernel();
            Kernel.Load<AppModule>();
        }

        public static AppKernel Instance =>
            _instance ?? (_instance = new AppKernel());

        public IKernel Kernel { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static T Get<T>()
        {
            return Instance.Kernel.Get<T>();
        }

        public static void ClearInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Kernel != null)
                {
                    Kernel.Dispose();
                    Kernel = null;
                }
            }
        }
    }
}