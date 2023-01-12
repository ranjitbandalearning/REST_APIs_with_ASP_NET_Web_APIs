using CourseLibrary.API.DataStore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //RANJIT - configurating and running the app
            //  As ours is a web application, so this needs to be hosted.   So the below method is doing the same
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            // migrate the database.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetService<IAuthorData>();
                    // for demo purposes, delete the database & migrate on startup so 
                    // we can start with a clean slate
                    ////context.Database.EnsureDeleted();
                    ////context.Database.Migrate();
                    context.RestoreDataStore();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
