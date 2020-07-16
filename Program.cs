using AutoMapper;
using LifeLongApi.Data.DataInitializer;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi {
    public class Program {
        public static async Task Main (string[] args) {
            var host = CreateHostBuilder (args).Build ();
            using (var scope = host.Services.CreateScope ()) {
                var serviceProvider = scope.ServiceProvider;
                try {
                    var userService = serviceProvider.GetRequiredService<IUserService> ();
                    var roleService = serviceProvider.GetRequiredService<IRoleService> ();
                    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
                    var topicService = serviceProvider.GetRequiredService<ITopicService>();
                    var mapper = serviceProvider.GetRequiredService<IMapper>();
                    //UserAndRoleDataInitializer.SeedData (userManager, roleManager);
                    var dbs = new DbSeeder(userService, roleService, categoryService, topicService);
                    await dbs.SeedDataAsync();
                } catch (Exception ex) {
                    Debug.WriteLine (ex.Message);
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder (args)
            .ConfigureWebHostDefaults (webBuilder => {
                webBuilder.UseStartup<Startup> ();
                //webBuilder.UseUrls("https://localhost:4000/");
            });
    }
}