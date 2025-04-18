using Microsoft.OpenApi.Models;
using Spotify.Identity.Midlleware;

namespace Spotify.Identity;

public class StartUp(IConfiguration configuration,IWebHostEnvironment hostEnvironment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<ValidationExeptionHandler>();
        services.AddSwaggerGen(x =>
            x.SwaggerDoc("v1", new OpenApiInfo { Title = "Spotify.Identity", Version = "v1" }));
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddLogger(configuration)
            .AddAuth(configuration)
            .AddMediator()
            .AddMapper();
    }

    public void Configure(IApplicationBuilder app)
    {
        
       // if (hostEnvironment.IsDevelopment())
       // {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Spotify.Identity v1");
                x.RoutePrefix = string.Empty;
            });
       // }
       // else
      //  {
            app.UseExceptionHandler("/Error");
       // }
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
       
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
        });

    }
}