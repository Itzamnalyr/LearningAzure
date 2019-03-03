using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
            services.AddDbContext<SamsAppDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SamsAppConnectionString")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IColorsRepository, ColorsRepository>();
            services.AddScoped<IInventoriesRepository, InventoriesRepository>();
            services.AddScoped<IInventoryPartsRepository, InventoryPartsRepository>();
            services.AddScoped<IInventorySetsRepository, InventorySetsRepository>();
            services.AddScoped<IOwnersRepository, OwnersRepository>();
            services.AddScoped<IOwnerSetsRepository, OwnerSetsRepository>();
            services.AddScoped<IPartCategoriesRepository, PartCategoriesRepository>();
            services.AddScoped<IPartRelationshipsRepository, PartRelationshipsRepository>();
            services.AddScoped<IPartsRepository, PartsRepository>();
            services.AddScoped<ISetsRepository, SetsRepository>();
            services.AddScoped<IThemesRepository, ThemesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
