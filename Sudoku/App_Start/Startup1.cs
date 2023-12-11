using Hangfire;
using Microsoft.Owin;
using Owin;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Sudoku.App_Start.Startup1))]

namespace Sudoku.App_Start
{
    public class Startup1
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("Server=MSI; Database=HangFire;TrustServerCertificate=True; Integrated Security=True;");

            yield return new BackgroundJobServer();
        }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
            app.MapSignalR();

            RecurringJob.AddOrUpdate<HangFireService>("setRankUser", x => x.RankPlayer(), "0 */5 * ? * *");
            RecurringJob.AddOrUpdate<HangFireService>("reStoreAuthorPost", x => x.RestoreAuthorPostQuantity(), "0 */5 * ? * *");
        }
    }
}
