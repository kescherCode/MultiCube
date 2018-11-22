using Microsoft.Win32;

namespace MultiCube
{
    /// <summary>
    /// Contains properties that change registry settings for the program.
    /// </summary>
    static class RegistrySettings
    {
        private static readonly RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MultiCube");
        /// <summary>
        /// Bool that determines whether the tutorial should be shown or not. Persists in registry.
        /// </summary>
        static public bool ShowTutorial
        {
            get => rk.GetValue("showTutorial", "true").Equals("true");
            set => rk.SetValue("showTutorial", value.ToString().ToLower());
        }
    }
}
