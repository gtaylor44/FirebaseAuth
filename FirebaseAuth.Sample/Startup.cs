using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirebaseAuth.Sample
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
            // required for Firebase Auth
            void options(FirebaseAuthenticationOptions o)
            {
                o.FirebaseProjectId = "journeytracker44";
                o.ExceptionLogger = (Exception ex) =>
                {
                    // set up exception logging here.
                };
            }

            // Required for Firebase Auth. Place above AddMvc()
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "Default";
            }).AddScheme<FirebaseAuthenticationOptions, FirebaseAuthenticationHandler>("Default", options);

            services.AddMemoryCache();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // required for Firebase Auth
            app.UseAuthentication();
            

            app.UseMvc();
        }
    }
}
