//
// Copyright (c) 2016 AKS
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;


namespace WTA_Elec {
    [Transaction(TransactionMode.Manual)]
    public class CeilingPicker : IExternalCommand {
        #region Implementation of IExternalCommand
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            try {
                bool stay = true;
                while (stay) {
                    CeilingSelectionFilter cf = new CeilingSelectionFilter();
                    /// Unable to pick either an active file ceiling and linked file ceiling with PickObject
                    Reference pickedCeilingRef = sel.PickObject(ObjectType.LinkedElement, cf, "Selecting Linked Ceilings Only");

                    //#region Dealing with non linked picks
                    //Reference pickedCeilingRefNL = sel.PickObject(ObjectType.Element, cf, "Selecting Nonlinked Ceilings Only");
                    //Element firstCeilingElement = doc.GetElement(pickedCeilingRefNL.ElementId);  
                    //#endregion

                    #region Dealing with Linked picks
                    if (pickedCeilingRef == null) return Result.Failed;
                    // we need to get the linked document and then get the element that was picked from the LinkedElementId
                    RevitLinkInstance linkInstance = doc.GetElement(pickedCeilingRef) as RevitLinkInstance;
                    Document linkedDoc = linkInstance.GetLinkDocument();
                    Element firstCeilingElement = linkedDoc.GetElement(pickedCeilingRef.LinkedElementId);
                    #endregion

                    string daRmName = "";
                    string daHT = "";
                    string daLV = "";
                    string daPhsCreated = "";
                    string daPhsDemo = "";

                    switch (firstCeilingElement.GetType().ToString()) {
                        case "Autodesk.Revit.DB.Architecture.Room":
                            Room thisPickRm = firstCeilingElement as Room;
                            if (thisPickRm != null) {
                                daRmName = thisPickRm.Name.ToString();
                                Phase phCR = linkedDoc.GetElement(thisPickRm.CreatedPhaseId) as Phase;
                                if (phCR != null) { daPhsCreated = phCR.ToString(); }
                                Level itsLevelRm = thisPickRm.Level;
                                if (itsLevelRm != null) { daLV = itsLevelRm.Name.ToString(); }
                            }
                            break;
                        case "Autodesk.Revit.DB.Ceiling":
                            Ceiling thisPickCl = firstCeilingElement as Ceiling;
                            Parameter itsRoomparam = thisPickCl.get_Parameter(BuiltInParameter.ROOM_NAME);
                            if (itsRoomparam != null) { daRmName = itsRoomparam.AsValueString(); }
                            Parameter daHTparam = thisPickCl.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM);
                            if (daHTparam != null) { daHT = daHTparam.AsValueString(); }
                            Parameter itsLevelCl = thisPickCl.get_Parameter(BuiltInParameter.LEVEL_PARAM);
                            if (itsLevelCl != null) { daLV = itsLevelCl.AsValueString(); }
                            Parameter whenCreated = thisPickCl.get_Parameter(BuiltInParameter.PHASE_CREATED);
                            if (whenCreated != null) { daPhsCreated = whenCreated.AsValueString(); }
                            break;
                        default:
                            break;
                    }

                    TaskDialog thisDialog = new TaskDialog("Ceiling Pick-O-Matic");
                    thisDialog.TitleAutoPrefix = false;
                    thisDialog.MainIcon = TaskDialogIcon.TaskDialogIconNone;
                    thisDialog.CommonButtons = TaskDialogCommonButtons.Close | TaskDialogCommonButtons.Retry;
                    thisDialog.DefaultButton = TaskDialogResult.Retry;
                    thisDialog.FooterText = "Hitting Escape allows picking again.";

                    //TaskDialog.Show("Ceiling Picker Says",
                    //                 firstCeilingElement.Category.Name + "\n" + firstCeilingElement.Name + "\n" +
                    //                 daHT);
                    string msg = firstCeilingElement.Name + "\n" + daHT + " from " + daLV;
                    msg = msg + "\n" + daPhsCreated + " that is " + daPhsDemo + "\n" + "Room Name: " + daRmName;

                    thisDialog.MainInstruction = msg;
                    thisDialog.MainContent = "";
                    TaskDialogResult tResult = thisDialog.Show();

                    if (TaskDialogResult.Close == tResult) {
                        stay = false;
                    }
                }
                return Result.Succeeded;
            } catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                //TaskDialog.Show("Cancelled", "User cancelled");
                return Result.Cancelled;
            }
                //Catch other errors
            catch (Exception ex) {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }
        #endregion

        /// <summary>
        /// This is the selection filter class
        /// </summary>
        /// The problem here is that PickObject takes either an ObjectType.Element argument
        /// or a ObjectType.LinkedElement, but not both. We cannot select ceilings from the
        /// active file and a linked file at once.
        public class CeilingSelectionFilter : ISelectionFilter {
            private RevitLinkInstance thisInstance = null;
            // If the calling document is needed then we'll use this to pass it to this class.
            //Document doc = null;
            //public CeilingSelectionFilter(Document document)
            //{
            //    doc = document;
            //}

            /// During the selection process, when the cursor is hovering over an element,
            /// this element will be passed into the AllowElement() method. The AllowElement()
            /// method allows you to examine the element and determine whether it should be 
            /// highlighted or not. If you return true from this method, the element can be
            /// highlighted and selected. If you return false, the element can be neither
            /// highlighted nor selected.
            public bool AllowElement(Element e) {
                // if the element is a non linked file ceiling
                if (e.GetType() == typeof(Ceiling)) { return true; }

                /// Accept any link instance, and save the handle for use in AllowReference()
                /// because the LinkedElementId needs to be used and it requires a reference
                /// which is available only in the AllowReference section.
                thisInstance = e as RevitLinkInstance;
                return (thisInstance != null);
            }

            ///  During the selection process, if the cursor is hovering over a reference,
            ///  this reference will be passed into the AllowReference() method.
            ///  If you return true from this method, then the reference can be highlighted
            ///  and selected. If you return false, the reference can be neither highlighted
            ///  nor selected. 
            public bool AllowReference(Reference refer, XYZ point) {
                // Back in AllowElement 'thisInstance' was assigned.  

                if (thisInstance == null) { return false; }
                ////// Get the handle to the element in the link
                Document linkedDoc = thisInstance.GetLinkDocument();
                Element elem = linkedDoc.GetElement(refer.LinkedElementId);
                if (elem.GetType() == typeof(Ceiling)) { return true; }
                return false;
            }
        }//end CeilingSelectionFilter class

    }


    [Transaction(TransactionMode.Manual)]
    public class CmdRoomsLightPwrDensityReport : IExternalCommand {
        #region Implementation of IExternalCommand
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            try {
                LightingPowerDensityTotalizer lf = new LightingPowerDensityTotalizer();
                lf.StartLightFinding(uiDoc.Application);
            } catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                //TaskDialog.Show("Cancelled", "User cancelled");
                return Result.Cancelled;
            }
                //Catch other errors
                catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.Message + "\n" + ex.ToString(), "Error At CmdRoomsLightPwrDensityReport");
                return Result.Failed;
            }

        #endregion
            return Result.Succeeded;
        }

    }
    
    [Transaction(TransactionMode.Manual)]
    public class CmdRoomLightingReporter : IExternalCommand {
        #region Implementation of IExternalCommand
        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements) {

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            List<ElementId> selIds = new List<ElementId>();
            Room thisPickRm = null;
            string pNamePWR = "INPUT POWER";

            PlunkOClass plunkThis = new PlunkOClass(commandData.Application);
            if (plunkThis.NotInThisView()) { return Result.Succeeded; }

            // phase basis is the phase on which element status is determined for inclusion
            Phase phsBasis = FamilyUtils.GetDesiredPhaseBasis(doc);

            FormMsgWPF thisReport = new FormMsgWPF();
            string bot = "( ESC key cancels )";
            string purpose = "Room Lighting - Phase Basis: " + phsBasis.Name;
            bool stay = true;
            bool doWeSelect = false;

            try {
                while (stay) {
                    RoomSelectionFilter cf = new RoomSelectionFilter();
                    Reference pickedRoomReference = sel.PickObject(ObjectType.LinkedElement, cf, "Selecting Rooms Only");
                    if (pickedRoomReference == null) return Result.Failed;
                    // we need to get the linked document and then get the element that was picked from the LinkedElementId
                    RevitLinkInstance linkInstance = doc.GetElement(pickedRoomReference) as RevitLinkInstance;
                    Document linkedDoc = linkInstance.GetLinkDocument();
                    Element firstRoomElement = linkedDoc.GetElement(pickedRoomReference.LinkedElementId);

                    string selRmName = "";
                    string selLV = "";
                    string sePhsCreated = "";
                    // string daPhsDemo = "";
                    string selRmNumber = "";
                    double selRmArea = 0.0;

                    switch (firstRoomElement.GetType().ToString()) {
                        case "Autodesk.Revit.DB.Architecture.Room":
                            thisPickRm = firstRoomElement as Room;
                            if (thisPickRm != null) {
                                selRmName = thisPickRm.Name.ToString();
                                selRmNumber = thisPickRm.Number.ToString();
                                Phase phCR = linkedDoc.GetElement(thisPickRm.CreatedPhaseId) as Phase;
                                if (phCR != null) { sePhsCreated = phCR.ToString(); }
                                Level itsLevelRm = thisPickRm.Level;
                                if (itsLevelRm != null) {
                                    selLV = itsLevelRm.Name.ToString();
                                    selRmArea = thisPickRm.Area;
                                    //selIds.Add(thisPickRm.Id);  // does not work. A known Revit fact, cannot highlight linked elements
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    string msgMain = firstRoomElement.Name;
                    msgMain = msgMain
                        + "\n" + "Rm Numb.: " + selRmNumber
                        + "\n" + "At level: " + selLV
                        ;

                    // Get all LightingFixtures in the current room
                    BuiltInCategory bic = BuiltInCategory.OST_LightingFixtures;
                    List<FamilyInstance> famInstances = FamilyUtils.FamilyInstanceCategoryInThisRoom(thisPickRm, bic, doc, phsBasis);
                    Dictionary<string, int> dicFamInstances = new Dictionary<string, int>();
                    Dictionary<string, double> dicLightFixTypeIP = new Dictionary<string, double>();
                    if (famInstances != null) {
                        if (doWeSelect) { selIds.Clear(); }
                        foreach (FamilyInstance fi in famInstances) {
                            string fiNamePair = fi.Symbol.FamilyName + " | " + fi.Symbol.Name;
                            //msg = msg + "\n" + fiNamePair;
                            int qty;
                            if (dicFamInstances.TryGetValue(fiNamePair, out qty)) {
                                dicFamInstances[fiNamePair] = qty + 1;
                            } else {
                                dicFamInstances.Add(fiNamePair, 1);
                                Parameter pPwr = fi.Symbol.LookupParameter(pNamePWR);
                                if (pPwr != null) {
                                    double convVal = FamilyUtils.ConvertParmValueFromRaw(pPwr.AsDouble());
                                    dicLightFixTypeIP.Add(fiNamePair, convVal);
                                    //System.Windows.Forms.MessageBox.Show(fi.Symbol.Name + "  " + convVal.ToString());
                                }
                            }
                            if (doWeSelect) { selIds.Add(fi.Id); }
                        }
                    }

                    string msgDetail = "";
                    double totRoomLWatts = 0.0;
                    foreach (var item in dicFamInstances) {
                        string itemName = item.Key;
                        int itemCount = item.Value;
                        double itemWatts;
                        if (dicLightFixTypeIP.TryGetValue(item.Key, out itemWatts)) {
                            double totalWattsForItem = itemWatts * Convert.ToDouble(itemCount);
                            totRoomLWatts = totRoomLWatts + totalWattsForItem;
                            msgDetail = msgDetail + "\n" + itemName + "  cnt " +
                                        itemCount.ToString() + " @ " +
                                        itemWatts.ToString("0.00") + " for " +
                                        totalWattsForItem.ToString("0.0 w"); 
                        } else {
                            /// item key not in dictionary!
                        }
                    }
                    double lightFixPWRDensity = totRoomLWatts / selRmArea;
                    msgMain = msgMain
                           + "\n" + "Rm Area: " + selRmArea.ToString("0.00 sf") + "  L Pwr. Density: " + lightFixPWRDensity.ToString("0.00 w/sf") + "  Tot: " + totRoomLWatts.ToString("0.00 w")
                           + "\n"
                           + msgDetail;

                    thisReport.SetMsg(msgMain, purpose, bot, true);
                    thisReport.Show();

                }

            } catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                //TaskDialog.Show("Cancelled", "User cancelled");
                stay = false;
                thisReport.Close();
                uiDoc.Selection.SetElementIds(selIds);
                return Result.Cancelled;
            }
                //Catch other errors
            catch (Exception ex) {
                thisReport.Close();
                FamilyUtils.SayMsg("Error At RoomLightingReporter", ex.Message);
                return Result.Failed;
            }

            if (doWeSelect) { uiDoc.Selection.SetElementIds(selIds); }
            return Result.Succeeded;
        }
        #endregion

        //This is the selection filter class
        public class CeilingSelectionFilter : ISelectionFilter {
            private RevitLinkInstance thisInstance = null;
            // If the calling document is needed then we'll use this to pass it to this class.
            //Document doc = null;
            //public CeilingSelectionFilter(Document document)
            //{
            //    doc = document;
            //}

            public bool AllowElement(Element e) {
                if (e.GetType() == typeof(Ceiling)) { return true; }
                // Accept any link instance, and save the handle for use in AllowReference()
                thisInstance = e as RevitLinkInstance;
                return (thisInstance != null);
            }

            public bool AllowReference(Reference refer, XYZ point) {
                if (thisInstance == null) { return false; }
                //// Get the handle to the element in the link
                Document linkedDoc = thisInstance.GetLinkDocument();
                Element elem = linkedDoc.GetElement(refer.LinkedElementId);
                if (elem.GetType() == typeof(Ceiling)) { return true; }
                return false;
            }
        }//end class

        public class RoomSelectionFilter : ISelectionFilter {
            private RevitLinkInstance thisInstance = null;
            // If the calling document is needed then we'll use this to pass it to this class.
            //Document doc = null;
            //public CeilingSelectionFilter(Document document)
            //{
            //    doc = document;
            //}

            public bool AllowElement(Element e) {
                if (e.GetType() == typeof(Room)) { return true; }
                if (e.GetType() == typeof(SpatialElement)) {
                    Room r = e as Room;
                    if (null != r) {
                        return true;
                    }
                }
                // Accept any link instance, and save the handle for use in AllowReference()
                thisInstance = e as RevitLinkInstance;
                return (thisInstance != null);
            }

            public bool AllowReference(Reference refer, XYZ point) {
                if (thisInstance == null) { return false; }
                //// Get the handle to the element in the link
                Document linkedDoc = thisInstance.GetLinkDocument();
                Element elem = linkedDoc.GetElement(refer.LinkedElementId);
                if (elem.GetType() == typeof(Room)) { return true; }
                if (elem.GetType() == typeof(SpatialElement)) { return true; }
                return false;
            }
        }//end class

    } // end class RoomPicker


}
