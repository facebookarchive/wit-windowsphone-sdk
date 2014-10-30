using System.Diagnostics;

namespace Witai
{
    internal class WitLog
    {
        public static bool IsLoggingEnabled = false;

        public static void Log(string title, string text)
        {
            if (IsLoggingEnabled)
            {
                Debug.WriteLine(title + " : " + text);
            }
        }

        public static void Log(string text)
        {
            if (IsLoggingEnabled)
            {
                Debug.WriteLine("Wit : " + text);
            }
        }
    }
}
