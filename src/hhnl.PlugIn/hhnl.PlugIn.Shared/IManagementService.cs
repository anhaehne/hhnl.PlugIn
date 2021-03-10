using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hhnl.PlugIn.Shared
{
    public interface IManagementService
    {
        IReadOnlyCollection<Type> GetPlugInServices();
    }
}