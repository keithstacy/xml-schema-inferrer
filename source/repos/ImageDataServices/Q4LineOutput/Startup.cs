using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InspectionResultsDataContext = ImageDataAccess.InspectionResultsContext.InspectionResultsDataContext;

#if !NO_SECURITY
//using Microsoft.AspNetCore.Mvc.Authorization;
//using IdentityServer4.AccessTokenValidation;
#endif

namespace Q4LineOutput
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
            services.AddControllers();
            // Register the Swagger services
            services.AddSwaggerDocument();
#if !NO_SECURITY
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.Authority = "http://localhost:5000";
            //        options.RequireHttpsMetadata = false;
            //        options.Audience = "Q4LineOutput";
            //    });
            //services.AddMvcCore(options =>
            //    {
            //        options.Filters.Add(new AuthorizeFilter());
            //    })
            //    .AddAuthorization();
#endif
            services.AddDbContext<InspectionResultsDataContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("DataContextLocalhost")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
