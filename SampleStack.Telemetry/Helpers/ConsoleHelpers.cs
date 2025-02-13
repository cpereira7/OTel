using System.Text;

namespace SampleStack.Telemetry.Helpers
{
    internal static class ConsoleHelpers
    {
        public static void DisplayProgress(int current, int total)
        {
            int progressWidth = 50;
            int progress = (int)((double)current / total * progressWidth);
            
            StringBuilder progressBar = new();
            progressBar.Append('[');
            progressBar.Append(new string('#', progress));
            progressBar.Append(new string('-', progressWidth - progress));
            progressBar.Append(']');
            
            Console.Write($"\rProgress: {progressBar} {current}/{total}");
            if (current == total)
            {
                Console.WriteLine();
            }
        }
    }
}
