using System;
using System.Collections.Generic;
using hhnl.PlugIn.Shared;

namespace hhnl.PlugIn.Host.Services
{
    public class ManagementService : IManagementService
    {
        public IReadOnlyCollection<Type> GetPlugInServices()
        {
            if (PluginLoader.ImplementedPlugInServices is null)
                throw new InvalidOperationException("Plugin not initialized.");

            return PluginLoader.ImplementedPlugInServices;
        }
    }
}