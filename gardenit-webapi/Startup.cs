using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

            string connectionString = Configuration.GetConnectionString("DefaultDB") ?? 
                Environment.GetEnvironmentVariable("DefaultDB");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
            // MQTT
            // TODO: May replace with env var option
            var mqttOptions = new MqttOptions() {
                Host = Configuration["MqttOptions:Host"] ?? Environment.GetEnvironmentVariable("MqttHost"), 
                User = Configuration["MqttOptions:User"] ?? Environment.GetEnvironmentVariable("MqttUser"),
                Password = Configuration["MqttOptions:Password"] ?? Environment.GetEnvironmentVariable("MqttPassword"),
                Port = Convert.ToInt32(Configuration["MqttOptions:Port"] ?? Environment.GetEnvironmentVariable("MqttPort"))
            };
            services.AddSingleton<IOptions<MqttOptions>>(x => Options.Create(mqttOptions));
            // This needs to be a singleton in order to hold onto the MQTT connection...
            services.AddSingleton<IMqttLib, MqttLib>();   

            // Async init
            services.AddScoped<AsyncInitializer>();
            services.AddHostedService<AsyncHostedService>();

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

    public class AsyncHostedService: IHostedService
    {
        // We need to inject the IServiceProvider so we can create 
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public AsyncHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using(var scope = _serviceProvider.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<AsyncInitializer>();
                await initializer.Initialize();
            }
        }

        // noop
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
