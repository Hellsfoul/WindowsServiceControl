using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ServiceProcess;       // from nuget packet System.ServiceProcess.ServiceController
using System.Reflection;
using System.IO;
using System.Drawing;

namespace WindowsServiceControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// https://www.youtube.com/watch?v=8NuoBe1HS0U
    /// </summary>
    public partial class App : Application
    {
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon;
        private readonly ServiceController service = new ServiceController("WireGuardTunnel$Office-Alexander");  // Test: AdobeARMservice  Prod: WireGuardTunnel$Office-Alexander
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;

        public App()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                Stream streamAppIcon = assembly.GetManifestResourceStream("WindowsServiceControl.Resources.AppIcon.ico");
                Stream streamPlus = assembly.GetManifestResourceStream("WindowsServiceControl.Resources.add_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24.png");
                Stream streamExit = assembly.GetManifestResourceStream("WindowsServiceControl.Resources.close_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24.png");
                Icon appIcon = new Icon(streamAppIcon);
                Bitmap plusIcon = new Bitmap(streamPlus);
                Bitmap exitIcon = new Bitmap(streamExit);

                _notifyIcon = new System.Windows.Forms.NotifyIcon()
                {

                    Icon = appIcon, // Ensure you have an icon file named icon.ico
                    Visible = true,
                    Text = "Windows Service Control"
                };

                #region Add Context Menu Items


                contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

                contextMenuStrip.Items.Add("Connect VPN", plusIcon, (sender, e) =>
                {
                    //_notifyIcon.ShowBalloonTip(100, "VPN Connection", "Connecting to VPN...", ToolTipIcon.Info);
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        contextMenuStrip.Items[0].Enabled = false;
                        contextMenuStrip.Items[1].Enabled = true;
                        _notifyIcon.ShowBalloonTip(100, "Info", "VPN connected", System.Windows.Forms.ToolTipIcon.Info);
                    }

                });

                contextMenuStrip.Items.Add("Disconntect VPN", exitIcon, (sender, e) =>
                {
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        contextMenuStrip.Items[1].Enabled = false;
                        contextMenuStrip.Items[0].Enabled = true;
                        _notifyIcon.ShowBalloonTip(100, "Info", "VPN disconnected", System.Windows.Forms.ToolTipIcon.Info);
                    }
                });

                contextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());

                contextMenuStrip.Items.Add("Exit", null, (sender, e) =>
                {
                    MessageBoxResult msgBoxresult = System.Windows.MessageBox.Show("Do you really want to close the application?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                    if (msgBoxresult == MessageBoxResult.Yes)
                        App.Current.Shutdown();
                });


                _notifyIcon.ContextMenuStrip = contextMenuStrip;


                //----------------------------


                #endregion


                isServiceRunning();

                _notifyIcon.ContextMenuStrip.Opening += (s, e) =>
                {
                    isServiceRunning();
                };



                //_notifyIcon.BalloonTipClicked += (s, e) =>
                //{
                //    System.Windows.MessageBox.Show("Balloon tip clicked!");
                //};

                //// Show on left click
                //_notifyIcon.Click += (s, e) =>
                //{
                //    _notifyIcon.ContextMenuStrip.Show(System.Windows.Forms.Control.MousePosition);
                //};
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error initializing NotifyIcon: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }



        }


        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;  // Otherwise the application will exit when the main window is closed.
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }


        void isServiceRunning()
        {
            service.Refresh();
            if (service.Status == ServiceControllerStatus.Running)
            {
                contextMenuStrip.Items[0].Enabled = false;
                contextMenuStrip.Items[1].Enabled = true;
            }
            else
            {
                contextMenuStrip.Items[0].Enabled = true;
                contextMenuStrip.Items[1].Enabled = false;
            }
        }

    }

}