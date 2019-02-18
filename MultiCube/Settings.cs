using System;
using System.Collections.Generic;
using System.IO;
using static System.Text.Encoding;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    ///     Contains properties that change/get settings for the program.
    /// </summary>
    public static class Settings
    {
        private const string ConfigFile = "./MultiCube.config";

        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>();

        static Settings()
        {
            if (!File.Exists(ConfigFile))
                File.AppendAllText(ConfigFile, string.Empty, UTF8);

            try
            {
                using (var r = new StreamReader(ConfigFile, UTF8))
                {
                    while (!r.EndOfStream)
                    {
                        string[] values = r.ReadLine()?.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (values != null && values.Length == 2)
                            Config[values[0]] = values[1];
                    }
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        /// <summary>
        ///     Bool that determines whether the tutorial should be shown or not. Persists in config file. Defaults to true if not
        ///     set.
        /// </summary>
        public static bool ShowIntro
        {
            // Default value: true
            // Try getting value from Config. If unsuccessful, return default value, else: Try to parse value. If unsuccessful, return default value, else: return parsed value;
            get =>
                !Config.TryGetValue("showIntro", out string configStr) || !bool.TryParse(configStr.ToLower(), out bool val) || val;
            set => WriteToConfig(new KeyValuePair<string, string>("showIntro", value.ToString()));
        }

        private static void WriteToConfig(KeyValuePair<string, string> configPair)
        {
            Config[configPair.Key] = configPair.Value;
            try
            {
                using (var w = new StreamWriter(ConfigFile + ".tmp", false))
                {
                    using (var r = new StreamReader(ConfigFile, UTF8))
                    {
                        bool applied = false;
                        while (!r.EndOfStream)
                        {
                            string line = r.ReadLine();
                            string[] pair = line?.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                            if (pair == null || pair.Length >= 2) continue;

                            if (pair[0].Contains(configPair.Key) && !applied)
                            {
                                w.WriteLine($"{configPair.Key}:{configPair.Value}");
                                applied = true;
                            }
                            else w.WriteLine(line);
                        }

                        if (!applied) w.WriteLine($"{configPair.Key}:{configPair.Value}");
                    }
                }

                using (var w = new StreamWriter(ConfigFile, false))
                {
                    using (var r = new StreamReader(ConfigFile + ".tmp", UTF8))
                    {
                        while (!r.EndOfStream)
                        w.WriteLine(r.ReadLine());
                    }
                }

                try
                {
                    if (File.Exists(ConfigFile + ".tmp")) File.Delete(ConfigFile + ".tmp");
                }
                catch (Exception e)
                {
                    // If we can't delete .tmp, we don't care.
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
    }
}