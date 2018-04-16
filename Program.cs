using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
#if NETCOREAPP2_0
using System.Runtime.InteropServices;
#else
using Microsoft.Win32;
#endif

namespace CoreThrower
{
    class Program
    {
        static void Main(string[] args)
        {


            var framework = TargetFramework();
            Console.WriteLine($@"Built against:                                          {framework}");
            Console.WriteLine($@"Environment.Version:                                    {Safe(() => Environment.Version.ToString())}");
            Console.WriteLine($@"Dns.GetHostName():                                      {Safe(() => Dns.GetHostName().ToString())}");
            Console.WriteLine($@"Environment.MachineName:                                {Safe(() => Environment.MachineName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.DisplayName:                         {Safe(() => TimeZoneInfo.Local.DisplayName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.Id:                                  {Safe(() => TimeZoneInfo.Local.Id.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.StandardName:                        {Safe(() => TimeZoneInfo.Local.StandardName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.DaylightName:                        {Safe(() => TimeZoneInfo.Local.DaylightName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.BaseUtcOffset:                       {Safe(() => TimeZoneInfo.Local.BaseUtcOffset.ToString())}");
            Console.WriteLine($@"DateTimeOffset.Now.Offset:                              {Safe(() => DateTimeOffset.Now.Offset.ToString())}");
            Console.WriteLine($@"HOSTTYPE:                                               {Environment.GetEnvironmentVariable("HOSTTYPE", EnvironmentVariableTarget.Process)}");
            Console.WriteLine($@"PROCESSOR_ARCHITECTURE:                                 {Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine)}");
            Console.WriteLine($@"Environment.OSVersion:                                  {Safe(() => Environment.OSVersion.ToString())}");
            Console.WriteLine($@"MonoVersion():                                          {Safe(MonoVersion)}");
            Console.WriteLine($@"MsCorLibVer():                                          {Safe(MsCorLibVer)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryMethod().Name:                  {Safe(() => GetAssemblyNameFromStackTrace()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryMethod().Version:               {Safe(() => GetAssemblyNameFromStackTrace()?.Version.ToString())}");
            Console.WriteLine($@"GetAssemblyNameFromEntryAssembly().Name:                {Safe(() => GetAssemblyNameFromEntryAssembly()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryAssembly().Version:             {Safe(() => GetAssemblyNameFromEntryAssembly()?.Version.ToString())}");
            Console.WriteLine($@"GetAssemblyNameFromCurrentProcess().Name:               {Safe(() => GetAssemblyNameFromCurrentProcess()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromCurrentProcess().Version:            {Safe(() => GetAssemblyNameFromCurrentProcess()?.Version.ToString())}");
            Console.WriteLine($@"Process.GetCurrentProcess().StartTime:                  {Safe(Process.GetCurrentProcess().StartTime.ToString)}");
#if NET20 || NET35 || NET40
            Console.WriteLine($@"GetVersionFromRegistry()():                             {Safe(GetPreNet45VersionFromRegistry)}");
#elif NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
            Console.WriteLine($@"CheckFor45PlusVersionString():                          {Safe(CheckFor45PlusVersionString)}");
#elif NETCOREAPP2_0
            // https://github.com/dotnet/corefx/blob/611e3feabcbcf7f79673b351d3e7e156a55f98c2/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.Unix.cs
            // https://github.com/dotnet/corefx/blob/611e3feabcbcf7f79673b351d3e7e156a55f98c2/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.Windows.cs
            // https://github.com/mono/mono/blob/90b49aa3aebb594e0409341f9dca63b74f9df52e/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs
            Console.WriteLine($@"RuntimeInformation.FrameworkDescription:                {Safe(() => RuntimeInformation.FrameworkDescription)}");
            // https://github.com/dotnet/corefx/blob/611e3feabcbcf7f79673b351d3e7e156a55f98c2/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.cs
            Console.WriteLine($@"RuntimeInformation.OSDescription:                       {Safe(() => RuntimeInformation.OSDescription)}");
            Console.WriteLine($@"RuntimeInformation.ProcessArchitecture:                 {Safe(() => RuntimeInformation.ProcessArchitecture.ToString())}");
            Console.WriteLine($@"GetIsWindowsSubsystemForLinux():                        {Safe(GetIsWindowsSubsystemForLinux)}");
#endif
        }

        private static string Safe(Func<string> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "errored";
            }
        }

#if NETCOREAPP2_0
        // https://github.com/dotnet/corefx/blob/f7bec37564c1b61450090336b9cc76b64480d915/src/CoreFx.Private.TestUtilities/src/System/PlatformDetection.cs
        private static string GetIsWindowsSubsystemForLinux()
        {
            // https://github.com/Microsoft/BashOnWindows/issues/423#issuecomment-221627364

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                const string versionFile = "/proc/version";
                if (File.Exists(versionFile))
                {
                    return File.ReadAllText(versionFile);
                    //string s = File.ReadAllText(versionFile);

                    //if (s.Contains("Microsoft") || s.Contains("WSL"))
                    //{
                    //    return true;
                    //}
                }
            }

            return null;
        }
#endif

        // https://github.com/dotnet/corefx/blob/c46e2e98b77d8c5eb2bc147df13b1505cf9c041e/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.cs
        private static string MsCorLibVer()
            => (typeof(object).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .FirstOrDefault() as AssemblyFileVersionAttribute)
                ?.Version;

        // https://github.com/dotnet/corefx/issues/9012#issuecomment-229421187
        private static string MonoVersion()
            => Type.GetType("Mono.Runtime", false)
                ?.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, null) as string;

        public static AssemblyName GetAssemblyNameFromEntryAssembly()
            // Returns null when entry is unmanaged code
            // Fails when running tests but worked on CLR, Mono and CoreCLR, Win, macOS and Linux
            => Assembly.GetEntryAssembly()?.GetName();

        public static AssemblyName GetAssemblyNameFromCurrentProcess()
        {
            // Doesn't work with 'dotnet app.dll or mono app.exe'
            var currentProc = Process.GetCurrentProcess();
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.Location == currentProc.MainModule.FileName)
                ?.GetName();
        }

        // Worked in all cases tested: CLR, mono, coreCLR
        internal static AssemblyName GetAssemblyNameFromStackTrace()
            => GetApplicationEntryMethod()?.Module?.Assembly?.GetName();

        internal static MethodBase GetApplicationEntryMethod()
        {
            var frames = new StackTrace().GetFrames();
            MethodBase entryMethod = null;

            for (var i = 0; i < frames.Length; i++)
            {
                var stackFrame = frames[i];
                var method = stackFrame.GetMethod() as MethodInfo;
                if (!method?.IsStatic == null)
                {
                    continue;
                }

                if (method.Name == "Main"
                    && (method.ReturnType == typeof(int)
                        || method.ReturnType == typeof(void)))
                {
                    entryMethod = method;
                    break;
                }

                if (method.Name == "InvokeMethod"
                    && method.DeclaringType == typeof(RuntimeMethodHandle))
                {
                    entryMethod = i == 0
                        ? method
                        : frames[i - 1].GetMethod();

                    break;
                }
            }

            return entryMethod;
        }

#if NET20 || NET35 || NET40
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#to-find-net-framework-versions-by-querying-the-registry-in-code-net-framework-1-4
        private static string GetPreNet45VersionFromRegistry()
        {
            // Opens the registry key for the .NET Framework entry.
            using (var ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5 
                // or later, you can use:
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "")
                        {
                            //no install info, must be later.
                            Console.WriteLine("No install key: " + versionKeyName + "  " + name);
                        }
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                Console.WriteLine(versionKeyName + "  " + name + "  SP" + sp);
                            }

                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            var subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                Console.WriteLine(versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name + "  SP" + sp);
                                }
                                else if (install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name);
                                }
                            }
                        }
                    }
                }

                return "TODO!";
            }
        }
#elif NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
// Registry API is Windows only. These set of registry keys are specific to .NET Framework 4.5 and forward
// https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#to-find-net-framework-versions-by-querying-the-registry-in-code-net-framework-45-and-later
// Returns the latest version installed! not necessarily the one running.
        private static string Get45PlusLatestReleaseNumberFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                int.TryParse(ndpKey?.GetValue("Release")?.ToString(), out var releaseId);
                return releaseId.ToString();
            }
        }

        private static string CheckFor45PlusVersionString()
        {
            var releaseKey = int.Parse(Get45PlusLatestReleaseNumberFromRegistry());
            switch (int.Parse(Get45PlusLatestReleaseNumberFromRegistry()))
            {
                // NOTE: 4.7.1 and forward support netstandard 2.0 and should use a different API!
                case int _ when releaseKey >= 461308: return "4.7.1 or higher";
                case int _ when releaseKey >= 460798: return "4.7";
                case int _ when releaseKey >= 394802: return "4.6.2";
                case int _ when releaseKey >= 394254: return "4.6.1";
                case int _ when releaseKey >= 393295: return "4.6";
                case int _ when releaseKey >= 379893: return "4.5.2";
                case int _ when releaseKey >= 378675: return "4.5.1";
                case int _ when releaseKey >= 378389: return "4.5";
                default: return null;
            }
        }
    
#endif

        private static string TargetFramework()
            =>
#if NET35
            "net35"
#elif NET40
            "net40"
#elif NET45
            "net45"
#elif NET451
            "net451"
#elif NET452
            "net452"
#elif NET46
            "net46"
#elif NET461
            "net461"
#elif NET462
            "net462"
#elif NET47
            "net47"
#elif NET471
            "net471"
#elif NET472
            "net472"
#elif NETCOREAPP2_0
            "netcoreapp2.0"
#elif NETCOREAPP1_1
            "netcoreapp1.1"
#elif NETCOREAPP1_0
            "netcoreapp1.0"
#else
            "Other"
#endif
            ;
    }
}
