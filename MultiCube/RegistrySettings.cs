using Microsoft.Win32;

namespace MultiCube
{
    /// <summary>
    /// Contains properties that change registry settings for the program.
    /// </summary>
    internal static class RegistrySettings
    {
        private static readonly RegistryKey Key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MultiCube");

        /// <summary>
        /// Bool that determines whether the tutorial should be shown or not. Persists in registry.
        /// </summary>
        public static bool ShowTutorial
        {
            get => Key.GetValue("showTutorial", "true").Equals("true");
            set => Key.SetValue("showTutorial", value.ToString().ToLower());
        }
    }
}