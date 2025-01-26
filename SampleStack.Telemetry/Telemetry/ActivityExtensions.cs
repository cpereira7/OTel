using System.Diagnostics;

namespace SampleStack.Telemetry.Telemetry
{
    internal static class ActivityExtensions
    {
        public static string GetSpanId(this Activity activity)
        {
            return activity?.IdFormat switch
            {
                ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
                ActivityIdFormat.Hierarchical => activity.Id,
                _ => null
            } ?? string.Empty;
        }

        public static string GetTraceId(this Activity activity)
        {
            return activity?.IdFormat switch
            {
                ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
                ActivityIdFormat.Hierarchical => activity.RootId,
                _ => null
            } ?? string.Empty;
        }

        public static string GetParentSpanId(this Activity activity)
        {
            return activity?.IdFormat switch
            {
                ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
                ActivityIdFormat.Hierarchical => activity.ParentId,
                _ => null
            } ?? string.Empty;
        }
    }
}
