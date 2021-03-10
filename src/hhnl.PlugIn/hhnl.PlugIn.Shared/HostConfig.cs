namespace hhnl.PlugIn.Shared
{
    public class HostConfig
    {
        /// <summary>
        /// The path to the entry .dll file of the plug in.
        /// </summary>
        public string? PlugInDllPath { get; set; }
        

        /// <summary>
        /// The path to the contract .dll file of the plug in.
        /// </summary>
        public string? ContractDllPath { get; set; }
    }
}