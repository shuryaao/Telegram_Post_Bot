using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram_Memes_Post_Bot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddControllers().AddJsonOptions(json =>
            {
                json.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddOptions();
           
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseRouting();

            app.UseResponseCompression();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                var healthCheckPort = Configuration.GetValue("HEALTH_CHECK_PORT", 5015);

                endpoints.MapHealthChecks("/readiness").RequireHost($"*:{healthCheckPort}");
                endpoints.MapHealthChecks("/liveness").RequireHost($"*:{healthCheckPort}");

                endpoints.MapControllers();
            });
        }
    }
}