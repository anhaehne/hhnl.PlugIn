using System;
using System.Reflection;
using System.Threading.Tasks;
using hhnl.PlugIn.Common;
using hhnl.PlugIn.Host.Services;
using hhnl.PlugIn.Shared;
using JKang.IpcServiceFramework.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hhnl.PlugIn.Host
{
    public static class Program
    {
        private static readonly MethodInfo _addNamedPipeEndpointMethodInfo =
            typeof(NamedPipeIpcHostBuilderExtensions).GetMethod(nameof(NamedPipeIpcHostBuilderExtensions.AddNamedPipeEndpoint),
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(IIpcHostBuilder), typeof(string) })!;



        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>()
                        .CreateLogger(nameof(Program));
                    
                    // load plugin
                    var pluginServices = PluginLoader.LoadPluginAndDiscoverServices(PluginLoader.HostConfig, logger);

                    // register plugin services
                    foreach (var (service, implementation) in pluginServices)
                    {
                        var lifeTime = GetServiceLifetime(implementation);

                        logger.LogInformation(
                            $"Adding plugin service '{service.Name}' with implementation '{implementation.Name}' and lifetime '{lifeTime}'");

                        services.Add(ServiceDescriptor.Describe(service, implementation, lifeTime));
                    }
                    
                    // register management services
                    services.AddSingleton<IManagementService, ManagementService>();
                })
                .ConfigureIpcHost(builder =>
                {
                    // configure IPC endpoints
                    foreach (var plugInServiceInterface in PluginLoader.PlugInServiceInterfaces!)
                    {
                        _addNamedPipeEndpointMethodInfo.MakeGenericMethod(plugInServiceInterface).Invoke(null,
                            new object?[] { builder, plugInServiceInterface.FullName });
                    }
                    
                    // add management services
                    builder.AddNamedPipeEndpoint<IManagementService>("_management");
                });
        }


        private static ServiceLifetime GetServiceLifetime(Type t)
        {
            return t.GetCustomAttribute<PlugInServiceImplementationAttribute>()?.ServiceLifetime ?? ServiceLifetime.Transient;
        }
    }
}