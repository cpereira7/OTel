using System.Diagnostics;

namespace SampleStack.Telemetry.Generics.Diagnostics
{
    public static class DiagnosticActivity
    {
        static ActivitySource ActivitySource => new(DiagnosticNames.ServiceName, DiagnosticNames.ServiceVersion);


        /// <summary>
        /// Starts a new diagnostic activity with the specified name and optional parameter.
        /// </summary>
        /// <param name="activityName">The name of the activity to start.</param>
        /// <param name="parameter">An optional parameter to add as a tag to the activity.</param>
        /// <returns>
        /// The started <see cref="Activity"/> if the activity name is not null or empty; otherwise, null.
        /// </returns>
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
