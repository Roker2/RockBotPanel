using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockBotPanel.Data;
using RockBotPanel.Models;
using RockBotPanel.Services;

namespace RockBotPanel
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
            services.AddDbContext<d940mhn2jd7mllContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("BotDbContext")));

            services.AddDbContext<PanelDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityPanelDbContext")));

            services.AddIdentity<TelegramUser, IdentityRole>()
                .AddEntityFrameworkStores<PanelDbContext>();

            services.AddSingleton<IEmailConfiguration>(new EmailConfiguration
            {
                SmtpServer = Configuration["EmailConfiguration:SmtpServer"],
                SmtpPort = int.Parse(Configuration["EmailConfiguration:SmtpPort"]),
                SmtpUsername = Configuration["EmailConfiguration:SmtpUsername"],
                SmtpPassword = Configuration["EmailConfiguration:SmtpPassword"]
            });

            services.AddTransient<IEmailMessanger, EmailMessanger>();

            services.AddSignalR();

            services.AddSingleton<ITelegramToken>(new TelegramToken(Configuration.GetConnectionString("TelegramBotToken")));

            services.AddScoped<ITelegramService, TelegramService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                NLog.LogManager.Configuration.Variables["globalLevel"] = "Debug";
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                NLog.LogManager.Configuration.Variables["globalLevel"] = "Info";
            }
            NLog.LogManager.ReconfigExistingLoggers();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<Hubs.ChatHub>("/chathub");
            });
        }
    }
}
