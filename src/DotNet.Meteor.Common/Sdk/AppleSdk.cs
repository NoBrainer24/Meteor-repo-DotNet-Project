using System;
using System.IO;
using System.Linq;
using DotNet.Meteor.Processes;

namespace DotNet.Meteor.Common {
    public static class AppleSdk {
        public static string XCodePath() {
            var selector = new FileInfo(Path.Combine("/usr", "bin", "xcode-select"));
            ProcessResult result = new ProcessRunner(selector, new ProcessArgumentBuilder()
                .Append("-p"))
                .WaitForExit();

            string path = string.Join(Environment.NewLine, result.StandardOutput)?.Trim();

            if (string.IsNullOrEmpty(path))
                throw new Exception("Could not find XCode path");

            return path;
        }

        public static string SimulatorsLocation() {
            string home = Environment.GetEnvironmentVariable("HOME");
            string path = Path.Combine(home, "Library", "Developer", "CoreSimulator", "Devices");

            if (string.IsNullOrEmpty(path))
                throw new Exception("Could not find simulator path");

            return path;
        }

        public static FileInfo SystemProfilerTool() {
            string path = Path.Combine("/usr", "sbin", "system_profiler");
            var tool = new FileInfo(path);

            if (!tool.Exists)
                throw new Exception("Could not find system_profiler path");

            return tool;
        }

        public static FileInfo MLaunchTool() {
            var mlaunchToolPath = Environment.GetEnvironmentVariable("MLAUNCH_PATH");
            if (File.Exists(mlaunchToolPath))
                return new FileInfo(mlaunchToolPath);

            var dotnetPath = Common.MicrosoftSdk.DotNetRootLocation();
            var sdkPath = Path.Combine(dotnetPath, "packs", "Microsoft.iOS.Sdk");
            if (!Directory.Exists(sdkPath)) {
                var sdkPaths = Directory.GetDirectories(Path.Combine(dotnetPath, "packs"), "Microsoft.iOS.Sdk.net*");
                if (sdkPaths.Length == 0)
                    throw new FileNotFoundException("Could not find mlaunch tool");

                sdkPath = sdkPaths.OrderByDescending(x => Path.GetFileName(x)).First();
            }

            var toolLocations = Directory.GetDirectories(sdkPath);
            if (toolLocations.Length == 0)
                throw new FileNotFoundException("Could not find mlaunch tool");

            var latestToolDirectory = toolLocations.OrderByDescending(x => Path.GetFileName(x)).First();
            mlaunchToolPath = Path.Combine(latestToolDirectory, "tools", "bin", "mlaunch");
            return new FileInfo(mlaunchToolPath);
        }

        public static string IDeviceLocation() {
            string dotnetPath = Common.MicrosoftSdk.DotNetRootLocation();
            string sdkPath = Path.Combine(dotnetPath, "packs", "Microsoft.iOS.Windows.Sdk");
            DirectoryInfo newestTool = null;

            foreach (string directory in Directory.GetDirectories(sdkPath)) {
                string idevicePath = Path.Combine(directory, "tools", "msbuild", "iOS", "imobiledevice-x64");

                if (Directory.Exists(idevicePath)) {
                    var tool = new DirectoryInfo(idevicePath);

                    if (newestTool == null || tool.CreationTime > newestTool.CreationTime)
                        newestTool = tool;
                }
            }

            if (newestTool == null || !newestTool.Exists)
                throw new DirectoryNotFoundException("imobiledevice-x64");

            return newestTool.FullName;
        }

        public static FileInfo XCRunTool() {
            string path = Path.Combine("/usr", "bin", "xcrun");
            FileInfo tool = new FileInfo(path);

            if (!tool.Exists)
                throw new Exception("Could not find xcrun tool");

            return tool;
        }

        public static FileInfo OpenTool() {
            string path = Path.Combine("/usr", "bin", "open");
            FileInfo tool = new FileInfo(path);

            if (!tool.Exists)
                throw new Exception("Could not find open tool");

            return tool;
        }
    }
}