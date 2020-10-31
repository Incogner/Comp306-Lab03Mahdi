using System;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Lab03Mahdi.Areas.Identity.IdentityHostingStartup))]
namespace Lab03Mahdi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((context, services) => {

                //Get db user and password from amazon
                var builder = new SqlConnectionStringBuilder(context.Configuration.GetConnectionString("AWS-RDSConnection"));
                builder.UserID = context.Configuration["DbUser"];
                builder.Password = context.Configuration["DbPassword"];
                var connection = builder.ConnectionString;
                services.AddDbContext<Lab03MahdiContext>(options =>
                    options.UseSqlServer(connection));

                //services.AddDbContext<Lab03MahdiContext>(options =>
                //    options.UseSqlServer(context.Configuration.GetConnectionString("AWS-RDSConnection")));

                services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<Lab03MahdiContext>();
            });
        }
    }
}