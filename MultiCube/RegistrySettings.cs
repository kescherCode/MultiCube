using Microsoft.Win32;

namespace MultiCube
{
    static class RegistrySettings
    {
        private static readonly RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MultiCube");
        static public bool ShowTutorial
        {
            get => rk.GetValue("showTutorial", "true").Equals("true");
            set => rk.SetValue("showTutorial", value.ToString());
        }
    }
}
