using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
#if NETCOREAPP2_0 || NET471
using Plugin.DeviceInfo;
#else
using Microsoft.Win32;
#endif

namespace CoreThrower
{
    class Program
    {
        static void Main(string[] args)
        {
            // 
            var framework = TargetFramework();
            Console.WriteLine($@"Built against:                                          {framework}");
            Console.WriteLine($@"IsOS(OSType.AnyServer):                                 {IsOS(OSType.AnyServer)}"); // False on Win10Pro
            Console.WriteLine($@"IsOS(OSType.Professional):                              {IsOS(OSType.Professional)}"); // True on Win10Pro
            Console.WriteLine($@"IsOS(OSType.Windows):                                   {IsOS(OSType.Windows)}"); // returns false on Windows 10 Pro
            Console.WriteLine($@"Environment.Version:                                    {Safe(() => Environment.Version.ToString())}");
            Console.WriteLine($@"Dns.GetHostName():                                      {Safe(() => Dns.GetHostName().ToString())}");
            Console.WriteLine($@"Environment.MachineName:                                {Safe(() => Environment.MachineName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.DisplayName:                         {Safe(() => TimeZoneInfo.Local.DisplayName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.Id:                                  {Safe(() => TimeZoneInfo.Local.Id.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.StandardName:                        {Safe(() => TimeZoneInfo.Local.StandardName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.DaylightName:                        {Safe(() => TimeZoneInfo.Local.DaylightName.ToString())}");
            Console.WriteLine($@"TimeZoneInfo.Local.BaseUtcOffset:                       {Safe(() => TimeZoneInfo.Local.BaseUtcOffset.ToString())}");
            Console.WriteLine($@"DateTimeOffset.Now.Offset:                              {Safe(() => DateTimeOffset.Now.Offset.ToString())}");
            Console.WriteLine($@"HOSTTYPE:                                               {Environment.GetEnvironmentVariable("HOSTTYPE")}");
            Console.WriteLine($@"PROCESSOR_ARCHITECTURE:                                 {Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine)}");
            Console.WriteLine($@"Environment.OSVersion:                                  {Safe(() => Environment.OSVersion.ToString())}");
            Console.WriteLine($@"Environment.OSVersion.Platform:                         {Safe(() => Environment.OSVersion.Platform.ToString())}");
            Console.WriteLine($@"Environment.OSVersion.VersionString:                    {Safe(() => Environment.OSVersion.VersionString.ToString())}");
            Console.WriteLine($@"Environment.OSVersion.Version.Build:                    {Safe(() => Environment.OSVersion.Version.Build.ToString())}");
            Console.WriteLine($@"Environment.OSVersion.Version:                          {Safe(() => Environment.OSVersion.Version.ToString())}");
            Console.WriteLine($@"MonoVersion():                                          {Safe(MonoVersion)}");
            Console.WriteLine($@"MsCorLibVer():                                          {Safe(MsCorLibVer)}");
            Console.WriteLine($@"GetComputerStartupTime():                               {Safe(GetComputerStartupTime)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryMethod().Name:                  {Safe(() => GetAssemblyNameFromStackTrace()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryMethod().Version:               {Safe(() => GetAssemblyNameFromStackTrace()?.Version.ToString())}");
            Console.WriteLine($@"GetAssemblyNameFromEntryAssembly().Name:                {Safe(() => GetAssemblyNameFromEntryAssembly()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromEntryAssembly().Version:             {Safe(() => GetAssemblyNameFromEntryAssembly()?.Version.ToString())}");
            Console.WriteLine($@"GetAssemblyNameFromCurrentProcess().Name:               {Safe(() => GetAssemblyNameFromCurrentProcess()?.Name)}");
            Console.WriteLine($@"GetAssemblyNameFromCurrentProcess().Version:            {Safe(() => GetAssemblyNameFromCurrentProcess()?.Version.ToString())}");
            Console.WriteLine($@"Process.GetCurrentProcess().StartTime:                  {Safe(Process.GetCurrentProcess().StartTime.ToString)}");
#if NET20 || NET35 || NET40
            Console.WriteLine($@"GetVersionFromRegistry()():                             {Safe(GetPreNet45VersionFromRegistry)}");
#elif NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47
            Console.WriteLine($@"CheckFor45PlusVersionString():                          {Safe(CheckFor45PlusVersionString)}");
#elif NETCOREAPP2_0 || NET471
            
            Console.WriteLine($@"CrossDeviceInfo.IsSupported:                            {Safe(() => CrossDeviceInfo.IsSupported.ToString())}");
            if (CrossDeviceInfo.IsSupported)
            {
                Console.WriteLine($@"CrossDeviceInfo.Current.DeviceName:                     {Safe(() => CrossDeviceInfo.Current.DeviceName)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.AppBuild:                       {Safe(() => CrossDeviceInfo.Current.AppBuild)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.AppVersion:                     {Safe(() => CrossDeviceInfo.Current.AppVersion)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Id:                             {Safe(() => CrossDeviceInfo.Current.Id)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Idiom:                          {Safe(() => CrossDeviceInfo.Current.Idiom.ToString())}");
                Console.WriteLine($@"CrossDeviceInfo.Current.IsDevice:                       {Safe(() => CrossDeviceInfo.Current.IsDevice.ToString())}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Manufacturer:                   {Safe(() => CrossDeviceInfo.Current.Manufacturer)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Model:                          {Safe(() => CrossDeviceInfo.Current.Model)}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Platform:                       {Safe(() => CrossDeviceInfo.Current.Platform.ToString())}");
                Console.WriteLine($@"CrossDeviceInfo.Current.VersionNumber:                  {Safe(() => CrossDeviceInfo.Current.VersionNumber.ToString())}");
                Console.WriteLine($@"CrossDeviceInfo.Current.Version:                        {Safe(() => CrossDeviceInfo.Current.Version)}");
            }

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

        public static string GetComputerStartupTime()
        {
            return new DateTimeOffset(
                DateTime.UtcNow - TimeSpan.FromTicks(Stopwatch.GetTimestamp()),
                TimeSpan.Zero).ToString();
        }
#if NETCOREAPP2_0 || NET471
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

        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsOS(OSType dwOS);

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
#elif NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47
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

        // https://www.pinvoke.net/default.aspx/shlwapi/IsOS.html
        /// <summary>
        /// Options for IsOS() function
        /// </summary>
        public enum OSType : uint
        {
            /// <summary>
            /// The program is running on one of the following versions of Windows:
            ///    * Windows 95
            ///    * Windows 98
            ///    * Windows Me
            /// Equivalent to PlatformID.Win32Windows. Note that none of those systems
            /// are supported at this time. IsOS(Windows) returns false on all supported
            /// systems.
            /// </summary>
            Windows = 0,

            /// <summary>
            /// Always returns true
            /// </summary>
            NT = 1,

            /// <summary>
            /// Always returns false
            /// </summary>
            Win95OrGreater = 2,

            /// <summary>
            /// Always returns false
            /// </summary>
            NT4OrGreater = 3,

            /// <summary>
            /// Always returns false
            /// </summary>
            Win98OrGreater = 5,

            /// <summary>
            /// Always returns false
            /// </summary>
            Win98Gold = 6,

            /// <summary>
            /// The program is running on Windows 2000 or one of its successors.
            /// </summary>
            Win2000OrGreater = 7,

            /// <summary>
            /// Do not use; use Professional.
            /// </summary>
            Win2000Pro = 8,

            /// <summary>
            /// Do not use; use Server.
            /// </summary>
            Win2000Server = 9,

            /// <summary>
            /// Do not use; use AdvancedServer.
            /// </summary>
            Win2000AdvancedServer = 10,

            /// <summary>
            /// Do not use; use DataCenter.
            /// </summary>
            Win2000DataCenter = 11,

            /// <summary>
            /// The program is running on Windows 2000 Terminal Server in either Remote
            /// Administration mode or Application Server mode, or Windows Server 2003 (or
            /// one of its successors) in Terminal Server mode or Remote Desktop for
            /// Administration mode. Consider using a more specific value such as
            /// TerminalServer, TerminalRemoteAdmin, or PersonalTerminalServer.
            /// </summary>
            Win2000Terminal = 12,

            /// <summary>
            /// The program is running on Windows Embedded, any version. Equivalent to
            /// SuiteMask.EmbeddedNT.
            /// </summary>
            Embedded = 13,

            /// <summary>
            /// The program is running as a Terminal Server client. Equivalent to
            /// GetSystemMetrics(SM_REMOTESESSION).
            /// </summary>
            TerminalClient = 14,

            /// <summary>
            /// The program is running on Windows 2000 Terminal Server in the Remote
            /// Administration mode or Windows Server 2003 (or one of its successors) in
            /// the Remote Desktop for Administration mode (these are the default
            /// installation modes). This is equivalent to SuiteMask.Terminal &&
            /// SuiteMask.SingleUserTerminalServer.
            /// </summary>
            TerminalRemoteAdmin = 15,

            /// <summary>
            /// Always returns false.
            /// </summary>
            Win95Gold = 16,

            /// <summary>
            /// Always returns false.
            /// </summary>
            MEOrGreater = 17,

            /// <summary>
            /// Always returns false.
            /// </summary>
            XPOrGreater = 18,

            /// <summary>
            /// Always returns false.
            /// </summary>
            Home = 19,

            /// <summary>
            /// The program is running on Windows NT Workstation or Windows 2000 (or one of
            /// its successors) Professional. Equivalent to PlatformID.Win32NT &&
            /// ProductType.NTWorkstation.
            /// </summary>
            Professional = 20,

            /// <summary>
            /// The program is running on Windows Datacenter Server or Windows Server
            /// Datacenter Edition, any version. Equivalent to (ProductType.NTServer ||
            /// ProductType.NTDomainController) && SuiteMask.DataCenter.
            /// </summary>
            DataCenter = 21,

            /// <summary>
            /// The program is running on Windows Advanced Server or Windows Server
            /// Enterprise Edition, any version. Equivalent to (ProductType.NTServer ||
            /// ProductType.NTDomainController) && SuiteMask.Enterprise &&
            /// !SuiteMask.DataCenter.
            /// </summary>
            AdvancedServer = 22,

            /// <summary>
            /// The program is running on Windows Server (Standard) or Windows Server
            /// Standard Edition, any version. This value will not return true for
            /// SuiteMask.DataCenter, SuiteMask.Enterprise, SuiteMask.SmallBusiness, or
            /// SuiteMask.SmallBusinessRestricted.
            /// </summary>
            Server = 23,

            /// <summary>
            /// The program is running on Windows 2000 Terminal Server in Application
            /// Server mode, or on Windows Server 2003 (or one of its successors) in
            /// Terminal Server mode. This is equivalent to SuiteMask.Terminal &&
            /// SuiteMask.SingleUserTerminalServer.
            /// </summary>
            TerminalServer = 24,

            /// <summary>
            /// The program is running on Windows XP (or one of its successors), Home
            /// Edition or Professional. This is equivalent to
            /// SuiteMask.SingleUserTerminalServer && !SuiteMask.Terminal.
            /// </summary>
            PersonalTerminalServer = 25,

            /// <summary>
            /// Fast user switching is enabled.
            /// </summary>
            FastUserSwitching = 26,

            /// <summary>
            /// Always returns false.
            /// </summary>
            WelcomeLogonUI = 27,

            /// <summary>
            /// The computer is joined to a domain.
            /// </summary>
            DomainMember = 28,

            /// <summary>
            /// The program is running on any Windows Server product. Equivalent to
            /// ProductType.NTServer || ProductType.NTDomainController.
            /// </summary>
            AnyServer = 29,

            /// <summary>
            /// The program is a 32-bit program running on 64-bit Windows.
            /// </summary>
            WOW6432 = 30,

            /// <summary>
            /// Always returns false.
            /// </summary>
            WebServer = 31,

            /// <summary>
            /// The program is running on Microsoft Small Business Server with restrictive
            /// client license in force. Equivalent to SuiteMask.SmallBusinessRestricted.
            /// </summary>
            SmallBusinessServer = 32,

            /// <summary>
            /// The program is running on Windows XP Tablet PC Edition, or one of its
            /// successors.
            /// </summary>
            TabletPC = 33,

            /// <summary>
            /// The user should be presented with administrator UI. It is possible to have
            /// server administrative UI on a non-server machine. This value informs the
            /// application that an administrator's profile has roamed to a non-server, and
            /// UI should be appropriate to an administrator. Otherwise, the user is shown
            /// a mix of administrator and nonadministrator settings.
            /// </summary>
            ServerAdminUI = 34,

            /// <summary>
            /// The program is running on Windows XP Media Center Edition, or one of its
            /// successors. Equivalent to GetSystemMetrics(SM_MEDIACENTER).
            /// </summary>
            MediaCenter = 35,

            /// <summary>
            /// The program is running on Windows Appliance Server.
            /// </summary>
            Appliance = 36,
        }
    }
}
