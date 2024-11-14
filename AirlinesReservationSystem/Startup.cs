using AirlinesReservationSystem.Controllers;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(AirlinesReservationSystem.Startup))]

namespace AirlinesReservationSystem
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            JobStorage.Current = new SqlServerStorage("Model1");
            HomeController homeController = new HomeController();
            RecurringJob.AddOrUpdate(() => homeController.RunWithSeconds(), Cron.Minutely);
            app.UseHangfireServer();
        }
    }
}
