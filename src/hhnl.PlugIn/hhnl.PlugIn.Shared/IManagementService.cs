using System;
using System.Collections.Generic;

namespace hhnl.PlugIn.Shared
{
    public interface IManagementService
    {
        IReadOnlyCollection<Type> GetPlugInServices();
    }
}