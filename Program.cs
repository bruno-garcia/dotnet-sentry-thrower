using System;
#if !NET35
using System.Threading.Tasks;
#endif
using SharpRaven;
using SharpRaven.Data;

namespace CoreThrower
{
    class Program
    {
#if NET40 || NET35
        static void  Main()
#else
        static async Task Main()
#endif
        {
            var dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ?? "https://5fd7a6cda8444965bade9ccfd3df9882@sentry.io/1188141";
            
            var framework = TargetFramework();
            System.Console.WriteLine("Built against: " + framework);
            var client = new RavenClient(dsn);

            var ex = new Exception("I love throwing.");
            ex.Data.Add("AppBuiltTargetingFramework", framework);

#if NET40 || NET35
            var id = client.Capture(new SentryEvent(ex));
#else
            var id = await client.CaptureAsync(new SentryEvent(ex));
#endif
            Console.WriteLine("Event id: " + id);
        }

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
