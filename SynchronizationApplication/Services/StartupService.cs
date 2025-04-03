using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication.Services
{
    public class StartupService : IStartupService
    {
        private const string StartupRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "CustomerUserSync";

        public bool IsApplicationInStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey))
            {
                if (key == null) return false;
                string value = key.GetValue(ApplicationName) as string;

                if (string.IsNullOrEmpty(value)) return false;

                return value.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void AddApplicationToStartup()
        {
            if (!IsApplicationInStartup())
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
                {
                    if (key != null)
                    {
                        key.SetValue(ApplicationName, Application.ExecutablePath);
                    }
                    else
                    {
                        throw new Exception("Could not access startup registry key.");
                    }
                }
            }
        }

        public void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
            {
                if (key != null)
                {
                    key.DeleteValue(ApplicationName, false);
                }
                else
                {
                    throw new Exception("Could not access startup registry key.");
                }
            }
        }
    }
}