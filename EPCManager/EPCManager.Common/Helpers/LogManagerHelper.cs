using EPCManager.Common.Logging;
using log4net.Config;

namespace EPCManager.Common.Helpers
{
    public static class LogManagerHelper
    {
        private static ILogManager manager = null;

        static LogManagerHelper()
        {
            XmlConfigurator.Configure();
        }
        
        public static ILogManager GetLogManager()
        {
            if (manager == null)
                manager = new LogManagerAdapter();

            return manager;
        }
    }
}
