using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Hubs;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileDropBE {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
#if (DEBUG)
      var connectionString = Configuration.GetConnectionString("DebugConnection");
#else
      var connectionString = Configuration.GetConnectionString("ServerConnection");
#endif

      services.AddDbContextPool<DB_Context>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

      services.AddSingleton<IConfiguration>(Configuration);
      services.AddScoped<CurrentUserHelper>();
      services.AddScoped<FileLogic>();
      services.AddScoped<UserLogic>();
      services.AddScoped<BindingModelFactory>();
      
      services.AddSignalR();

      services.AddCors(options => {
        options.AddDefaultPolicy(builder => builder
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
      });

      services.Configure<FormOptions>(x => {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue;
      });

      services.AddControllers();
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileDropBE", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileDropBE v1"));
      }

      app.UseCors();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
        endpoints.MapHub<UploadHub>("/uploadhub");
      });

      using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
        var context = serviceScope.ServiceProvider.GetRequiredService<DB_Context>();
        context.Database.Migrate();
      }
    }
  }
}
