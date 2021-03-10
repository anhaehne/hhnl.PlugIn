using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using hhnl.PlugIn.Shared;
using hhnl.ProcessIsolation;
using JKang.IpcServiceFramework.Client;
using Microsoft.Extensions.DependencyInjection;
using FileAccess = hhnl.ProcessIsolation.FileAccess;

namespace hhnl.PlugIn
{
    public class PlugInManager
    {
        private readonly IProcessIsolator _processIsolator;

        public PlugInManager(IProcessIsolator processIsolator)
        {
            _processIsolator = processIsolator;
        }

        public async Task<PlugIn> LoadPluginAsync(string pluginHostPath, string plugInDllPath, string contractDllPath)
        {
            var plugInHost = new FileInfo(pluginHostPath);

            if (!plugInHost.Exists)
                throw new ArgumentException("Plugin host does not exist.");
            
            var plugInDll = new FileInfo(plugInDllPath);

            if (!plugInDll.Exists)
                throw new ArgumentException("Plugin dll does not exist.");

            var contractDll = new FileInfo(contractDllPath);

            if (!contractDll.Exists)
                throw new ArgumentException("Contract dll does not exist.");

            var p = _processIsolator.StartIsolatedProcess(plugInDll.Name,
                plugInHost.FullName,
                GetCommandLineArguments(plugInDll.FullName, contractDll.FullName),
                fileAccess: new[]
                {
                    new FileAccess(contractDll.DirectoryName!, FileAccess.Right.Read),
                    new FileAccess(plugInDll.DirectoryName!, FileAccess.Right.Read)
                });

            return await PlugIn.CreateAsync(p);
        }


        private string[] GetCommandLineArguments(string plugInDllPath, string contractDllPath)
        {
            return new[]
            {
                $"--{nameof(HostConfig.PlugInDllPath)} {plugInDllPath}",
                $"--{nameof(HostConfig.ContractDllPath)} {contractDllPath}"
            };
        }

        private int GetAvailableUdpPort()
        {
            var udp = new TcpClient(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)udp.Client.LocalEndPoint).Port;
        }
    }

    public class PlugIn
    {
        private IIpcClient<IManagementService> _managementServiceClient;

        private PlugIn(IIsolatedProcess process, IIpcClient<IManagementService> managementServiceClient)
        {
            _managementServiceClient = managementServiceClient;
            Process = process;
        }

        public IIsolatedProcess Process { get; }

        public T GetService<T>()
        {
            return default;
        }


        public static async Task<PlugIn> CreateAsync(IIsolatedProcess process)
        {
            var tempServices = new ServiceCollection()
                .AddNamedPipeIpcClient<IManagementService>(nameof(PlugIn), "_management")
                .BuildServiceProvider();

            var managementServiceClient = tempServices
                .GetRequiredService<IIpcClientFactory<IManagementService>>()
                .CreateClient(nameof(PlugIn));

            var implementedPlugInTypes = await managementServiceClient.InvokeAsync(c => c.GetPlugInServices());

            return null;
        }

        private void ConfigureService(Type t, IServiceCollection services)
        {
        }

        private T GetService<T>(IServiceProvider serviceProvider)
        {
            return default;
        }

        private IIpcClient<T> GetServiceClient<T>(IServiceProvider serviceProvider) where T : class
        {
            return default;
        }

        private string GetFullPipeName(string t)
        {
            return t;
        }
    }
}