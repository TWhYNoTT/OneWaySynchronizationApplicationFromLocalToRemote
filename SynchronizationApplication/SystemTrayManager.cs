using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SynchronizationApplication
{
    public class SystemTrayManager
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Form _ownerForm;
        private readonly ContextMenuStrip _contextMenu;

        public SystemTrayManager(Form ownerForm, Icon appIcon, string tooltip)
        {
            _ownerForm = ownerForm ?? throw new ArgumentNullException(nameof(ownerForm));

            // Initialize context menu
            _contextMenu = new ContextMenuStrip();

            // Add menu items
            var openMenuItem = new ToolStripMenuItem("Open");
            openMenuItem.Click += (s, e) => ShowApplication();
            _contextMenu.Items.Add(openMenuItem);

            var syncNowMenuItem = new ToolStripMenuItem("Sync Now");
            syncNowMenuItem.Click += new EventHandler(SyncNow_Click);
            _contextMenu.Items.Add(syncNowMenuItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += (s, e) => Application.Exit();
            _contextMenu.Items.Add(exitMenuItem);

            // Initialize NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Icon = appIcon,
                Text = tooltip,
                ContextMenuStrip = _contextMenu,
                Visible = false
            };

            // Double-click on tray icon shows the application
            _notifyIcon.DoubleClick += (s, e) => ShowApplication();
        }

        // Event handler for 'Sync Now' menu item
        private void SyncNow_Click(object sender, EventArgs e)
        {
            // Call a method on the owner form to trigger sync
            // We need to use reflection to call a method that might not exist on all forms
            var method = _ownerForm.GetType().GetMethod("TriggerSyncFromTray",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            method?.Invoke(_ownerForm, null);
        }

        public void ShowInTray()
        {
            _notifyIcon.Visible = true;
        }

        public void HideFromTray()
        {
            _notifyIcon.Visible = false;
        }

        public void ShowApplication()
        {
            // Show and restore the window if it was minimized
            _ownerForm.Show();
            _ownerForm.WindowState = FormWindowState.Normal;
            _ownerForm.BringToFront();
            _ownerForm.Focus();
        }

        public void ShowBalloonTip(string title, string message, ToolTipIcon icon, int timeout = 5000)
        {
            if (_notifyIcon.Visible)
            {
                _notifyIcon.BalloonTipTitle = title;
                _notifyIcon.BalloonTipText = message;
                _notifyIcon.BalloonTipIcon = icon;
                _notifyIcon.ShowBalloonTip(timeout);
            }
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

        public void SetTrayIcon(Icon icon)
        {
            _notifyIcon.Icon = icon;
        }

        public void UpdateSyncStatus(string status)
        {
            // Update the tooltip to show sync status
            _notifyIcon.Text = $"Database Sync: {status}";
        }
    }
}




