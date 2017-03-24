#region Header
//
// based on examples from BuildingCoder Jeremy Tammik,
// AKS 6/27/2016
//
#endregion // Header

#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using ComponentManager = Autodesk.Windows.ComponentManager;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Keys = System.Windows.Forms.Keys;
using Autodesk.Revit.UI.Selection;
using System.Text;
using System.Runtime.InteropServices;



#endregion // Namespaces

namespace WTA_Elec {
 
    [Transaction(TransactionMode.Manual)]
    class CmdPlaceOCCSensorToolInstance : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            string wsName = "ELEC LIGHTING DIAGNOSTIC";
            string FamilyName = "OCC-SENSOR-DETECTION-CMR09";
            string FamilySymbolName = "DET BLADES ALL";
            string pNameOccHeight = "HEIGHT_ABOVE_FLOOR";

            bool oneShot = true;
            BuiltInCategory bicFamily = BuiltInCategory.OST_LightingDevices;

            if (plunkThis.NotInThisView()) { return Result.Succeeded; }

            CheckThisFamilyPairing(doc, typeof(FamilySymbol), FamilyName, FamilySymbolName, bicFamily);

            Element elemPlunked = null;
            double optOffset = plunkThis.GetCeilingHeight("OCC Sensor Tool Plunk");
            Double pOffSetX = 0.0;
            Double pOffSetY = 0.0;
            Double pOffSetZ = 0.0 + optOffset;
            Units unit = commandData.Application.ActiveUIDocument.Document.GetUnits();
            string optMSG = " : will be at " + UnitFormatUtils.Format(unit, UnitType.UT_Length, optOffset, false, false);
            if (optOffset != 0.0) {
                plunkThis.PlunkThisFamilyType(FamilyName, FamilySymbolName, wsName, bicFamily, out elemPlunked, oneShot, pOffSetX, pOffSetY, pOffSetZ, optMSG);
            }

            /// At this point there may or may not have been an element placed.
            #region SetParametersSection
            if (elemPlunked != null) {

                using (Transaction tp = new Transaction(doc, "PlunkOMatic:SetParam")) {
                    tp.Start();
                    //TaskDialog.Show(_pName, _pName);
                    Parameter parToSet = null;
                    parToSet = elemPlunked.LookupParameter(pNameOccHeight);
                    string strVal = UnitFormatUtils.Format(unit, UnitType.UT_Length, optOffset, false, false);
                    if (null != parToSet) {
                        parToSet.SetValueString(strVal); // this parameter is distance, therefore valuestring
                    } else {
                        FamilyUtils.SayMsg("Cannot Set Parameter Value: " + strVal, "... because parameter:\n" + pNameOccHeight
                            + "\ndoes not exist in the family:\n" + FamilyName
                            + "\nof Category:\n" + bicFamily.ToString().Replace("OST_", ""));
                    }
                    tp.Commit();
                }
            }
            #endregion
            return Result.Succeeded;
        }

        private void CheckThisFamilyPairing(Autodesk.Revit.DB.Document doc, Type targetType, string familyTagName, string familyTagSymbName, BuiltInCategory bic) {
            Element ConfirmTag = FamilyUtils.FindFamilyType(doc, targetType, familyTagName, familyTagSymbName, bic);
            if (ConfirmTag == null) {
                FamilyUtils.SayMsg("Road Closed", "Unable to resolve loading family " + familyTagName +
                                    " that has a type " + familyTagSymbName +
                                    "Maybe the tool settings are not correct.");
            }
        }

    } /// end class CmdPlaceOCCSensorToolInstance


    [Transaction(TransactionMode.Manual)]
    class CmdBeLightingWorkSet : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            //UIApplication _uiapp = commandData.Application;
            //UIDocument _uidoc = _uiapp.ActiveUIDocument;
            //Autodesk.Revit.ApplicationServices.Application _app = _uiapp.Application;
            //Autodesk.Revit.DB.Document _doc = _uidoc.Document;
            //string _wsName = "ELEC LIGHTING";

            //WorksetTable wst = _doc.GetWorksetTable();
            //WorksetId wsID = FamilyUtils.WhatIsThisWorkSetIDByName(_wsName, _doc);
            //if (wsID != null) {
            //    using (Transaction trans = new Transaction(_doc, "WillChangeWorkset")) {
            //        trans.Start();
            //        wst.SetActiveWorksetId(wsID);
            //        trans.Commit();
            //    }
            //}

            string _wsName = "ELEC LIGHTING";
            HelperA beThis = new HelperA();
            beThis.BeWorkset(_wsName, commandData);
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdBeAuxiliaryWorkSet : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            string _wsName = "ELEC AUXILIARY";
            HelperA beThis = new HelperA();
            beThis.BeWorkset(_wsName, commandData);
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdBePowerWorkSet : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            string _wsName = "ELEC POWER";
            HelperA beThis = new HelperA();
            beThis.BeWorkset(_wsName, commandData);
            return Result.Succeeded;
        }
    }

    class HelperA {
        public void BeWorkset(string _wsName, ExternalCommandData commandData) {
            UIApplication _uiapp = commandData.Application;
            UIDocument _uidoc = _uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = _uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;
            WorksetTable wst = _doc.GetWorksetTable();
            WorksetId wsID = FamilyUtils.WhatIsThisWorkSetIDByName(_wsName, _doc);
            if (wsID != null) {
                using (Transaction trans = new Transaction(_doc, "WillChangeWorkset")) {
                    trans.Start();
                    wst.SetActiveWorksetId(wsID);
                    trans.Commit();
                }
            } else {
                System.Windows.MessageBox.Show("Sorry but there is no workset "
                    + _wsName + " to switch to.", "Smells So Bad It Has A Chain On It",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Exclamation);
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickLightingTag : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            IWin32Window _revit_window;
            _revit_window = new JtWindowHandle(ComponentManager.ApplicationWindow);

            IntPtr revWinIntPtr = ComponentManager.ApplicationWindow;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            string _FamilyTagName = "LFT_UNIVERSAL_NEW";
            string _FamilyTagSymbName = "QTY_TYPE_CIRC_CONTROL";

            string tagContext = "LT";
            string tagPref = "PreferedLTTag";
            PlunkOClass.ChangeToSavedFamTypePairing(tagPref, ref _FamilyTagName, ref _FamilyTagSymbName);
            Properties.Settings.Default.LastContextMode = tagContext;
            Properties.Settings.Default.Save();

            BuiltInCategory _bicItemBeingTagged = BuiltInCategory.OST_LightingFixtures;
            BuiltInCategory _bicTagBeing = BuiltInCategory.OST_LightingFixtureTags;

            // Check if task is applicable in this view type
            if (plunkThis.NotInThisView()) { return Result.Succeeded; }

            // first check if families are good
            EnsureTagFamiliesAreLoaded(_doc);

            plunkThis.TagThisLightSwitchFamilyWithThisTag(_FamilyTagName,  // tag family name
                                                           _FamilyTagSymbName,         // tag family symbol name (Type)
                                                           _bicTagBeing,               // builtincategory of the tag
                                                           _bicItemBeingTagged,        // builtincategory of what gets tagged
                                                           tagPref,                    // settings name for fam/type pairing to use
                                                           tagContext);
            return Result.Succeeded;
        }

        // Check if families are good
        private void EnsureTagFamiliesAreLoaded(Autodesk.Revit.DB.Document doc) {
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LFT_UNIVERSAL_NEW", "QTY_TYPE_CIRC_CONTROL", BuiltInCategory.OST_LightingFixtureTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LFT_UNIVERSAL_EXISTING", "QTY_TYPE_CIRC_CONTROL", BuiltInCategory.OST_LightingFixtureTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LDT_SWITCH_UNIVERSAL_NEW", "SWITCH-TYPE-CONTROL", BuiltInCategory.OST_LightingDeviceTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LDT_SWITCH_UNIVERSAL_EXISTING", "SWITCH-TYPE-CONTROL", BuiltInCategory.OST_LightingDeviceTags);
        }

        private void CheckThisFamilyPairing(Autodesk.Revit.DB.Document doc, Type targetType, string familyTagName, string familyTagSymbName, BuiltInCategory bic) {
            Element ConfirmTag = FamilyUtils.FindFamilyType(doc, targetType, familyTagName, familyTagSymbName, bic);
            if (ConfirmTag == null) {
                FamilyUtils.SayMsg("Road Closed", "Unable to resolve loading family " + familyTagName +
                                    " that has a type " + familyTagSymbName +
                                    "Maybe the tool settings are not correct.");
            }
        }

    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickSwitchTag : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            IWin32Window _revit_window;
            _revit_window = new JtWindowHandle(ComponentManager.ApplicationWindow);

            IntPtr revWinIntPtr = ComponentManager.ApplicationWindow;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            string _FamilyTagName = "LDT_SWITCH_UNIVERSAL_NEW";
            string _FamilyTagSymbName = "SWITCH-TYPE-CONTROL";

            string tagContext = "SW";
            string tagPref = "PreferedSWTag";
            PlunkOClass.ChangeToSavedFamTypePairing(tagPref, ref _FamilyTagName, ref _FamilyTagSymbName);
            Properties.Settings.Default.LastContextMode = tagContext;
            Properties.Settings.Default.Save();

            BuiltInCategory _bicItemBeingTagged = BuiltInCategory.OST_LightingFixtures;
            BuiltInCategory _bicTagBeing = BuiltInCategory.OST_LightingDeviceTags;

            // Check if task is applicable in this view type
            if (plunkThis.NotInThisView()) { return Result.Succeeded; }

            // first check if families are good
            EnsureTagFamiliesAreLoaded(_doc);

            plunkThis.TagThisLightSwitchFamilyWithThisTag(_FamilyTagName,  // tag family name
                                                           _FamilyTagSymbName,         // tag family symbol name (Type)
                                                           _bicTagBeing,               // builtincategory of the tag
                                                           _bicItemBeingTagged,        // builtincategory of what gets tagged
                                                           tagPref,                    // settings name for fam/type pairing to use
                                                           tagContext);
            return Result.Succeeded;
        }

        // Check if families are good
        private void EnsureTagFamiliesAreLoaded(Autodesk.Revit.DB.Document doc) {
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LFT_UNIVERSAL_NEW", "QTY_TYPE_CIRC_CONTROL", BuiltInCategory.OST_LightingFixtureTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LFT_UNIVERSAL_EXISTING", "QTY_TYPE_CIRC_CONTROL", BuiltInCategory.OST_LightingFixtureTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LDT_SWITCH_UNIVERSAL_NEW", "SWITCH-TYPE-CONTROL", BuiltInCategory.OST_LightingDeviceTags);
            CheckThisFamilyPairing(doc, typeof(FamilySymbol), "LDT_SWITCH_UNIVERSAL_EXISTING", "SWITCH-TYPE-CONTROL", BuiltInCategory.OST_LightingDeviceTags);
        }

        private void CheckThisFamilyPairing(Autodesk.Revit.DB.Document doc, Type targetType, string familyTagName, string familyTagSymbName, BuiltInCategory bic) {
            Element ConfirmTag = FamilyUtils.FindFamilyType(doc, targetType, familyTagName, familyTagSymbName, bic);
            if (ConfirmTag == null) {
                FamilyUtils.SayMsg("Road Closed", "Unable to resolve loading family " + familyTagName +
                                    " that has a type " + familyTagSymbName +
                                    "Maybe the tool settings are not correct.");
            }
        }

    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickLightRot : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemBeingRot = BuiltInCategory.OST_LightingFixtures;

            plunkThis.TwoPickAimRotateOne(_bicItemBeingRot);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdPickOnlyLights : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory bicItemDesired = BuiltInCategory.OST_LightingFixtures;

            List<ElementId> selIds;
            plunkThis.PickTheseBicsOnly(bicItemDesired, out selIds);
            uidoc.Selection.SetElementIds(selIds);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdLightsNotTagged : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);

            List<ElementId> selIds;

            selIds = plunkThis.notTaggedLights();

            if (selIds != null) { uidoc.Selection.SetElementIds(selIds); }

            return Result.Succeeded;
        }
    }


    [Transaction(TransactionMode.Manual)]
    class CmdPickOnlyDevices : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemDesired = BuiltInCategory.OST_LightingDevices;

            List<ElementId> _selIds;
            plunkThis.PickTheseBicsOnly(_bicItemDesired, out _selIds);
            _uidoc.Selection.SetElementIds(_selIds);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdAimManyLights : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemBeingRot = BuiltInCategory.OST_LightingFixtures;

            List<ElementId> _selIds;
            plunkThis.TwoPickAimRotateMany(_bicItemBeingRot, out _selIds);
            // _uidoc.Selection.SetElementIds(_selIds);
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdMatchAngleLights : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemBeingRot = BuiltInCategory.OST_LightingFixtures;

            List<ElementId> _selIds;
            plunkThis.MatchRotationMany(_bicItemBeingRot, out _selIds);
            // _uidoc.Selection.SetElementIds(_selIds);
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdOpenDocFolder : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            string docFile = System.IO.Path.Combine(AppElecRibbon._app.docsPath, "WTA_ELEC.pdf");
            if (System.IO.File.Exists(docFile)) {
                System.Diagnostics.Process.Start("explorer.exe", docFile);
            } else {
                System.Diagnostics.Process.Start("explorer.exe", AppElecRibbon._app.docsPath);
            }
            return Result.Succeeded;
        }
    }

}

