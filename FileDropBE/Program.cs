using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FileDropBE {
  public class Program {
    public static void Main(string[] args) {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
              webBuilder.UseStartup<Startup>();

              #if (!DEBUG)
                webBuilder.UseKestrel(options => {
                  options.Limits.MaxRequestBodySize = long.MaxValue;
                });
              #endif
            });
  }
}
