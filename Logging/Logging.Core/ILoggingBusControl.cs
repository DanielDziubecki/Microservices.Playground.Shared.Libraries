using System.Threading.Tasks;
using Logging.Model;

namespace Logging.Core
{
    public interface ILoggingBusControl
    {
        Task Log<T>(T log) where T : ILog;
    }
}