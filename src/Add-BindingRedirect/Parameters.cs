using Serilog.Events;
using Yaclp.Attributes;

namespace Add_BindingRedirect
{
    internal class Parameters
    {
        [ParameterIsOptional]
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Verbose;

        [ParameterIsOptional]
        public string Directory { get; set; }

        [ParameterIsOptional]
        public string ExcludeAssemblyRegex { get; set; }

        public Parameters()
        {
            Directory = System.IO.Directory.GetCurrentDirectory();
        }
    }
}