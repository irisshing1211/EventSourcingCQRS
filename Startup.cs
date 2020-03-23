using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Models;
using EventSourcingCQRS.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace EventSourcingCQRS
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { Configuration = configuration; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbSettings>(Configuration.GetSection(nameof(DbSettings)));
            services.AddSingleton<IDbSettings>(sp => sp.GetRequiredService<IOptions<DbSettings>>().Value);

            services.AddDbContext<EventSourceContext>(options =>
                                                          options.UseSqlServer(
                                                              Configuration["DbSettings:RelationalConnString"]));

            services.AddSingleton<QueryContext>();
            services.AddScoped<IQueryService<CountItem>, ItemQueryService>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            else { app.UseExceptionHandler("/Home/Error"); }

           // applicationLifetime.ApplicationStopping.Register(OnShutdown);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
                                             pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            DbSettings setting = new DbSettings();
            Configuration.GetSection("DbSettings").Bind(setting);
            SeedData.Initialize(app.ApplicationServices, setting);
        }

        private void OnShutdown()
        {
            // use _logger here;
        }
    }
}
