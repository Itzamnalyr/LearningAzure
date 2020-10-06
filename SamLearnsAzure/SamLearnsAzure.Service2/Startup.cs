using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SamLearnsAzure.Service.DataAccess;
//using SamLearnsAzure.Service.EFCore;
using StackExchange.Redis;

namespace SamLearnsAzure.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //string sqlConnectionStringName = "ConnectionStrings:SamsAppConnectionString" + Configuration["AppSettings:Environment"];
            //services.AddDbContext<SamsAppDBContext>(options =>
            //    options.UseSqlServer(Configuration[sqlConnectionStringName]));

            services.AddControllers()
                //This JSON setting stops the JSON from being truncated
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SamLearnsAzure API", Version = "v1" });

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IRedisService, RedisService>();
            string redisConnectionStringName = "AppSettings:RedisCacheConnectionString" + Configuration["AppSettings:Environment"];
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(Configuration[redisConnectionStringName]);
            if (connectionMultiplexer != null)
            {
                IDatabase database = connectionMultiplexer.GetDatabase();
                services.AddSingleton<IDatabase>(_ => database);
            }

            services.AddScoped<IColorsRepository, ColorsRepository>();
            services.AddScoped<IInventoriesRepository, InventoriesRepository>();
            services.AddScoped<IInventoryPartsRepository, InventoryPartsRepository>();
            services.AddScoped<IInventorySetsRepository, InventorySetsRepository>();
            services.AddScoped<IOwnersRepository, OwnersRepository>();
            services.AddScoped<IOwnerSetsRepository, OwnerSetsRepository>();
            services.AddScoped<IPartCategoriesRepository, PartCategoriesRepository>();
            services.AddScoped<IPartRelationshipsRepository, PartRelationshipsRepository>();
            services.AddScoped<IPartsRepository, PartsRepository>();
            services.AddScoped<ISetPartsRepository, SetPartsRepository>();
            services.AddScoped<ISetsRepository, SetsRepository>();
            services.AddScoped<ISetImagesRepository, SetImagesRepository>();
            services.AddScoped<IThemesRepository, ThemesRepository>();
            services.AddScoped<IPartImagesRepository, PartImagesRepository>();

            //Application insights initialization
            services.AddApplicationInsightsTelemetry(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SamLearnsAzure API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
