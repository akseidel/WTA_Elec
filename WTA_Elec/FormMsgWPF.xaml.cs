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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WTA_Elec {
    /// <summary>
    /// Interaction logic for FormMsg.xaml
    /// </summary>
    public partial class FormMsgWPF : Window {
        Brush ClrA;
        Brush ClrB;
        DispatcherTimer timeOut = new DispatcherTimer();

        public FormMsgWPF() {
            InitializeComponent();
            this.Top = Properties.Settings.Default.FormMSG_Top;
            this.Left = Properties.Settings.Default.FormMSG_Left;
            this.Height = Properties.Settings.Default.FormMSG_HT;
            this.Width = Properties.Settings.Default.FormMSG_WD;
        }
        public void SetMsg(string _msg, string _purpose, string _bot = "") {
            this.MsgTextBlockMainMsg.Text = _msg;
            this.MsgLabelTop.Content = _purpose;
            if (_bot != "") {
                this.MsgLabelBot.Content = _bot;
            }
            FlipColor();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Properties.Settings.Default.FormMSG_Top = this.Top;
            Properties.Settings.Default.FormMSG_Left = this.Left;
            Properties.Settings.Default.FormMSG_HT = this.Height;
            Properties.Settings.Default.FormMSG_WD = this.Width;
            Properties.Settings.Default.Save();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.WindowStyle = WindowStyle.None;
            RandomColorPair();
            timeOut.Tick += new EventHandler(timeOut_Tick);
            }
        private void DockPanel_MouseEnter(object sender, MouseEventArgs e) {
            this.WindowStyle = WindowStyle.ToolWindow;
            //RandomColorPair();
            //FlipColor();
            timeOut.Interval = new TimeSpan(0, 0, 2);
            timeOut.Start();
        }
        private void DockPanel_MouseUp(object sender, MouseButtonEventArgs e) {
            if (this.WindowStyle == WindowStyle.ToolWindow) {
                this.WindowStyle = WindowStyle.None;
            }
        }
        private void Window_LocationChanged(object sender, EventArgs e) {
            if (this.WindowStyle == WindowStyle.ToolWindow) {
                this.WindowStyle = WindowStyle.None;
                timeOut.Interval = new TimeSpan(0, 0, 2);
                timeOut.Start();
            }
        }
        private void timeOut_Tick(object sender, EventArgs e) {
            timeOut.Stop();
            this.WindowStyle = WindowStyle.None;
        }
        private void FlipColor() {
            if (this.Background == ClrA) {
                this.Background = ClrB;
            } else {
                this.Background = ClrA;
            }
        }
        private void RandomColorPair() {
            Random rand = new Random();
            int randInt = rand.Next(0, 4);
            switch (randInt) {
                case 0:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.Aqua);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.Aquamarine);
                    break;
                case 1:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.PapayaWhip);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.PeachPuff);
                    break;
                case 2:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.Bisque);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.BlanchedAlmond);
                    break;
                case 3:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.Lavender);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.LavenderBlush);
                    break;
                default:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.Aqua);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.Aquamarine);
                    break;
            }
        }
    }

    /// <summary>
    /// Used to convert system drawing colors to WPF brush
    /// </summary>
    public static class ColorExt {
        public static System.Windows.Media.Brush ToBrush(System.Drawing.Color color) {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }


}
