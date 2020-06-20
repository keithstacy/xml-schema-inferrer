#if !NO_SECURITY
//using IdentityServer4.AccessTokenValidation;
//using Microsoft.AspNetCore.Mvc.Authorization;
#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InspectionResultsDataContext = ImageDataAccess.InspectionResultsContext.InspectionResultsDataContext;

namespace FoundDefect
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
            services.AddSwaggerDocument();
#if !NO_SECURITY
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.Authority = "http://localhost:5000";
            //        options.RequireHttpsMetadata = false;
            //        options.Audience = "FoundDefect";
            //    });
            //services.AddMvcCore(options => options.Filters.Add(new AuthorizeFilter()));
#endif
            //services.AddDbContext<InspectionResultsContext>();
            services.AddDbContext<InspectionResultsDataContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DataContextLocalhost"])
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Dev environment running");
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
