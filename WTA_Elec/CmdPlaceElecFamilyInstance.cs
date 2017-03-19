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
    //[Transaction(TransactionMode.Manual)]
    //class CmdPlaceTComDrop2DHInstance : IExternalCommand {
    //    public Result Execute(ExternalCommandData commandData,
    //                          ref string message,
    //                          ElementSet elements) {

    //        PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
    //        string wsName = "TCOM";
    //        string FamilyName = "T-COM DROP-WTA";
    //        string FamilySymbolName = "DROP";
    //        string pName = "TCOM - INSTANCE";
    //        string pNameVal = "2D";
    //        string FamilyTagName = "T-COMM TAG - INSTANCE";
    //        string FamilyTagNameSymb = "T-COMM INSTANCE";
    //        bool oneShot = false;
    //        bool hasLeader = false;
    //        BuiltInCategory bicTagBeing = BuiltInCategory.OST_CommunicationDeviceTags;
    //        BuiltInCategory bicFamily = BuiltInCategory.OST_CommunicationDevices;
    //        Element elemPlunked;

    //        plunkThis.PlunkThisFamilyWithThisTagWithThisParameterSet(FamilyName, FamilySymbolName,
    //            pName, pNameVal, wsName, FamilyTagName, FamilyTagNameSymb, bicTagBeing, bicFamily, out elemPlunked, oneShot, hasLeader);

    //        return Result.Succeeded;
    //    }
    //}

    //[Transaction(TransactionMode.Manual)]
    //class CmdPlaceTComDrop4DNHInstance : IExternalCommand {
    //    public Result Execute(ExternalCommandData commandData,
    //                         ref string message,
    //                         ElementSet elements) {

    //        PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
    //        string wsName = "TCOM";
    //        string FamilyName = "T-COM DROP-NH-WTA";
    //        string FamilySymbolName = "DROP";
    //        string pName = "TCOM - INSTANCE";
    //        string pNameVal = "4D";
    //        string FamilyTagName = "T-COMM TAG - INSTANCE";
    //        string FamilyTagNameSymb = "T-COMM INSTANCE";
    //        bool oneShot = false;
    //        bool hasLeader = false;
    //        BuiltInCategory bicTagBeing = BuiltInCategory.OST_CommunicationDeviceTags;
    //        BuiltInCategory bicFamily = BuiltInCategory.OST_CommunicationDevices;
    //        Element elemPlunked;

    //        plunkThis.PlunkThisFamilyWithThisTagWithThisParameterSet(FamilyName, FamilySymbolName,
    //            pName, pNameVal, wsName, FamilyTagName, FamilyTagNameSymb, bicTagBeing, bicFamily, out elemPlunked, oneShot, hasLeader);

    //        return Result.Succeeded;
    //    }
    //}

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
                    + _wsName + " to switch to.","Smells So Bad It Has A Chain On It",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Exclamation);
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickTag : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            string wsName = "MECH HVAC";
            string FamilyTagName = "T-COMM TAG - INSTANCE";
            string FamilyTagSymbName = "T-COMM INSTANCE";
            bool hasLeader = false;
            bool oneShot = false;
            BuiltInCategory bicItemBeingTagged = BuiltInCategory.OST_CommunicationDevices;
            BuiltInCategory bicTagBeing = BuiltInCategory.OST_CommunicationDeviceTags;
            Element elemTagged = null;

            plunkThis.TwoPickTag(wsName, FamilyTagName, FamilyTagSymbName, bicItemBeingTagged, bicTagBeing, hasLeader, oneShot, ref elemTagged);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickLightingTag : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            string _FamilyTagName = "LFT_QUANTITY-TYPE-CIRCUIT-CONTROL";
            string _FamilyTagSymbName = "NEW";
            BuiltInCategory _bicItemBeingTagged = BuiltInCategory.OST_LightingFixtures;
            BuiltInCategory _bicTagBeing = BuiltInCategory.OST_LightingFixtureTags;

            plunkThis.TagThisFamilyWithThisTag(_FamilyTagName, _FamilyTagSymbName,
                                               _bicTagBeing, _bicItemBeingTagged);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdReHostLights : IExternalCommand {

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);

        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {


            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            IWin32Window _revit_window;
            _revit_window = new JtWindowHandle(ComponentManager.ApplicationWindow);


            Element _pickedElemItem = null;
            if (NotInThisView(_doc)) { return Result.Cancelled; }
            ICollection<BuiltInCategory> categoriesA = new[] {
                 BuiltInCategory.OST_LightingFixtures
            };
            ElementFilter myPCatFilter = new ElementMulticategoryFilter(categoriesA);
            ISelectionFilter myPickFilter = SelFilter.GetElementFilter(myPCatFilter);

            //FormMsgWPF formMsg = new FormMsgWPF();
            //formMsg.Show();
            //SetForegroundWindow(ComponentManager.ApplicationWindow.ToInt32());

            // while (true) {
            try {
                //formMsg.SetMsg("Select items to pin/rehost. Press the under the ribbon finish button when done.");
                IList<Reference> pickedElemRefs = _uidoc.Selection.PickObjects(ObjectType.Element, myPickFilter, "Select items to rehost.");
                using (Transaction t = new Transaction(_doc, "Rehosting Many")) {
                    t.Start();
                    foreach (Reference pickedElemRef in pickedElemRefs) {
                        _pickedElemItem = _doc.GetElement(pickedElemRef.ElementId);
                        FamilyInstance fi = _pickedElemItem as FamilyInstance;
                        if (fi.Symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBased) {
                            continue;   // skip over nonhosting lights
                        }
                        fi.Pinned = true;
                        elements.Insert(_pickedElemItem);
                    }  // end foreach
                    t.Commit();
                }  // end using transaction
                //formMsg.Close();

            } catch {
                // Get here when the user hits ESC when prompted for selection
                // "break" exits from the while loop
                //formMsg.Close();
                //throw;
                // break;
            }
            // }  // end while


            //PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            //BuiltInCategory _bicItemBeingRehosted = BuiltInCategory.OST_LightingFixtures;
            //plunkThis.ReHostLightsMany(_bicItemBeingRehosted);
            return Result.Succeeded;
        }


        // returns true if view is not of type for plunking
        private bool NotInThisView(Document _doc) {
            if ((_doc.ActiveView.ViewType != ViewType.CeilingPlan) & (_doc.ActiveView.ViewType != ViewType.FloorPlan)
                & (_doc.ActiveView.ViewType != ViewType.Section) & (_doc.ActiveView.ViewType != ViewType.Elevation)) {
                string msg = ".... In this " + _doc.ActiveView.ViewType.ToString() + " viewtype?";
                FamilyUtils.SayMsg("Huh? Do What?", msg);
                return true;
            }
            return false;
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
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemDesired = BuiltInCategory.OST_LightingFixtures;

            List<ElementId> _selIds;
            plunkThis.PickTheseBicsOnly(_bicItemDesired, out _selIds);
            _uidoc.Selection.SetElementIds(_selIds);

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
    class CmdTwoPickSprinkRot3D : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemBeingRot = BuiltInCategory.OST_Sprinklers;
            string _pNameForAimLine = "Z_RAY_LENGTH";
            List<ElementId> _selIds;
            plunkThis.TwoPickAimRotateOne3D(_bicItemBeingRot, out _selIds, _pNameForAimLine);
            _uidoc.Selection.SetElementIds(_selIds);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class CmdTwoPickSprinkRot3DMany : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            BuiltInCategory _bicItemBeingRot = BuiltInCategory.OST_Sprinklers;
            string _pNameForAimLine = "Z_RAY_LENGTH";
            List<ElementId> _selIds;
            plunkThis.TwoPickAimRotateOne3DMany(_bicItemBeingRot, out _selIds, _pNameForAimLine);
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

    //[Transaction(TransactionMode.Manual)]
    //class CmdMatchParamterForTCOMDropTag : IExternalCommand {
    //    public Result Execute(ExternalCommandData commandData,
    //                          ref string message,
    //                          ElementSet elements) {

    //        PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
    //        string pName = "TCOM - INSTANCE";
    //        BuiltInCategory _bicItemBeingTagged = BuiltInCategory.OST_CommunicationDevices;
    //        BuiltInCategory _bicTagBeing = BuiltInCategory.OST_CommunicationDeviceTags;

    //        plunkThis.MatchParamenterValue(pName, _bicItemBeingTagged, _bicTagBeing);

    //        return Result.Succeeded;
    //    }
    //}

    [Transaction(TransactionMode.Manual)]
    class CmdCycleAirDeviceTypes : IExternalCommand {
        public Result Execute(ExternalCommandData commandData,
                             ref string message,
                             ElementSet elements) {

            UIApplication uiapp = commandData.Application;
            UIDocument _uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application _app = uiapp.Application;
            Autodesk.Revit.DB.Document _doc = _uidoc.Document;

            BuiltInCategory bicFamilyA = BuiltInCategory.OST_DuctTerminal;
            BuiltInCategory bicFamilyB = BuiltInCategory.OST_DataDevices;
            BuiltInCategory bicFamilyC = BuiltInCategory.OST_MechanicalEquipment;
            //BuiltInCategory bicFamilyC = BuiltInCategory.OST_Sprinklers;

            ICollection<BuiltInCategory> categories = new[] { bicFamilyA, bicFamilyB, bicFamilyC };
            ElementFilter myPCatFilter = new ElementMulticategoryFilter(categories);
            ISelectionFilter myPickFilter = SelFilter.GetElementFilter(myPCatFilter);

            bool keepOnTruckn = true;
            FormMsgWPF formMsg = new FormMsgWPF();
            formMsg.Show();

            using (TransactionGroup pickGrp = new TransactionGroup(_doc)) {
                pickGrp.Start("CmdCycleType");
                bool firstTime = true;

                //string strCats= "";
                //foreach (BuiltInCategory iCat in categories) {
                //    strCats = strCats + iCat.ToString().Replace("OST_", "") + ", "; 
                //}
                string strCats = FamilyUtils.BICListMsg(categories);

                formMsg.SetMsg("Pick the " + strCats + " to check its type.", "Type Cycle:");
                while (keepOnTruckn) {
                    try {
                        Reference pickedElemRef = _uidoc.Selection.PickObject(ObjectType.Element, myPickFilter, "Pick the " + bicFamilyA.ToString() + " to cycle its types. (Press ESC to cancel)");
                        Element pickedElem = _doc.GetElement(pickedElemRef.ElementId);

                        FamilyInstance fi = pickedElem as FamilyInstance;
                        FamilySymbol fs = fi.Symbol;

                        var famTypesIds = fs.Family.GetFamilySymbolIds().OrderBy(e => _doc.GetElement(e).Name).ToList();
                        int thisIndx = famTypesIds.FindIndex(e => e == fs.Id);
                        int nextIndx = thisIndx;
                        if (!firstTime) {
                            nextIndx = nextIndx + 1;
                            if (nextIndx >= famTypesIds.Count) {
                                nextIndx = 0;
                            }
                        } else {
                            firstTime = false;
                        }

                        if (pickedElem != null) {
                            using (Transaction tp = new Transaction(_doc, "PlunkOMatic:SetParam")) {
                                tp.Start();
                                if (pickedElem != null) {
                                    fi.Symbol = _doc.GetElement(famTypesIds[nextIndx]) as FamilySymbol;
                                    formMsg.SetMsg("Currently:\n" + fi.Symbol.Name + "\n\nPick again to cycle its types.", "Type Cycling");
                                }
                                tp.Commit();
                            }
                        } else {
                            keepOnTruckn = false;
                        }
                    } catch (Exception) {
                        keepOnTruckn = false;
                        //throw;
                    }
                }
                pickGrp.Assimilate();
            }

            formMsg.Close();
            return Result.Succeeded;
        }
    }

}

