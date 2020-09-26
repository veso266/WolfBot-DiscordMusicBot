using System.Diagnostics;


namespace WolfBot.Tools.NativeTools
{
    public static class ProcessHelpers
    {
        public static bool IsRunning(string name) => Process.GetProcessesByName(name).Length > 0;
    }
}
