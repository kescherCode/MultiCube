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
        private static readonly string ConfigDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MultiCube");

        private static readonly string ConfigFile =
            Path.Combine(ConfigDirectory, "MultiCube.config");

        private static readonly Dictionary<string, string> Config = new();

        static Settings()
        {
            try
            {
                if (!File.Exists(ConfigFile))
                {
                    Directory.CreateDirectory(ConfigDirectory);
                    ShowIntro = ShowIntro;
                }
            }
            catch
            {
                // Probably a permission or filesystem issue we can't fix
            }

            try
            {
                using var r = new StreamReader(ConfigFile, UTF8);
                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line == null || line.StartsWith('#')) continue;
                    var values = line?.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (values is { Length: 2 })
                        Config[values[0]] = values[1];
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
            get =>
                !Config.TryGetValue("showIntro", out var configStr) ||
                !bool.TryParse(configStr.ToLower(), out var val) || val;
            set => WriteToConfig(new KeyValuePair<string, string>("showIntro", value.ToString()));
        }

        private static void WriteToConfig(KeyValuePair<string, string> configPair)
        {
            Config[configPair.Key] = configPair.Value;
            try
            {
                using (var w = new StreamWriter(ConfigFile + ".tmp", false))
                {
                    try
                    {
                        using var r = new StreamReader(ConfigFile, UTF8);
                        var applied = false;
                        while (!r.EndOfStream)
                        {
                            var line = r.ReadLine();
                            var pair = line?.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                            if (pair is not { Length: 2 }) continue;

                            if (pair[0].StartsWith(configPair.Key) && !applied)
                            {
                                w.WriteLine($"{configPair.Key}:{configPair.Value}");
                                applied = true;
                            }
                            else
                            {
                                w.WriteLine(line);
                            }
                        }

                        if (!applied) w.WriteLine($"{configPair.Key}:{configPair.Value}");
                    }
                    catch
                    {
                        // Probably a permission or filesystem issue we can't fix
                    }
                }

                try
                {
                    File.Move(ConfigFile + ".tmp", ConfigFile, true);
                }
                catch
                {
                    // Probably a permission or filesystem issue we can't fix
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
    }
}