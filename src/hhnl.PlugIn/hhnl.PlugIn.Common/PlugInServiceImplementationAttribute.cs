using System;
using Microsoft.Extensions.DependencyInjection;

namespace hhnl.PlugIn.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlugInServiceImplementationAttribute : Attribute
    {
        public PlugInServiceImplementationAttribute(ServiceLifetime serviceLifetime)
        {
            ServiceLifetime = serviceLifetime;
        }

        public ServiceLifetime ServiceLifetime { get; }
    }
}