using Microsoft.AspNetCore;
using Serilog;
using Spotify.Identity;

public class Program
{
    public static void Main(string[] args)
    {
       CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webhost =>
            {
                webhost.UseStartup<StartUp>();
            });
        return builder;
    }
}





