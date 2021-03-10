using System.Threading.Tasks;
using hhnl.PlugIn.Common;

namespace hhnl.PlugIn.Tests.Contract
{
    [PlugInService]
    public interface ITestPluginService
    {
        Task<string> AddTestToStringAsync(string param);
    }
}