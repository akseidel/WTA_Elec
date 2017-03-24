using System;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using IWin32Window = System.Windows.Forms.IWin32Window;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Controls;
using Autodesk.Windows;

namespace WTA_Elec {
    /// <summary>
    /// Interaction logic for SelectForSettingWPF.xaml
    /// </summary>
    public partial class TwoPickTagWPF : Window {
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(IntPtr hWnd);

        string _purpose;
        string curQuickFltrStr = "";
        string delim = "|"; // DO NOT CHANGE
        DispatcherTimer timeOut = new DispatcherTimer();
        Document _doc;
        bool SuspendEvents = false;
        public string LastContextMode = "LT";
        public string CurContextMode = "LT";
        const string LTMode = "LT";
        const string SWMode = "SW";
        string PrefLTTagFltr;
        string PrefSWTagFltr;
        string pfFamLTTagPair = "";
        string pfFamSWTagPair = "";
        List<string> tagNamTypeLstLT = new List<string>();
        List<string> tagNamTypeLstSW = new List<string>();
        public bool ContextChangeWanted = false;
        public FormMsgWPF autoTimeOutMsg;

        IWin32Window _revit_window;
        IntPtr _revWinIntPtr;  // used for returning focus

        public TwoPickTagWPF(Document doc, IntPtr revWinIntPtr) {
            InitializeComponent();
            _doc = doc;
            _revWinIntPtr = revWinIntPtr;
            _revit_window = new JtWindowHandle(ComponentManager.ApplicationWindow);
            Top = Properties.Settings.Default.FormMSG_Top;
            Left = Properties.Settings.Default.FormMSG_Left;
            CurContextMode = Properties.Settings.Default.LastContextMode;
            btn_Close.IsCancel = true;
        }

        public void SetMsg(string _msg, string purpose, string _bot = "") {
            _purpose = purpose;
            this.MsgTextBlockMainMsg.Text = _msg;
            this.MsgLabelTop.Content = purpose;
            if (_bot != "") {
                //this.MsgLabelBot.Content = _bot;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            SuspendEvents = true;
            _purpose = MsgTextBlockMainMsg.Text;
            timeOut.Tick += new EventHandler(timeOut_Tick);
            GetInitialUserSetting();
            ApplyUserSetting();
            InitSelectorCombo(true);
            tboxQfltr.Focus();
            SuspendEvents = false;
        }

        private void GetInitialUserSetting() {
            CurContextMode = Properties.Settings.Default.LastContextMode;
            if (CurContextMode == "") { CurContextMode = LTMode; }
            PrefLTTagFltr = Properties.Settings.Default.PrefLTTagFltr;
            pfFamLTTagPair = Properties.Settings.Default.PreferedLTTag;
            PrefSWTagFltr = Properties.Settings.Default.PrefSWTagFltr;
            pfFamSWTagPair = Properties.Settings.Default.PreferedSWTag;
            chkWithLeader.IsChecked = Properties.Settings.Default.PrefLeader;
            BeNew.IsChecked = Properties.Settings.Default.tbNewState;
            BeExist.IsChecked = Properties.Settings.Default.tbExistState;
            chkSyncCntrl_SW.IsChecked = Properties.Settings.Default.chkSync;
            ChkTagOption.IsChecked = Properties.Settings.Default.TagOtherViews;
            switch (CurContextMode) {
                case LTMode:
                    Dispatcher.BeginInvoke((Action)(() => tabContext.SelectedIndex = 0));
                    break;
                case SWMode:
                    Dispatcher.BeginInvoke((Action)(() => tabContext.SelectedIndex = 1));
                    break;
                default:
                    break;
            }
        }

        private void ApplyUserSetting() {
            string tempStr;
            switch (CurContextMode) {
                case LTMode:
                    // must happen prior to next setting
                    tempStr = Properties.Settings.Default.PrefLTTagFltr;
                    tboxQfltr.Text = tempStr;
                    PrefLTTagFltr = tempStr;
                    String pfLTag = Properties.Settings.Default.PreferedLTTag;
                    if (pfLTag != null) {
                        if (pfTagNamePair.Items.Contains(pfLTag)) {
                            pfTagNamePair.Text = pfLTag;
                        }
                    }
                    break;
                case SWMode:
                    // must happen prior to next setting
                    tempStr = Properties.Settings.Default.PrefSWTagFltr;
                    tboxQfltr.Text = tempStr;
                    PrefSWTagFltr = tempStr;
                    String pfSWTag = Properties.Settings.Default.PreferedSWTag;
                    if (pfSWTag != null) {
                        if (pfTagNamePair.Items.Contains(pfSWTag)) {
                            pfTagNamePair.Text = pfSWTag;
                        }
                    }
                    break;
                default:
                    break;
            }
            chkWithLeader.IsChecked = Properties.Settings.Default.PrefLeader;
        }

        private void InitSelectorCombo(bool scanDocDB) {
            pfTagNamePair.Items.Clear();
            switch (CurContextMode) {
                case LTMode:
                    if (scanDocDB) {
                        // Dictionary of Family Tagmanes and their symbols (i.e. types)
                        Dictionary<string, List<FamilySymbol>> lightFixTagTypes = FamilyUtils.FindFamilyTypes(_doc, BuiltInCategory.OST_LightingFixtureTags);
                        // Now make a pairings list
                        tagNamTypeLstLT.Clear();
                        foreach (KeyValuePair<string, List<FamilySymbol>> entry in lightFixTagTypes) {
                            string tagName = entry.Key;
                            foreach (FamilySymbol item in entry.Value) {
                                string tagNameTypePairing = tagName + delim + item.Name;
                                tagNamTypeLstLT.Add(tagNameTypePairing);
                            }
                        }
                    }
                    // Rebuild pairings list to only those entries that satisfy filter strings
                    if (curQuickFltrStr.Length > 0) {
                        //List<string> qFltrList = tagNamTypeLst.FindAll(t => t.ToLower().Contains(_qFltr));
                        // lists only that satisfy containing every filter item separated by a comma
                        string fltrStr = tboxQfltr.Text;
                        char chrDelim = ','; // DO NOT CHANGE
                        string[] words = fltrStr.Split(chrDelim);
                        List<string> keywords = words.Select(x => x.Trim()).ToList();
                        List<string> qFltrList = tagNamTypeLstLT.FindAll(t => keywords.All(t.ToLower().Contains));
                        tagNamTypeLstLT = qFltrList;
                    }
                    // Fill combobox using list in sorted form and stripping off OST_.
                    try {
                        foreach (string itm in tagNamTypeLstLT.OrderBy(i => i)) {
                            if (itm != "INVALID") {
                                pfTagNamePair.Items.Add(itm.Replace("OST_", ""));
                            }
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Problem at InitSelectorCombo()");
                    }

                    pfTagNamePair.SelectedIndex = 0;
                    if (pfFamLTTagPair != null) {
                        if (pfTagNamePair.Items.Contains(pfFamLTTagPair)) {
                            pfTagNamePair.Text = pfFamLTTagPair;
                        }
                    }
                    MsgTextBlockMainMsg.Text = "Set Tag | Type" + " (" + tagNamTypeLstLT.Count.ToString() + ")";
                    break;

                case SWMode:
                    if (scanDocDB) {
                        // Dictionary of Family Tagmanes and their symbols (i.e. types)
                        Dictionary<string, List<FamilySymbol>> switchTagTypes = FamilyUtils.FindFamilyTypes(_doc, BuiltInCategory.OST_LightingDeviceTags);
                        // Now make a pairings list
                        tagNamTypeLstSW.Clear();
                        foreach (KeyValuePair<string, List<FamilySymbol>> entry in switchTagTypes) {
                            string tagName = entry.Key;
                            foreach (FamilySymbol item in entry.Value) {
                                string tagNameTypePairing = tagName + delim + item.Name;
                                tagNamTypeLstSW.Add(tagNameTypePairing);
                            }
                        }
                    }
                    // Rebuild pairings list to only those entries that satisfy filter strings
                    if (curQuickFltrStr.Length > 0) {
                        //List<string> qFltrList = tagNamTypeLst.FindAll(t => t.ToLower().Contains(_qFltr));
                        // lists only that satisfy containing every filter item separated by a comma
                        string fltrStr = tboxQfltr.Text;
                        char chrDelim = ','; // DO NOT CHANGE
                        string[] words = fltrStr.Split(chrDelim);
                        List<string> keywords = words.Select(x => x.Trim()).ToList();
                        List<string> qFltrList = tagNamTypeLstSW.FindAll(t => keywords.All(t.ToLower().Contains));
                        tagNamTypeLstSW = qFltrList;
                    }
                    // Fill combobox using list in sorted form and stripping off OST_.
                    try {
                        foreach (string itm in tagNamTypeLstSW.OrderBy(i => i)) {
                            if (itm != "INVALID") {
                                pfTagNamePair.Items.Add(itm.Replace("OST_", ""));
                            }
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Problem at InitSelectorCombo()");
                    }
                    pfTagNamePair.SelectedIndex = 0;
                    if (pfFamSWTagPair != null) {
                        if (pfFamSWTagPair != "") {
                            if (pfTagNamePair.Items.Contains(pfFamSWTagPair)) {
                                pfTagNamePair.Text = pfFamSWTagPair;
                            }
                        }
                    }
                    MsgTextBlockMainMsg.Text = "Set Tag | Type" + " (" + tagNamTypeLstSW.Count.ToString() + ")";
                    break;
                default:
                    break;
            }
        }

        private void Window_Closing(object sender,
            System.ComponentModel.CancelEventArgs e) {
            if (autoTimeOutMsg != null) { autoTimeOutMsg.Close(); }
            SaveSettings();
        }

        public void SaveSettings() {
            try {
                // save from the very last tap view change
                switch (CurContextMode) {
                    case LTMode:
                        PrefLTTagFltr = tboxQfltr.Text;
                        pfFamLTTagPair = pfTagNamePair.Text;
                        break;
                    case SWMode:
                        PrefSWTagFltr = tboxQfltr.Text;
                        pfFamSWTagPair = pfTagNamePair.Text;
                        break;
                    default:
                        break;
                }
                Properties.Settings.Default.FormMSG_Top = Top;
                Properties.Settings.Default.FormMSG_Left = Left;
                Properties.Settings.Default.LastContextMode = CurContextMode;
                Properties.Settings.Default.tbNewState = (bool)BeNew.IsChecked;
                Properties.Settings.Default.tbExistState = (bool)BeExist.IsChecked;
                Properties.Settings.Default.chkSync = (bool)chkSyncCntrl_SW.IsChecked;
                Properties.Settings.Default.TagOtherViews = (bool)ChkTagOption.IsChecked;
                Properties.Settings.Default.PreferedLTTag = pfFamLTTagPair;
                Properties.Settings.Default.PreferedSWTag = pfFamSWTagPair;
                Properties.Settings.Default.PrefLTTagFltr = PrefLTTagFltr;
                Properties.Settings.Default.PrefSWTagFltr = PrefSWTagFltr;
                Properties.Settings.Default.Save();
                // MessageBox.Show("LTP+> " + pfFamLTTagPair + "\n" + "SWP+> " + pfFamSWTagPair);
            } catch (Exception) {
                MessageBox.Show("For some unknown reason.\n\nNo change was made.",
                    "Settings Error");
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        public void DragWindow(object sender, MouseButtonEventArgs args) {
            timeOut.Stop();
            // Watch out. Fatal error if not primary button!
            if (args.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e) {
            MsgTextBlockMainMsg.Text = "Position Where You Like.";
            //ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            timeOut.Stop();
            timeOut.Interval = new TimeSpan(0, 0, 1);
            timeOut.Start();
        }

        private void Window_LocationChanged(object sender, EventArgs e) {
            timeOut.Stop();
            timeOut.Interval = new TimeSpan(0, 0, 1);
            timeOut.Start();
        }

        private void timeOut_Tick(object sender, EventArgs e) {
            timeOut.Stop();
            MsgTextBlockMainMsg.Text = _purpose;
            ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            curQuickFltrStr = tboxQfltr.Text.ToLower();
            InitSelectorCombo(true);
            //SaveComboTagParing("TextBox_TextChanged");
            SetPrefFltrsFromQFltrTboxPerMode();
        }

        private void btn_Clr_Click(object sender, RoutedEventArgs e) {
            this.tboxQfltr.Clear();
            SetPrefFltrsFromQFltrTboxPerMode();
            BeNew.IsChecked = false;
            BeExist.IsChecked = false;
        }

        private void chkWithLeader_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.PrefLeader = (bool)chkWithLeader.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e) {
            SaveSettings();
            SetForegroundWindow(_revWinIntPtr);
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (!SuspendEvents) {
                // The exception is done on purpose to prevent reentrancy bugs caused by weirdness resulting from
                // altering the visual tree, while such an event (which itself has been triggered by the visual tree
                // altering) is firing. If you really must confirm something when the state of a UI element changes,
                // delaying with Dispatcher.BeginInvoke is probably the right thing to do.
                // ALL HELL BREAKS LOOSE IF NOT DONE THIS WAY!
                //if (tab_LT != null && tab_LT.IsSelected) {
                //    LastTagMode = SWMode;
                //    CurTagMode = LTMode;
                //    // set the common controls from last
                //    tboxQfltr.Text = PrefLTTagFltr;
                //    pfTagNamePair.Text = pfFamLTTagPair;
                //}
                //if (tab_SW != null && tab_SW.IsSelected) {
                //    LastTagMode = LTMode;
                //    CurTagMode = SWMode;
                //    // set the common controls from last
                //    tboxQfltr.Text = PrefSWTagFltr;
                //    pfTagNamePair.Text = pfFamSWTagPair;
                //}
                Dispatcher.BeginInvoke(new Action(() => DoTabChangeReactions()));
                Dispatcher.BeginInvoke(new Action(() => InitSelectorCombo(true)));
                Dispatcher.BeginInvoke(new Action(() => ConformQFltrToNewExistButtons()));
            }
        }

        private void DoTabChangeReactions() {
            if (tab_LT != null && tab_LT.IsSelected) {
                LastContextMode = SWMode;
                CurContextMode = LTMode;
                //MessageBox.Show("CurContextMode " + CurContextMode);
                // set the common controls from last
                tboxQfltr.Text = PrefLTTagFltr;
                //   pfTagNamePair.Text = pfFamLTTagPair;
            }
            if (tab_SW != null && tab_SW.IsSelected) {
                LastContextMode = LTMode;
                CurContextMode = SWMode;
                //MessageBox.Show("CurContextMode " + CurContextMode);
                // set the common controls from last
                tboxQfltr.Text = PrefSWTagFltr;
                //  MessageBox.Show(pfFamSWTagPair,"TAB REACTION2");
                //   pfTagNamePair.Text = pfFamSWTagPair;
            }
        }

        private void tab_LT_MouseUp(object sender, MouseButtonEventArgs e) {
            Dispatcher.BeginInvoke(new Action(() => SetContextChangeWanted()));

            // Press.PressEsc(_revit_window.Handle);  // does nothing!
            // Press.PressEsc(_revit_window.Handle);

            //Press.PostMessage(_revit_window.Handle,
            //     (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
            //     (uint)System.Windows.Forms.Keys.Escape, 0);

            //Press.PostMessage(_revit_window.Handle,
            //  (uint)Press.KEYBOARD_MSG.WM_KEYDOWN,
            //  (uint)System.Windows.Forms.Keys.Escape, 0);

        }

        private void tab_SW_MouseUp(object sender, MouseButtonEventArgs e) {
            Dispatcher.BeginInvoke(new Action(() => SetContextChangeWanted()));
        }

        private void SetContextChangeWanted() {
            ShowContextSwitchMessage();
            ContextChangeWanted = true;
            chkEyeDropper.IsChecked = true;
            ContextChangeColorState();
        }

        private void ShowContextSwitchMessage() {
            autoTimeOutMsg = new FormMsgWPF(10);
            autoTimeOutMsg.SetMsg("Make the selection type Revit is asking, even though it seems illogical.", "Dummy Selection Required");
            autoTimeOutMsg.Show();
        }

        public void MatchingColorState() {
            Body.Background = ColorExt.ToBrush(System.Drawing.Color.Aquamarine);
        }

        public void ContextChangeColorState() {
            Body.Background = ColorExt.ToBrush(System.Drawing.Color.MediumAquamarine);
        }

        public void NormalColorState() {
            Body.Background = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
        }

        private void ConformQFltrToNewExistButtons() {
            DoBeNewState();
            DoBeExistState();
        }

        private void BeNew_Click(object sender, RoutedEventArgs e) {
            DoBeNewState();
        }

        private void BeExist_Click(object sender, RoutedEventArgs e) {
            DoBeExistState();
        }

        private void DoBeNewState() {
            if ((bool)BeNew.IsChecked) {
                AddValToFilter("new");
                BeExist.IsChecked = false;
                RemoveValFromFilter("exist");
            } else {
                RemoveValFromFilter("new");
            }
        }

        private void DoBeExistState() {
            if ((bool)BeExist.IsChecked) {
                AddValToFilter("exist");
                BeNew.IsChecked = false;
                RemoveValFromFilter("new");
            } else {
                RemoveValFromFilter("exist");
            }
        }

        public void SetPrefFamTagPairFromComboPerMode() {
            switch (CurContextMode) {
                case LTMode:
                    pfFamLTTagPair = pfTagNamePair.Text;
                    break;
                case SWMode:
                    pfFamSWTagPair = pfTagNamePair.Text;
                    break;
                default:
                    break;
            }
        }

        private void SetPrefFltrsFromQFltrTboxPerMode() {
            switch (CurContextMode) {
                case LTMode:
                    PrefLTTagFltr = tboxQfltr.Text;
                    break;
                case SWMode:
                    PrefSWTagFltr = tboxQfltr.Text;
                    break;
                default:
                    break;
            }
        }

        private void SetQFltrTboxPerMode() {
            switch (CurContextMode) {
                case LTMode:
                    tboxQfltr.Text = PrefLTTagFltr;
                    break;
                case SWMode:
                    tboxQfltr.Text = PrefSWTagFltr;
                    break;
                default:
                    break;
            }
        }

        private void RemoveValFromFilter(string strVal) {
            PrefLTTagFltr = RemFltrValFromThisString(PrefLTTagFltr, strVal);
            PrefSWTagFltr = RemFltrValFromThisString(PrefSWTagFltr, strVal);
            SetQFltrTboxPerMode();
        }

        private string RemFltrValFromThisString(string strTarg, string strVal) {
            if (null == strTarg) { strTarg = ""; }
            strTarg = strTarg.ToLower().Replace("," + strVal, "");
            strTarg = strTarg.ToLower().Replace(strVal + ",", "");
            strTarg = strTarg.ToLower().Replace(strVal, "");
            return strTarg;
        }

        private string AddFltrValToThisString(string strTarg, string strVal) {
            if (null == strTarg) { strTarg = ""; }
            if (!strTarg.ToLower().Contains(strVal)) {
                if (strTarg.Length == 0) {
                    strTarg += strVal;
                } else {
                    strTarg += "," + strVal;
                }
            }
            return strTarg;
        }

        private void AddValToFilter(string strVal) {
            PrefLTTagFltr = AddFltrValToThisString(PrefLTTagFltr, strVal);
            PrefSWTagFltr = AddFltrValToThisString(PrefSWTagFltr, strVal);
            SetQFltrTboxPerMode();
        }

        private void pfTagNamePair_DropDownClosed(object sender, EventArgs e) {
            SetPrefFamTagPairFromComboPerMode();
            SaveSettings();
        }

        private void pfTagNamePair_KeyUp(object sender, KeyEventArgs e) {
            SetPrefFamTagPairFromComboPerMode();
            SaveSettings();
        }

        private void chkEyeDropper_Unchecked(object sender, RoutedEventArgs e) {
            ContextChangeWanted = false;
            chkEyeDropper.IsEnabled = true;
            NormalColorState();
        }

        private void chkEyeDropper_Checked(object sender, RoutedEventArgs e) {
            if (ContextChangeWanted) { chkEyeDropper.IsEnabled = false; }
            if (!ContextChangeWanted) { MatchingColorState(); }
        }

        private void chkSyncCntrl_LT_Checked(object sender, RoutedEventArgs e) {
            chkSyncCntrl_SW.IsChecked = true;
        }

        private void chkSyncCntrl_SW_Checked(object sender, RoutedEventArgs e) {
            chkSyncCntrl_LT.IsChecked = true;
        }

        private void chkSyncCntrl_SW_Unchecked(object sender, RoutedEventArgs e) {
            chkSyncCntrl_LT.IsChecked = false;
        }

        private void chkSyncCntrl_LT_Unchecked(object sender, RoutedEventArgs e) {
            chkSyncCntrl_SW.IsChecked = false;
        }

        private void PCNTRL_LT_TextChanged(object sender, TextChangedEventArgs e) {
            if ((bool)chkSyncCntrl_LT.IsChecked || PCNTRL_LT.Text.Trim().Length > 0) {
                PCNTRL_SW.Text = PCNTRL_LT.Text;
            }
        }

        private void PCNTRL_SW_TextChanged(object sender, TextChangedEventArgs e) {
            if ((bool)chkSyncCntrl_SW.IsChecked || PCNTRL_SW.Text.Trim().Length > 0) {
                PCNTRL_LT.Text = PCNTRL_SW.Text;
            }
        }
        /// <summary>
        /// The usual suspects did not work????
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkTagOption_MouseLeave(object sender, MouseEventArgs e) {
            Properties.Settings.Default.TagOtherViews = (bool)ChkTagOption.IsChecked;
            Properties.Settings.Default.Save();
        }
    } // end partial class

} // end namesapce
