using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Apps.Sdk.Helpers
{
    public static class OperatingSystem
    {
        public static bool IsWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
