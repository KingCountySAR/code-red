using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twilio;

namespace Kcsar.Paging.Web
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews().AddRazorRuntimeCompilation();
      services.AddAuthentication(options =>
      {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "google";
      })
      .AddCookie("Cookies")
      .AddGoogle("google", options =>
      {
        options.ClientId = Configuration["auth:clientId"];
        options.ClientSecret = Configuration["auth:clientSecret"];
        options.AuthorizationEndpoint += $"?hd=${Configuration["auth:domain"]}";
      });

      services.AddSingleton<CodeRedService>();

      var twilioSid = Configuration["twilio:sid"];
      var twilioToken = Configuration["twilio:token"];

      if (!(string.IsNullOrWhiteSpace(twilioSid) || string.IsNullOrWhiteSpace(twilioToken)))
      {
        TwilioClient.Init(twilioSid, twilioToken);
      }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
