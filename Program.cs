using System;
using System.Collections.Generic;
using System.Linq;
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
        static void Main(string[] args)
#else
        static async Task Main(string[] args)

#endif
        {
            var dsn = Environment.GetEnvironmentVariable("SENTRY_DSN")
                ?? "https://bd6ec3bf83bf4646952723c20ab16cd3@sentry.garcia.in/1";

            System.Console.WriteLine("Using DSN: " + dsn);

            var framework = TargetFramework();
            System.Console.WriteLine("Built against: " + framework);
            var client = new RavenClient(dsn);
            Exception ex = null;

#if NET40 || NET35
            ex = new Exception(args.FirstOrDefault() ?? "I love throwing.");
#else
            try
            {
                var neverSet = await Thrower(new List<Program>());
            }
            catch (Exception e)
            {
                ex = e;
            }
#endif
            ex.Data.Add("AppBuiltTargetingFramework", framework);

#if NET40 || NET35
            var id = client.Capture(new SentryEvent(ex));
#else
            var id = await client.CaptureAsync(new SentryEvent(ex));
#endif
            Console.WriteLine("Event id: " + id);
        }

#if !NET40 && !NET35
        private static async Task<int> Thrower(IEnumerable<Program> programs)
        {
#if !NET45
            await Task.Yield();
#else
            await Task.Delay(0);
#endif

            return LocalFunction();

            int LocalFunction() => throw null;
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
