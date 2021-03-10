using System;
using Microsoft.Extensions.DependencyInjection;

namespace hhnl.PlugIn.Common
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class PlugInServiceAttribute : Attribute
    {
    }
}