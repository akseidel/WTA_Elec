#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace WTA_Elec {
    class AppElecRibbon : IExternalApplication {
        static string _path = typeof(Application).Assembly.Location;
        public string docsPath = "N:\\CAD\\BDS PRM 2016\\WTA Common\\Revit Resources\\WTAAddins\\SourceCode\\Docs";
        /// Singleton external application class instance.
        internal static AppElecRibbon _app = null;
        /// Provide access to singleton class instance.
        public static AppElecRibbon Instance {
            get { return _app; }
        }
        /// Provide access to the radio button state
        internal static string _pb_state = String.Empty;
        public static string PB_STATE {
            get { return _pb_state; }
        }
        /// Provide access to the offset state
        internal static XYZ _pOffSet = new XYZ(1, 1, 0);
        public static XYZ POFFSET {
            get { return _pOffSet; }
        }
        public Result OnStartup(UIControlledApplication a) {
            _app = this;
            AddElec_WTA_ELEC_Ribbon(a);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a) {
            return Result.Succeeded;
        }

        public void AddElec_WTA_ELEC_Ribbon(UIControlledApplication a) {
            string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string ExecutingAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            // create ribbon tab 
            String thisNewTabName = "WTA-ELEC";
            try {
                a.CreateRibbonTab(thisNewTabName);
            } catch (Autodesk.Revit.Exceptions.ArgumentException) {
                // Assume error generated is due to "WTA" already existing
            }
            //   Add ribbon panels.
            String thisNewPanelBe = "Be This";
            RibbonPanel thisNewRibbonPanelBe = a.CreateRibbonPanel(thisNewTabName, thisNewPanelBe);

            String thisNewPanelNamLFixt = "Light Fixtures";
            RibbonPanel thisNewRibbonPanelLFixt = a.CreateRibbonPanel(thisNewTabName, thisNewPanelNamLFixt);

            String thisNewPanelNameAim = "Aiming Lights";
            RibbonPanel thisNewRibbonPanelAim = a.CreateRibbonPanel(thisNewTabName, thisNewPanelNameAim);

            //System.Windows.MessageBox.Show(a.GetRibbonPanels(thisNewTabName).Count.ToString());

            //   Create push buttons 
            PushButtonData pbTwoPickTagLight = new PushButtonData("TwoPickLightingTag", "Two Pick\nLighting Tag", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdTwoPickLightingTag");
            PushButtonData pbTwoPickTagSwitch = new PushButtonData("TwoPickSwitchTag", "Two Pick\nDevice Tag", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdTwoPickSwitchTag");
            PushButtonData pbTwoPickAimLight = new PushButtonData("AimLight", " Aim ", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdTwoPickLightRot");
            PushButtonData pbAimManyLights = new PushButtonData("AimManyLights", " Aim Many", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdAimManyLights");
        
            PushButtonData pbBeLighting = new PushButtonData("BeLighting", "Lighting", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdBeLightingWorkSet");
            PushButtonData pbBePower = new PushButtonData("BePower", "Power", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdBePowerWorkSet");
            PushButtonData pbBeAuxiliary = new PushButtonData("BeAuxiliary", "Auxiliary", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdBeAuxiliaryWorkSet");
            PushButtonData pbSelOnlyLights = new PushButtonData("SelOnlyLightFix", "Only Fixtures", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdPickOnlyLights");
            PushButtonData pbSelOnlyDevices = new PushButtonData("SelOnlyDevices", "Only Devices", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdPickOnlyDevices");
            PushButtonData pbLightingReporter = new PushButtonData("LRPT", "Room Picker", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdRoomLightingReporter");
            PushButtonData pbLightingTotReport = new PushButtonData("LRPTOT", "LPD Totalizer", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdRoomsLightPwrDensityReport");

            PushButtonData pbOCCDetTool = new PushButtonData("OCCDetTool", "OCC Det Tool", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdPlaceOCCSensorToolInstance");
            pbOCCDetTool.ToolTip = "Places Occupancy sensor detection tool.";
            string lDescOCCDetTool = "Requires a linked ceiling pick to set the sensor detection family tool's parameter. The tool adjusts the detection pattern " +
                                    "according to the ceiling height from the ceiling pick. Without a ceiling, place otherwise and manually change the instance parameter as needed. " + 
                                    "Use the family's type selector to get the detection pattern you are interested in. " +
                                    "The tool is placed on a diagnostic workset that will be created if needed.";
            pbOCCDetTool.LongDescription = lDescOCCDetTool;
            pbOCCDetTool.ToolTipImage = NewBitmapImage(System.Reflection.Assembly.GetExecutingAssembly(), ExecutingAssemblyName + ".SENSDETLG.PNG");
            pbOCCDetTool.Image = NewBitmapImage(System.Reflection.Assembly.GetExecutingAssembly(), ExecutingAssemblyName + ".SENSDETSM.PNG");


            //   Set the large image shown on button
            //Note that the full image name is namespace_prefix + "." + the actual imageName);
            pbTwoPickTagLight.LargeImage = NewBitmapImage(System.Reflection.Assembly.GetExecutingAssembly(), ExecutingAssemblyName + ".TwoPickTag.png");
            pbTwoPickTagSwitch.LargeImage = NewBitmapImage(System.Reflection.Assembly.GetExecutingAssembly(), ExecutingAssemblyName + ".TwoPickTagSwitch.png");

            // add button tips (when data, must be defined prior to adding button.)
            pbTwoPickTagLight.ToolTip = "Places lighting tag in two or less picks.";
            pbTwoPickTagSwitch.ToolTip = "Places device tag in two or less picks.";

            pbTwoPickAimLight.ToolTip = "2D Aims a non hosted light.";
            pbAimManyLights.ToolTip = "2D Aims a selection of non hosted lights.";
         
            pbBeLighting.ToolTip = "Switch to Lighting Workset.";
            pbBePower.ToolTip = "Switch to Power Workset.";
            pbBeAuxiliary.ToolTip = "Switch to Auxiliary Workset.";
            pbSelOnlyLights.ToolTip = "Selecting only lighting fixtures.";
            pbSelOnlyDevices.ToolTip = "Selecting only lighting devices.";

            pbLightingReporter.ToolTip = "Reports on all lighting in a room.";

            string lDescpbTwoPickTagLight = "Places the lighting tag in two picks.\nThe first pick selects the light fixture.\nThe second pick is the tag location.";
            string lDescpbTwoPickAimLight = "Pick a light.\nThen pick where it is supposed to aim.";
            string lDescpbAimManyLights = "Select a bunch of lights.\nThen pick the one spot where they all should aim towards.";
            string lDescpb3DAim = "The special element has to be a Sprinkler category family instance.";
            string lDescSelOnlyLights = "Swipe over anything. Only lighting fixtures are selected.";
            string lDescSelOnlyDevices = "Swipe over anything. Only lighting devices are selected.";
            string lDescBeLighting = "If you can't beat'm, join'm. Become Elec Lighting workset.";
            string lDescBePower = "If you can't beat'm, join'm. Become Elec Power workset.";
            string lDescBeAuxiliary = "If you can't beat'm, join'm. Become Elec Auxiliary workset.";

            pbTwoPickTagLight.LongDescription = lDescpbTwoPickTagLight;
            pbTwoPickAimLight.LongDescription = lDescpbTwoPickAimLight;
            pbAimManyLights.LongDescription = lDescpbAimManyLights;
            pbSelOnlyLights.LongDescription = lDescSelOnlyLights;
            pbSelOnlyDevices.LongDescription = lDescSelOnlyDevices;
            pbBeLighting.LongDescription = lDescBeLighting;
            pbBePower.LongDescription = lDescBePower;
            pbBeAuxiliary.LongDescription = lDescBeAuxiliary;

            // add to ribbon panelA
            List<RibbonItem> projectButtonsA = new List<RibbonItem>();
            projectButtonsA.AddRange(thisNewRibbonPanelBe.AddStackedItems(pbBeLighting, pbBePower, pbBeAuxiliary));

            // add to ribbon panelB
            thisNewRibbonPanelLFixt.AddItem(pbTwoPickTagLight);
            thisNewRibbonPanelLFixt.AddItem(pbTwoPickTagSwitch);
            thisNewRibbonPanelLFixt.AddSeparator();
            List<RibbonItem> projectButtonsB = new List<RibbonItem>();
            projectButtonsB.AddRange(thisNewRibbonPanelLFixt.AddStackedItems(pbSelOnlyLights, pbSelOnlyDevices));
            thisNewRibbonPanelLFixt.AddSeparator();
            List<RibbonItem> projectButtonsBB = new List<RibbonItem>();
            projectButtonsBB.AddRange(thisNewRibbonPanelLFixt.AddStackedItems(pbLightingReporter, pbOCCDetTool, pbLightingTotReport));

            // add to ribbon panelC
            List<RibbonItem> projectButtonsC = new List<RibbonItem>();
            projectButtonsC.AddRange(thisNewRibbonPanelAim.AddStackedItems(pbTwoPickAimLight, pbAimManyLights));
            //projectButtons.AddRange(thisNewRibbonPanel.AddStackedItems(pbData2DN, pbData4DN, pbDataAPN));

            thisNewRibbonPanelBe.AddSlideOut();
            PushButtonData bInfo = new PushButtonData("Info", "Info", ExecutingAssemblyPath, ExecutingAssemblyName + ".CmdOpenDocFolder");
            bInfo.ToolTip = "See the help document regarding this.";
            bInfo.LargeImage = NewBitmapImage(System.Reflection.Assembly.GetExecutingAssembly(), ExecutingAssemblyName + ".InfoLg.png");
            thisNewRibbonPanelBe.AddItem(bInfo);

            thisNewRibbonPanelLFixt.AddSlideOut();
            thisNewRibbonPanelLFixt.AddItem(bInfo);

        } // AddMech_WTA_Elec_Ribbon

        /// <summary>
        /// Load a new icon bitmap from embedded resources.
        /// For the BitmapImage, make sure you reference WindowsBase and Presentation Core
        /// and PresentationCore, and import the System.Windows.Media.Imaging namespace. 
        /// </summary>
        BitmapImage NewBitmapImage(System.Reflection.Assembly a, string imageName) {
            Stream s = a.GetManifestResourceStream(imageName);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = s;
            img.EndInit();
            return img;
        }

        //public void SetAs_STD() {
        //    _pb_state = "STD";
        //    //System.Windows.MessageBox.Show(_pb_state,"_pb_state was set to");
        //}
        //public void SetAs_EC() {
        //    _pb_state = "EC";
        //    //System.Windows.MessageBox.Show(_pb_state, "_pb_state was set to");
        //}

        //public void SetPlunkOffset(double offX, double offY, double OffZ) {
        //    _pOffSet = new XYZ(offX, offY, OffZ);
        //}

    }
}
