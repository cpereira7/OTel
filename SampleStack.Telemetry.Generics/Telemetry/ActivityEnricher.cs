using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace SampleStack.Telemetry.Generics.Telemetry
{
    public class ActivityEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity?.GetTraceId()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity?.GetSpanId()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentSpanId", activity?.GetParentSpanId()));
        }
    }
}
