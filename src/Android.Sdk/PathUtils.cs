using System;
using System.IO;
using DotNet.Mobile.Shared;

namespace Android.Sdk {
    public static class PathUtils {
        public static string ExecExtension => RuntimeSystem.IsWindows ? ".exe" : "";

        public static string SdkLocation() {
            string path = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            string home = RuntimeSystem.IsWindows
                ? Environment.GetEnvironmentVariable("HOMEPATH")
                : Environment.GetEnvironmentVariable("HOME");

            if (string.IsNullOrEmpty(path))
                path = RuntimeSystem.IsWindows
                ? Path.Combine(home, "AppData", "Local", "Android", "Sdk")
                : Path.Combine(home, "Library", "Android", "sdk");

            if (!Directory.Exists(path))
                throw new Exception("Could not find Android SDK path");

            return path;
        }

        public static string AvdLocation() {
            string home = RuntimeSystem.IsWindows
                ? Environment.GetEnvironmentVariable("HOMEPATH")
                : Environment.GetEnvironmentVariable("HOME");
            return Path.Combine(home, ".android", "avd");
        }

        public static FileInfo AdbTool() {
            string sdk = PathUtils.SdkLocation();
            string path = Path.Combine(sdk, "platform-tools", "adb" + ExecExtension);

            if (!File.Exists(path))
                throw new Exception("Could not find adb tool");

            return new FileInfo(path);
        }

        public static FileInfo EmulatorTool() {
            string sdk = PathUtils.SdkLocation();
            string path = Path.Combine(sdk, "emulator", "emulator" + ExecExtension);

            if (!File.Exists(path))
                throw new Exception("Could not find emulator tool");

            return new FileInfo(path);
        }

        public static FileInfo AvdTool() {
            string sdk = PathUtils.SdkLocation();
            string tools = Path.Combine(sdk, "cmdline-tools");
            FileInfo newestTool = null;

            foreach (string directory in Directory.GetDirectories(tools)) {
                string avdPath = Path.Combine(directory, "bin", "avdmanager" + ExecExtension);

                if (File.Exists(avdPath)) {
                    var tool = new FileInfo(avdPath);

                    if (newestTool == null || tool.CreationTime > newestTool.CreationTime)
                        newestTool = tool;
                }
            }

            if (newestTool == null || !newestTool.Exists)
                throw new Exception("Could not find avdmanager tool");

            return newestTool;
        }
    }
}