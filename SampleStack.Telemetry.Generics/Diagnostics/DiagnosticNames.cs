using System.Reflection;

namespace SampleStack.Telemetry.Generics.Diagnostics
{
    public static class DiagnosticNames
    {
        public static string ServiceName { get; } = Environment.GetEnvironmentVariable("SERVICE_NAME")
            ?? Assembly.GetEntryAssembly()?.GetName().Name
            ?? "UnknownService";

        public static string ServiceVersion { get; } = Environment.GetEnvironmentVariable("SERVICE_VERSION")
            ?? Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
            ?? "1.0.0";

        public static Dictionary<string, object> Attributes => new()
        {
            ["host.name"] = Environment.MachineName,
            ["host.environment"] = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development",
            ["os.description"] = Environment.OSVersion.VersionString,
            ["service.name"] = ServiceName,
            ["service.version"] = ServiceVersion
        };
    }
}
