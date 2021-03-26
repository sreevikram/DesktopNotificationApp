using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System.IO;
using ElectronNetWithMVC.Settings;
using MQTTnet;
using MQTTnet.Client.Options;
using System.Threading;
using MQTTnet.Client;
using System.Text;
using MQTTnet.Extensions.ManagedClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Coravel;
using ElectronNetWithMVC.Invocables;

namespace ElectronNetWithMVC
{
    public class Startup
    {
         private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
             _env = env;
        }

        public IConfiguration Configuration { get; }

        
        #region  MQTT
        private void MapConfiguration()
        {
            MapBrokerHostSettings();
            MapClientSettings();
        }

        private void MapBrokerHostSettings()
        {
            BrokerHostSettings brokerHostSettings = new BrokerHostSettings();
            Configuration.GetSection(nameof(BrokerHostSettings)).Bind(brokerHostSettings);
            AppSettingsProvider.BrokerHostSettings = brokerHostSettings;
        }

        private void MapClientSettings()
        {
            ClientSettings clientSettings = new ClientSettings();
            Configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);
            AppSettingsProvider.ClientSettings = clientSettings;
        }
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            MapConfiguration();
             services.AddScheduler();  
              services.AddScoped<ReminderJob>();          
             services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            if (HybridSupport.IsElectronActive)
            {
               ElectronNetWithMVC.ElectronBootstrap.Start(_env);
            }
          
            Services.MQTTService.Start();

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<ReminderJob>( )
                .EveryMinute()
                .Weekday();
            });
        }


        
        public void Notify(string payload)
        {
                var options = new NotificationOptions("Notification with image", "Short message plus a custom image")
                    {
                        OnClick = async () => await Electron.Dialog.ShowMessageBoxAsync("Notification clicked"),
                        Icon = Path.Combine(_env.ContentRootPath, "wwwroot/img/Indian_Female_Welcome01.png"),
                        Sound=Path.Combine(_env.ContentRootPath, "wwwroot/sound/boss_you_have_a_msg.mp3"),
                        Body=payload,
                        Actions = new NotificationAction(){Text="YEs",Type="button"},
                    };

                    Electron.Notification.Show(options);

        }

       

       
    }
}
