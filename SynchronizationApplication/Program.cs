using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using SynchronizationApplication.Services;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Set default font for better appearance
            Application.SetDefaultFont(new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point));

            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Create service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Create main form with injected dependencies
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<ICustomerDataService, CustomerDataService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<ISynchronizationService, SynchronizationService>();

            // Register forms
            services.AddTransient<MainForm>();
        }
    }
}