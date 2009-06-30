using System.IO;

namespace Frog.Orm
{
    public class Configuration
    {
        public static void Initialize(string appConfigFileName)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(appConfigFileName));
        }
    }
}
