using System.Threading.Tasks;
using hhnl.PlugIn.Common;
using hhnl.PlugIn.Tests.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace hhnl.PlugIn.Tests.TestPlugin
{
    [PlugInServiceImplementation(ServiceLifetime.Singleton)]
    public class TestPluginService : ITestPluginService
    {
        public Task<string> AddTestToStringAsync(string param)
        {
            return Task.FromResult(param + "Test");
        }
    }
}