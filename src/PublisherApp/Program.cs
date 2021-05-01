using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PublisherApp
{
    class Program
    {
        public static IConfiguration _configuration;

        static async Task Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            Logger.LogInfo($"Initializing at: {DateTime.Now}");

            using IHost host = CreateHostBuilder(args).Build();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetService<Publisher>().Publish();

            Logger.LogInfo($"Finish at: {DateTime.Now}");
            watch.Stop();
            Logger.LogInfo($" === Time elapsed (ms): {watch.ElapsedMilliseconds} ===");
            Console.ReadLine();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);
        private static void ConfigureServices(IServiceCollection services)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddSingleton<IConfiguration>(_configuration);
            services.AddTransient<Publisher>();
            services.Configure<PathOptions>(_configuration.GetSection(PathOptions.Path));
        }
    }
}
