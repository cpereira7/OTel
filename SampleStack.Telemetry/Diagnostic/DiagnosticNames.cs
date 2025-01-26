namespace SampleStack.Telemetry.Telemetry
{
    internal static class DiagnosticNames
    {
        public static string ServiceName => "samplestack";
        public static string ServiceVersion => "1.0.0";

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
