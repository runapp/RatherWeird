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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RatherWeird
{
    /// <summary>
    /// Interaktionslogik für Playerlist.xaml
    /// </summary>
    public partial class Playerlist : Window
    {

        private readonly DispatcherTimer _refreshContentFromWeb = new DispatcherTimer();
        private HwndSource _hwndMyself;
        public Playerlist()
        {
            InitializeComponent();

            _refreshContentFromWeb.Interval = TimeSpan.FromSeconds(1);
            _refreshContentFromWeb.Tick += Tmr_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hwndMyself = (HwndSource)PresentationSource.FromDependencyObject(this);
            _hwndMyself?.AddHook(WindowProc);

            _refreshContentFromWeb.Start();
        }

        private async void Tmr_Tick(object sender, EventArgs e)
        {
            Game ra3 = await CncOnlineInfo.FetchRa3(Constants.CncOnlineInfo);
            
            lstPlayers.Items.Clear();
            foreach (var ra3User in ra3.Users)
            {
                lstPlayers.Items.Add(ra3User.Key);
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            _refreshContentFromWeb.Interval = TimeSpan.FromSeconds(30);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            _refreshContentFromWeb.Interval = TimeSpan.FromSeconds(1);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x10)
            {
                // WM_CLOSE (User clicked X)
                handled = true;
                Hide();
            }

            return IntPtr.Zero;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Unhook();
        }

        public new void Close()
        {
            Unhook();
            base.Close();
        }

        private void Unhook()
        {
            _hwndMyself?.RemoveHook(WindowProc);
        }
       
    }
}
