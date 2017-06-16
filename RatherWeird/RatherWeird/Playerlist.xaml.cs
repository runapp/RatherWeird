using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public Users Players = new Users();

        private readonly DispatcherTimer _refreshContentFromWeb = new DispatcherTimer();
        private HwndSource _hwndMyself;

        public Playerlist()
        {
            InitializeComponent();

            _refreshContentFromWeb.Interval = TimeSpan.FromSeconds(1);
            _refreshContentFromWeb.Tick += Tmr_Tick;

            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hwndMyself = (HwndSource)PresentationSource.FromDependencyObject(this);
            _hwndMyself?.AddHook(WindowProc);

            _refreshContentFromWeb.Start();

            lstPlayers.ItemsSource = Players;
        }

        private async void Tmr_Tick(object sender, EventArgs e)
        {
            Game ra3 = await CncOnlineInfo.FetchRa3(Constants.CncOnlineInfo);
            
            CncOnlineInfo.RefreshUsers(Constants.CncOnlineInfo, Players);
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

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstPlayers.SelectedItems.Count <= 0)
                return;

            Player selectedPlayer = lstPlayers.SelectedItems[0] as Player;
            if (selectedPlayer == null)
                return;

            Title = selectedPlayer.Nickname;
        }
        
        private void lstPlayers_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListBox box = sender as ListBox;

            if (box == null)
                return;

            if (box.SelectedItems.Count <= 0)
                return;

            Player selectedPlayer = box.SelectedItems[0] as Player;

            if (selectedPlayer == null)
                return;

            miWatchPlayer.Header = $"Watch {selectedPlayer.Nickname}";
        }
    }
}
