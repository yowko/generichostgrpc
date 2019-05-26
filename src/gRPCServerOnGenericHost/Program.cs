using System.Threading.Tasks;
using gRPC.Message;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace gRPCServerOnGenericHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var host = "127.0.0.1";
                    var port = 50051;

                    services.AddSingleton<gRPCService.gRPCServiceBase, gRPCServiceImpl>();
                    var Services = services.BuildServiceProvider();
                    services.AddSingleton(
                        new gRPCServer(host, port,
                            gRPCService.BindService(Services.GetRequiredService<gRPCService.gRPCServiceBase>())
                        ));
                });

            await builder.RunConsoleAsync();
        }
    }
}