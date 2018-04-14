using System;
using System.Threading.Tasks;
using SharpRaven;
using SharpRaven.Data;

namespace CoreThrower
{
    class Program
    {
        static async Task Main()
        {
            var dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ?? "https://5fd7a6cda8444965bade9ccfd3df9882@sentry.io/1188141";

            var client = new RavenClient(dsn);

            var id = await client.CaptureAsync(new SentryEvent(new InvalidOperationException()));
            Console.WriteLine("Event id: " + id);
        }
    }
}
