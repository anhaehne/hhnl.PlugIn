using System;
using System.Threading.Tasks;
using hhnl.PlugIn;
using hhnl.ProcessIsolation.Windows;

namespace hhnl.Plugin.TestApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pluginPath = "../../../../hhnl.PlugIn.Tests.TestPlugin/bin/Debug/netstandard2.0/hhnl.PlugIn.Tests.TestPlugIn.dll";
            var contractPath = "../../../../hhnl.PlugIn.Tests.Contract/bin/Debug/netstandard2.0/hhnl.PlugIn.Tests.Contract.dll";
            var hostPath = "../../../../hhnl.PlugIn.Host/bin/Debug/net6.0/hhnl.PlugIn.Host.exe";

            var loader = new PlugInManager(new AppContainerIsolator());
            await loader.LoadPluginAsync(hostPath, pluginPath, contractPath);
        }
    }
}