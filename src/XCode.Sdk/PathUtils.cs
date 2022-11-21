using System;
using System.IO;
using DotNet.Mobile.Shared;

namespace XCode.Sdk {
    public static class PathUtils {
        public static string GetXCodePath() {
            ProcessResult result = ProcessRunner.Run(
                new FileInfo("/usr/bin/xcode-select"),
                new ProcessArgumentBuilder().Append("-p")
            );

            string path = string.Join(Environment.NewLine, result.StandardOutput)?.Trim();

            if (string.IsNullOrEmpty(path))
                throw new Exception("Could not find XCode path");

            return path;
        }

        public static FileInfo GetMLaunch() {
            string dotnetPath = Path.Combine("usr", "local", "share", "dotnet");
            string sdkPath = Path.Combine(dotnetPath, "packs", "Microsoft.iOS.Sdk");
            FileInfo newestTool = null;

            foreach (string directory in Directory.GetDirectories(sdkPath)) {
                string mlaunchPath = Path.Combine(directory, "tools", "bin", "mlaunch");

                if (File.Exists(mlaunchPath)) {
                    var tool = new FileInfo(mlaunchPath);

                    if (newestTool == null || tool.CreationTime > newestTool.CreationTime)
                        newestTool = tool;
                }
            }

            return newestTool;
        }
        public static FileInfo GetXCDeviceTool() {
            string path = Path.Combine(GetXCodePath(), "usr", "bin", "xcdevice");
            FileInfo tool = new FileInfo(path);

            if (!tool.Exists)
                throw new Exception("Could not find xcdevice tool");

            return tool;
        }

         public static FileInfo GetXCRunTool() {
            string path = Path.Combine("/usr", "bin", "xcrun");
            FileInfo tool = new FileInfo(path);

            if (!tool.Exists)
                throw new Exception("Could not find xcrun tool");

            return tool;
        }
    }
}