using System.Diagnostics;

namespace SampleStack.Telemetry.Diagnostic
{
    public static class DiagnosticActivity
    {
        public static ActivitySource ActivitySource => new(DiagnosticNames.ServiceName, DiagnosticNames.ServiceVersion);

        public static Activity? StartActivity(string activityName, object? parameter = null)
        {
            if (string.IsNullOrEmpty(activityName))
                return null;

            var activity = ActivitySource.CreateActivity(activityName, ActivityKind.Internal);

            if (activity != null)
            {
                activity.Start();

                if (parameter != null)
                {
                    activity.AddTag("parameter", parameter.ToString());
                }
            }

            return activity;
        }
    }
}
