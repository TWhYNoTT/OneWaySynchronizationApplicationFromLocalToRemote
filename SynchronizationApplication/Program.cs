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
        static void Main(string[] args)
        {
            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                // Set default font for better appearance
                Application.SetDefaultFont(new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point));

                // Handle any unhandled exceptions
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                // Setup dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services);

                // Create service provider
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    // Handle command line arguments
                    bool startMinimized = Array.Exists(args, arg => arg.Equals("/minimized", StringComparison.OrdinalIgnoreCase));

                    // Create main form with injected dependencies
                    var mainForm = serviceProvider.GetRequiredService<MainForm>();

                    // Apply startup settings if applicable
                    if (startMinimized)
                    {
                        mainForm.StartMinimized = true;
                    }

                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical application error: {ex.Message}\n\n{ex.StackTrace}",
                                "Fatal Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<ICustomerDataService, CustomerDataService>();
            services.AddSingleton<IUserDataService, UserDataService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<ISynchronizationService, SynchronizationService>();
            services.AddSingleton<IStartupService, StartupService>();

            // Register the new services for auto sync functionality
            services.AddSingleton<ITriggerService, TriggerService>();
            services.AddSingleton<IChangeTrackingService, ChangeTrackingService>();

            // Register forms
            services.AddTransient<MainForm>();
        }

        // Handle UI thread exceptions
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nDo you want to continue running the application?",
                "Application Error",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result == DialogResult.No)
            {
                Application.Exit();
            }
        }

        // Handle non-UI thread exceptions
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"A fatal error occurred:\n\n{(e.ExceptionObject as Exception)?.Message}\n\nThe application will now terminate.",
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            Environment.Exit(1);
        }
    }
}