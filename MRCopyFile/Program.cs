using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MR.Log;

namespace MRCopyFile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MRLog.ConfigureLogMain();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                MRLog.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .MRConfigureLogService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
