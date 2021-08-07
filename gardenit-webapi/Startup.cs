using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

using gardenit_webapi.Lib;
using gardenit_webapi.Storage;
using gardenit_webapi.Storage.EF;
using gardenit_webapi.Mqtt;
using gardenit_webapi.Controllers;

namespace gardenit_webapi
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "gardenit_webapi", Version = "v1" });
            });

            services.AddScoped<IPlantLib, PlantLib>();
            services.AddScoped<IWateringLib, WateringLib>();
            services.AddScoped<IMoistureLib, MoistureLib>();

            services.AddScoped<IStorePlants, EfPlantStorage>();
            services.AddMemoryCache();

            string connectionString = Configuration.GetConnectionString("DefaultDB");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
            // MQTT
            // TODO: May replace with env var option
            var mqttOptions = new MqttOptions() {
                Host = Configuration["MqttOptions:Host"],
                User = Configuration["MqttOptions:User"],
                Password = Configuration["MqttOptions:Password"],
                Port = Convert.ToInt32(Configuration["MqttOptions:Port"]),
            };
            services.AddSingleton<IOptions<MqttOptions>>(x => Options.Create(mqttOptions));
            // This needs to be a singleton in order to hold onto the MQTT connection...
            services.AddSingleton<IMqttLib, MqttLib>();   

            // Encryption Filter
            var encryptionKey = Configuration["EncryptionOptions:EncryptionKey"] ?? 
                Environment.GetEnvironmentVariable("EncryptionKey");

            var encryptionFilterOptions = new EncryptionFilterOptions() {
                EncryptionKey = encryptionKey
            };
            services.AddSingleton<IOptions<EncryptionFilterOptions>>(x => Options.Create(encryptionFilterOptions));
            services.AddScoped<EncryptionFilterAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            // Apply migrations
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "gardenit_webapi v1"));
            }

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
