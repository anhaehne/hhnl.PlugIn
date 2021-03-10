using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using hhnl.PlugIn.Common;
using hhnl.PlugIn.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hhnl.PlugIn.Host
{
    public class PluginLoader
    {
        private static Type[]? _hostServiceInterfaces;
        private static Type[]? _plugInServiceInterfaces;
        private static Type[]? _contractTypes;
        private static HostConfig? _hostConfig;


        public static Type[] PlugInServiceInterfaces
        {
            get
            {
                if (_plugInServiceInterfaces is null)
                    LoadContract();

                return _plugInServiceInterfaces!;
            }
        }

        public static Type[] HostServiceInterfaces
        {
            get
            {
                if (_hostServiceInterfaces is null)
                    LoadContract();

                return _hostServiceInterfaces!;
            }
        }

        public static Type[] ContractTypes
        {
            get
            {
                if (_contractTypes is null)
                    LoadContract();

                return _contractTypes!;
            }
        }

        public static Type[]? ImplementedPlugInServices { get; private set; }

        public static HostConfig HostConfig => _hostConfig ??= GetConfiguration();

        public static IEnumerable<(Type service, Type implementation)> LoadPluginAndDiscoverServices(
            HostConfig config,
            ILogger logger)
        {
            var plugInDll = new FileInfo(Path.GetFullPath(config.PlugInDllPath ??
                                                          throw new ArgumentNullException(
                                                              $"{nameof(HostConfig.PlugInDllPath)} is null.")));

            if (PlugInServiceInterfaces.Length == 0 && HostServiceInterfaces.Length == 0)
                throw new InvalidOperationException("Not host or client services found in contract.");

            logger.LogInformation($"Loading plugin '{plugInDll.FullName}'.");

            // Load plugin
            var plugInLoader = McMaster.NETCore.Plugins.PluginLoader.CreateFromAssemblyFile(
                plugInDll.FullName,
                ContractTypes);

            var pluginAssembly = plugInLoader.LoadDefaultAssembly();

            var result = GetPlugInServiceImplementations(PlugInServiceInterfaces, pluginAssembly.GetTypes()).ToList();

            ImplementedPlugInServices = result.Select(x => x.service).Distinct().ToArray();
            
            return result;
        }

        private static IEnumerable<(Type service, Type implementation)> GetPlugInServiceImplementations(
            IReadOnlyCollection<Type> plugInServices,
            IEnumerable<Type> assemblyTypes)
        {
            foreach (var type in assemblyTypes)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var services = plugInServices.Where(type.IsAssignableTo);

                foreach (var service in services)
                {
                    yield return (service, type);
                }
            }
        }


        private static HostConfig GetConfiguration()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(c => c.AddCommandLine(Environment.GetCommandLineArgs())).Build()
                .Services
                .GetService<IConfiguration>()
                .Get<HostConfig>();
        }

        private static void LoadContract()
        {
            var contractDll = new FileInfo(Path.GetFullPath(HostConfig.ContractDllPath ??
                                                            throw new ArgumentNullException(
                                                                $"{nameof(HostConfig.ContractDllPath)} is null.")));

            var contract = Assembly.LoadFrom(contractDll.FullName);

            _contractTypes = contract.GetTypes();
            _plugInServiceInterfaces = GetInterfacesWithAttribute<PlugInServiceAttribute>(_contractTypes);
            _hostServiceInterfaces = GetInterfacesWithAttribute<HostServiceAttribute>(_contractTypes);
        }

        private static Type[] GetInterfacesWithAttribute<T>(Type[] types) where T : Attribute
        {
            return types.Where(t => t.IsInterface && t.CustomAttributes.Any(attr => attr.AttributeType == typeof(T))).ToArray();
        }
    }
}