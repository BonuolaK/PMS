using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PMS.Shared.DataAccess;
using PMS.Shared.DataAccess.EF;
using Proj.Core;
using Proj.Core.Database;
using Proj.Core.Models;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.HttpService;
using PMS.Shared.NetCore;

namespace Proj.Api
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMS_Project_Service", Version = "v1" });
            });

            services.AddDbContext<ProjectDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            QueueSettings options = Configuration.GetSection("AppSettings").GetSection("QueueSettings").Get<QueueSettings>();

            services.AddSingleton(options);

            services.AddTransient<BaseContext, ProjectDbContext>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IRepository<Project>, EFRepository<Project>>();
            services.AddTransient<IRepository<SubProject>, EFRepository<SubProject>>();
            services.AddTransient<IProjectService, ProjectService>();
            services.RegisterQueueServices(Configuration);
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundServiceWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Values Api V1");
            });


        }
    }
}
