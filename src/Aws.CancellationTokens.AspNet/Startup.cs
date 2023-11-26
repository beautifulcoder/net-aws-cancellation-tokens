using Aws.CancellationTokens.AspNet.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Aws.CancellationTokens.AspNet;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is null)
    {
      services.AddHttpContextAccessor();
      services.Configure<MvcOptions>(options => options.ConfigureGracefulTimeout());
    }
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}
