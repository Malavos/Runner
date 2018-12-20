using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        public MainWindow()
        {
            InitializeComponent();

            this.SourceInitialized += ScreenBlocker;

        }

        /// <summary>
        /// Handler to start up the screen blocker treatment.
        /// </summary>
        private void ScreenBlocker(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
        }

        /// <summary>
        /// Protect the screen from moving.
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// On Load event of the application.
        /// </summary>
        private void Loader(object sender, RoutedEventArgs e)
        {
            var desktopArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopArea.Right - this.Width;
            this.Top = desktopArea.Bottom - this.Height;
        }


        /// <summary>
        /// Open the configuration window.
        /// </summary>
        private void Configuration(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is Config)
                {
                    isWindowOpen = true;
                    window.Activate();
                }
            }

            if (!isWindowOpen)
            {
                Config configWindow = new Config();
                configWindow.Owner = this;
                configWindow.Show();
            }
        }

        /// <summary>
        /// Open the runner window.
        /// </summary>
        private void TestRunner(object sender, RoutedEventArgs e)
        {

        }

        private void About(object sender, RoutedEventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is About)
                {
                    isWindowOpen = true;
                    window.Activate();
                }
            }

            if (!isWindowOpen)
            {
                About aboutWindow = new About();
                aboutWindow.Owner = this;
                aboutWindow.HorizontalAlignment = HorizontalAlignment.Right;
                aboutWindow.VerticalAlignment = VerticalAlignment.Bottom;
                aboutWindow.Left = this.Left;
                aboutWindow.Top = this.Top;
                aboutWindow.Show();
            }
        }
    }
}
