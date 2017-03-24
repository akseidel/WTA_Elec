using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WTA_Elec {
    /// <summary>
    /// Interaction logic for FormMsg.xaml
    /// </summary>
    public partial class FormMsgWPF : Window {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);
        Brush ClrA = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
        Brush ClrB = ColorExt.ToBrush(System.Drawing.Color.Cornsilk);
        string _purpose;
        bool _closable;
        bool _anErr;
        DispatcherTimer timeOut = new DispatcherTimer();
        int _optTimeOut; // used for auto timeout mode

        // This is a specialized version of FormMsgWPF. It has a timeout feature.
        public FormMsgWPF(int optTimeOut = 0, bool closable = false, bool anErr = false) {
            InitializeComponent();
            _closable = closable;
            _anErr = anErr;
            Top = Properties.Settings.Default.FormMSG_Top;
            Left = Properties.Settings.Default.FormMSG_Left;
            _optTimeOut = optTimeOut;
        }
        public void SetMsg(string _msg, string purpose, string _bot = "", bool LeftText = false) {
            _purpose = purpose;
            MsgTextBlockMainMsg.Text = _msg;
            MsgLabelTop.Text = purpose;
            /// tag behavior
            ChkTagOption.IsChecked = Properties.Settings.Default.TagOtherViews;
            if (purpose.Contains("Tag")) {
                TagOption.Visibility = System.Windows.Visibility.Visible;
            } else {
                TagOption.Visibility = System.Windows.Visibility.Collapsed;
            }
            /// lefttext option
            if (LeftText) { MsgTextBlockMainMsg.TextAlignment = TextAlignment.Left; }
            /// timeout option
            if (_optTimeOut > 0) {  // auto timeout mode
                MsgLabelBot.Visibility = System.Windows.Visibility.Collapsed;
            }
            /// closable option
            if (_closable) {
                MsgLabelBot.Text = "Ok, I get it.";
                MsgLabelBot.FontSize = 18;
                if (_anErr) { ClrA = ColorExt.ToBrush(System.Drawing.Color.LavenderBlush);
                Body.BorderBrush = ColorExt.ToBrush(System.Drawing.Color.Red);
                }
            } else {
                if (_bot != "") {
                    MsgLabelBot.Text = _bot;
                }
            }
            FlipColor();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            timeOut.Stop();
            Properties.Settings.Default.FormMSG_Top = Top;
            Properties.Settings.Default.FormMSG_Left = Left;
            Properties.Settings.Default.Save();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            RandomColorPair();
            timeOut.Tick += new EventHandler(timeOut_Tick);
            if (_optTimeOut > 0) {
                SetTimer(_optTimeOut);
            }
        }
        private void SetTimer(int _optTimeOut) {
            timeOut.Stop();
            timeOut.Interval = new TimeSpan(0, 0, _optTimeOut);
            timeOut.Start();
        }
        private void DockPanel_MouseEnter(object sender, MouseEventArgs e) {
            if (_optTimeOut == 0) {
                MsgLabelTop.Text = "Position As You Like.";
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                SetTimer(1);
            }
        }
        private void Window_LocationChanged(object sender, EventArgs e) {
            if (_optTimeOut == 0) { SetTimer(1); } else { SetTimer(_optTimeOut); }
        }
        private void timeOut_Tick(object sender, EventArgs e) {
            timeOut.Stop();
            if (_optTimeOut > 0) { // auto timeout mode
                Close();
            } else {  // normal mode
                MsgLabelTop.Text = _purpose;
                ResizeMode = System.Windows.ResizeMode.NoResize;
            }
        }
        private void FlipColor() {
            if (Body.Background == ClrA) {
                Body.Background = ClrB;
            } else {
                Body.Background = ClrA;
            }
        }
        private void RandomColorPair() {
            Random rand = new Random();
            int randInt = rand.Next(0, 1);
            switch (randInt) {
                case 0:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.Cornsilk);
                    break;
                case 1:
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.Cornsilk);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
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
                    ClrA = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
                    ClrB = ColorExt.ToBrush(System.Drawing.Color.Cornsilk);
                    break;
            }
        }
        public void DragWindow(object sender, MouseButtonEventArgs args) {
            timeOut.Stop();
            // Watch out. Fatal error if not primary button!
            if (args.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        /// <summary>
        /// The usual suspects did not work????
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkTagOption_MouseLeave(object sender, MouseEventArgs e) {
           Properties.Settings.Default.TagOtherViews = (bool)ChkTagOption.IsChecked;
           Properties.Settings.Default.Save();
           SetForegroundWindow(Autodesk.Windows.ComponentManager.ApplicationWindow.ToInt32());
        }

        private void MsgLabelBot_MouseEnter(object sender, MouseEventArgs e) {
            if (_closable) {
                Close();
            }
        }

        private void DockPanel_MouseLeave(object sender, MouseEventArgs e) {
            SetForegroundWindow(Autodesk.Windows.ComponentManager.ApplicationWindow.ToInt32());
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
