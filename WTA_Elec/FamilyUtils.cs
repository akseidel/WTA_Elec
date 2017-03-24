using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.DB.Events;
using ComponentManager = Autodesk.Windows.ComponentManager;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Keys = System.Windows.Forms.Keys;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;


namespace WTA_Elec {
    class FamilyUtils {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int W, int H, uint uFlags);

        // find an element of the given type, name, and category(optional)
        public static Element FindFamilyType(Autodesk.Revit.DB.Document rvtDoc,
                                             Type targetType,
                                             string targetFamilyName,
                                             string targetTypeName,
                                             Nullable<BuiltInCategory> targetCategory) {

            // Narrow down to elements of the given type and category
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            // the optional argument
            if (targetCategory.HasValue) {
                collector.OfCategory(targetCategory.Value);
            }
            // Using LINQ query extract for family name and family type
            var targetElems =
                from element in collector
                where element.Name.Equals(targetTypeName) &&
                element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(targetFamilyName)
                select element;
            // put result as list of element for accessing
            IList<Element> elems = targetElems.ToList();
            if (elems.Count > 0) {
                // Done, exit with the desired element.
                return elems.FirstOrDefault();
            }

            // Attempt at this point to find and load the family. Then check if it has the type needed.
            FormMsgWPF formMsgWPF = new FormMsgWPF();
            string msg = "Family Load Needed For: " + targetFamilyName + " having a type: " + targetTypeName;
            formMsgWPF.SetMsg(msg, "Attempting To Find On Network", " ");
            formMsgWPF.Show();
            List<string> candidates = FindFamilyCandidates(rvtDoc, targetFamilyName);
            formMsgWPF.Close();

            string foundFamPath = candidates.FirstOrDefault();
            if (foundFamPath != null) {
                // sometimes we have a transaction already going on.
                if (rvtDoc.IsModifiable) {
                    rvtDoc.LoadFamily(foundFamPath);
                } else {
                    using (Transaction tx = new Transaction(rvtDoc)) {
                        tx.Start("Load " + targetFamilyName);
                        rvtDoc.LoadFamily(foundFamPath);
                        tx.Commit();
                    }
                }
                // check again for family and type
                var collector2 = new FilteredElementCollector(rvtDoc).OfClass(targetType);
                // the optional argument
                if (targetCategory.HasValue) {
                    collector2.OfCategory(targetCategory.Value);
                }
                // Using LINQ query extract for family name and family type
                var targetElems2 =
                    from element in collector
                    where element.Name.Equals(targetTypeName) &&
                    element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(targetFamilyName)
                    select element;
                // put result as list of element for accessing
                IList<Element> elems2 = targetElems.ToList();
                formMsgWPF.Close();
                if (elems2.Count > 0) {
                    // Done, exit with the desired element.
                    return elems2.FirstOrDefault();
                } else {
                    SayMsg("Found a family, but it is not right.", "It is either not a " +
                           targetCategory.ToString().Replace("OST_", "") + " or\n"
                           + "it does not having a type:\n" + targetTypeName + "\n\nYou are not standing in tall cotton.");
                }
            } else {
                // At this point no path discovered for the desired Family name. If the desired family
                // does exist somewhere then have a chance to load it.

                SayMsg("Item To Place Not Found  -  Family Load Needed For:",
                    targetCategory.ToString().Replace("OST_", "") + " family:\n"
                       + targetFamilyName + "\n"
                       + "having a type:\n" + targetTypeName + "\n\nMaybe you can find it.");

                SayMsg("Go Find " + targetFamilyName, "READ ALL OF THIS BEFORE DOING ANYTHING. After closing this message, drag that file anywhere into the Revit Project Browser view."
                 + " Make sure not to drop it on the Properties view. That will open the dragged family in the Family Editor. You will be quite confused."
                 + " That missing family will also not have been added to the project.");

            }// end fondFamPath
            formMsgWPF.Close();
            return null;
        }

        static List<string> FindFamilyCandidates(Autodesk.Revit.DB.Document rvtDoc, string targetFamilyName) {
            List<string> candidates = new List<string>();
            string fileToFind = targetFamilyName + ".rfa";
            string RootSearchPath = Properties.Settings.Default.RootSearchPath;
            string sDir = "N:\\CAD\\BDS PRM " + rvtDoc.Application.VersionNumber + "\\" + RootSearchPath;
            DirSearch(sDir, fileToFind, ref candidates);
            return candidates;
        }

        /// Return the family symbol found in the given document
        /// matching the given built-in category, or null if none is found.
        /// </summary>
        static FamilySymbol GetThisFamilySymbol(Autodesk.Revit.DB.Document doc, BuiltInCategory bic, string SymbName) {
            FamilySymbol s = GetFamilySymbols(doc, bic)
              .Where(t => t.Name == SymbName)
              .FirstOrDefault() as FamilySymbol;
            return s;
        }

        static FilteredElementCollector
          GetFamilySymbols(Autodesk.Revit.DB.Document doc, BuiltInCategory bic) {
            return GetElementsOfType(doc, typeof(FamilySymbol), bic);
        }

        public static FilteredElementCollector
         GetElementsOfType(Autodesk.Revit.DB.Document doc, Type type, BuiltInCategory bic) {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(bic);
            collector.OfClass(type);
            return collector;
        }

        public static Dictionary<string, List<FamilySymbol>> FindFamilyTypes(Autodesk.Revit.DB.Document doc,
                                                                             BuiltInCategory cat) {
            return new FilteredElementCollector(doc)
                    .WherePasses(new ElementClassFilter(typeof(FamilySymbol)))
                    .WherePasses(new ElementCategoryFilter(cat))
                    .Cast<FamilySymbol>()
                    .GroupBy(e => e.Family.Name)
                    .ToDictionary(e => e.Key, e => e.ToList());
        }

        // Returns the workset name for the workset id thiswid
        public static string WhatWorksetNameIsThis(WorksetId thiswid, Document doc) {
            if (thiswid == null) {
                return String.Empty;
            }
            // Find all user worksets 
            FilteredWorksetCollector worksets
                = new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset);
            foreach (Workset ws in worksets) {
                if (thiswid == ws.Id) {
                    return ws.Name.ToString();
                }
            }
            return String.Empty;
        }

        public static WorksetId WhatIsThisWorkSetIDByName(string wsName, Document doc) {
            if (wsName == null) {
                return null;
            }
            // Find all user worksets 
            FilteredWorksetCollector worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
            foreach (Workset ws in worksets) {
                if (wsName == ws.Name) {
                    return ws.Id;
                }
            }
            return null;
        }

        public static void DirSearch(string sDir, string fileToFind, ref List<string> filesFound) {
            if (!Directory.Exists(sDir)) {
                FormMsgWPF noway = new FormMsgWPF(0,true, true);
                noway.SetMsg("Searching is impossible because the path\n'" + sDir + "'\ndoes not exist.", "Mission Is Impossible");
                noway.ShowDialog();
                // System.Windows.MessageBox.Show("No Way");
                return;
            }
            try {
                foreach (string d in Directory.GetDirectories(sDir)) {
                    foreach (string f in Directory.GetFiles(d, fileToFind)) {
                        //lstFilesFound.Items.Add(f);
                        filesFound.Add(f);
                    }
                    DirSearch(d, fileToFind, ref filesFound);
                }
            } catch (System.Exception excpt) {
                SayMsg("Error at DirSearch", excpt.Message);
                throw;
            }
        }

        public static void SayMsg(string _title, string _msg) {

            TaskDialog thisDialog = new TaskDialog(_title);
            thisDialog.TitleAutoPrefix = false;
            thisDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            thisDialog.MainInstruction = _msg;
            thisDialog.MainContent = "";
            TaskDialogResult tResult = thisDialog.Show();

            //FormMsgWPF NITV = new FormMsgWPF(0, true, true);
            //NITV.SetMsg(_msg, _title);
            //NITV.ShowDialog();

        }

        public static string BICListMsg(ICollection<BuiltInCategory> _bicCats) {
            string strCats = "";
            foreach (BuiltInCategory iCat in _bicCats) {
                strCats = strCats + iCat.ToString().Replace("OST_", "") + ", ";
            }
            return strCats;
        }

        public static List<FamilyInstance> FamilyInstanceCategoryInThisRoom(Room _room, BuiltInCategory bic, Document _Doc, Phase phsBasis) {
            ProjectLocation projectLocation = _Doc.ActiveProjectLocation;
            XYZ origin = new XYZ(0, 0, 0);
            ProjectPosition position = projectLocation.get_ProjectPosition(origin);
            Double zOffset = position.Elevation; // will correct all z values with this value
            zOffset = 0;  // leaving it this way for the time being 12/6/16
            bool debugMsg = false;
            if (debugMsg) { TaskDialog.Show("Have position.Elevation", zOffset.ToString()); }
            
            #region Explanation
            // Typical code examples use the familyintance boundingbox values for passing a point to the Revit
            // Room IsPointInRoom function.
            // The problem here is that Revit's IsPointInRoom function operates on the "Room's" limits.
            // The room's upper limit might actually be set lower than the ceiling or where the familyinstance
            // might be. So instead we shall set the family instance's z value to 1 foot, taking the 
            // chance that the room Base Offset is not higher than 1 foot. 
            // At this point the Room IsPointInRoom function will consider all familyinstances that are vertically within
            // the room's footprint to be in the room. The tactic here is to guess the elevation of the next floor level
            // above the room and then use that to make sure only those family instances within the floor to floor range
            // are passed to the Room IsPointInRoom function.
            #endregion

            //TaskDialog.Show("Lookin for next level", "Lookin for next level " + _room.Name);

            // Together, the Upper Limit and Limit Offset parameters define the
            // upper boundary of the room. Often the limitoffset is negative.
            Double roomElev = _room.Level.Elevation + zOffset;
            Double roomElevTop = _room.Level.Elevation + zOffset + 7.5; // call this default room height
            Double aSmallDist = 0.1;
            // When the UpperLimit is then next floor level and the LimitOffset happens to be zero
            // then we need to subtract aSmallDist so that the next level algoritim finds the UpperLimit
            // floor level as the next level and not the next level higher.
            if (_room.UpperLimit != null) {
                roomElevTop = _room.UpperLimit.Elevation + zOffset + _room.LimitOffset - aSmallDist;
            } else {
                // go with default - for some reason there can be nulls
            }
            if (debugMsg) {
                string strDBMsg = "Room Level Name=" + _room.Level.Name + "\nRoom Elev=" + roomElev.ToString();
                if (_room.UpperLimit != null) {
                    strDBMsg = strDBMsg + "\n" + "Room Upper Limit Elev=" + _room.UpperLimit.Elevation.ToString();
                }
                strDBMsg = strDBMsg + "\n" + "Room LimitOffset=" + _room.LimitOffset.ToString();
                strDBMsg = strDBMsg + "\n" + "Calc Room Top Elev=" + roomElevTop.ToString();
                System.Windows.MessageBox.Show(strDBMsg,"Data About The Room Object");
              }

            Double topRangeElevation = roomElevTop;
            if (debugMsg) { TaskDialog.Show("Have elevations", "TopRangeElev " + roomElevTop.ToString() + "\nRoomElev " + roomElev.ToString()); }

            #region Guess The Next Higher Level
            // given an elevation, get the elevation for the next higher level elevation
            FilteredElementCollector collector = new FilteredElementCollector(_Doc);
            ICollection<Element> collection = collector.OfClass(typeof(Level)).ToElements();
            Level nextHigherLevel = null;
            Double minDelta = 1000.0;

            string strDebugLMsg = "";
            foreach (Element e in collection) {
                Level level = e as Level;
                if (null != level) {
                    if (debugMsg) {
                        strDebugLMsg = strDebugLMsg + "Level Name=" + level.Name + "\nLevel Elev=" + level.Elevation.ToString() + "\n\n";
                    }
                    if (level.Elevation > roomElevTop) {
                        double delta = level.Elevation - roomElevTop;
                        if (delta < minDelta) {
                            minDelta = delta;
                            nextHigherLevel = level;
                        }
                    }
                }
            }
            if (debugMsg) {System.Windows.MessageBox.Show(strDebugLMsg, "Levels in Doc =" + Path.GetFileName(_Doc.PathName));}
            #endregion

            if (null != nextHigherLevel) {
                topRangeElevation = nextHigherLevel.Elevation;
                if (debugMsg) { System.Windows.MessageBox.Show("Next higher Level above room's LimitOffset \nName =" + nextHigherLevel.Name.ToString() + "\nelev =" + topRangeElevation.ToString(),"Next higher Level Results" ); }
            }

            //TaskDialog.Show("have next level" , "have next level " + _room.Name);

            List<FamilyInstance> familyInstancesLST = new List<FamilyInstance>();
            try {
                // Create a LightingFixture/FamilyIntance collection that exist in the active document
                FilteredElementCollector filteredElemCol = new FilteredElementCollector(_Doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .OfCategory(bic)
                .OfClass(typeof(FamilyInstance));

                // Iterate through each instance
                bool warnMSGNotSeenYet = true;
                foreach (FamilyInstance fi in filteredElemCol) {
                    try {
                        // Get the bounding box of the instance
                        BoundingBoxXYZ boundingBoxXYZ = fi.get_BoundingBox(null);
                        if (boundingBoxXYZ == null) { continue; }
                        Double z = 0.0;
                        z = _room.Level.Elevation + 1.0 + zOffset;

                        LocationPoint fi_LocPt = fi.Location as LocationPoint;
                        Double actual_Z = fi_LocPt.Point.Z + zOffset;

                        //TaskDialog.Show("Debug", "The room Base Offset: " + _room.BaseOffset.ToString() + "\nRoomLevelBasisZ: " + _room.Level.Elevation.ToString());

                        if (_room.BaseOffset > 1.0) {
                            if (warnMSGNotSeenYet) {
                                TaskDialog.Show("Warning", "The room Base Offset is set higher than 1 foot. Some lights will not be counted.");
                                warnMSGNotSeenYet = false;
                            }
                        }

                        // Get the center point of the instance (except Z)
                        XYZ centerPT = new XYZ((boundingBoxXYZ.Min.X + boundingBoxXYZ.Max.X) / 2,
                                               (boundingBoxXYZ.Min.Y + boundingBoxXYZ.Max.Y) / 2,
                                               (z));

                        // exclude fi element when it is not present for the phase basis
                        ElementOnPhaseStatus elPhs = fi.GetPhaseStatus(phsBasis.Id);
                        if (elPhs == ElementOnPhaseStatus.Past ||
                            elPhs == ElementOnPhaseStatus.Demolished ||
                            elPhs == ElementOnPhaseStatus.Temporary ||
                            elPhs == ElementOnPhaseStatus.Future) { continue; }
                        // include only items having actual_Z within this floor level range
                        // _room.Level.Elevation >= z <= topRangeElevation 
                        if (actual_Z >= (_room.Level.Elevation + zOffset) && actual_Z <= (topRangeElevation + zOffset)) {
                            //Determine if the point exists within the bounding box/room
                            if (_room.IsPointInRoom(centerPT) == true) {
                                familyInstancesLST.Add(fi);
                            }
                        }
                    } catch (Exception ex) {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                        // continue processing
                    }
                }
                //TaskDialog.Show("done with", "done with " + _room.Name);
                return familyInstancesLST;

            } catch {
                return familyInstancesLST = new List<FamilyInstance>();
            }
        }

        // for the time being, use this
        public static Phase GetDesiredPhaseBasis(Document doc) {
            int topPhaseIndx = doc.Phases.Size - 1;
            Phase phsBasis = doc.Phases.get_Item(0);
            //System.Windows.Forms.MessageBox.Show("_Doc.Phases.Size " + _Doc.Phases.Size.ToString());
            if (doc.Phases.Size == 1) {
                phsBasis = doc.Phases.get_Item(0);
            }
            if (doc.Phases.Size == 2) {
                phsBasis = doc.Phases.get_Item(1);
            }
            if (doc.Phases.Size > 2) {
                phsBasis = doc.Phases.get_Item(topPhaseIndx);
            }
            return phsBasis;
        }

        public static Room DetermineRoom(Element el) {
            FamilyInstance fi = el as FamilyInstance;
            if (fi == null) { return null; }
            // As simple as that?

            try {
                if (fi.Room != null) {
                    //Debug.WriteLine("fi.Room != null");
                    return fi.Room;
                }
            } catch {
            }

            // Try phasing
            Room r = null;
            foreach (Phase p in el.Document.Phases) {
                try {
                    // TODO should check fi.GetPhaseStatus 
                    // instead of provoking an exception

                    ElementOnPhaseStatus eops = fi.GetPhaseStatus(p.Id);  // now what???
                    r = fi.get_Room(p);
                    if (r != null) {
                        //Debug.WriteLine("fi.get_Room( " + p.Name + ") != null");
                        return r;
                    }
                } catch {
                }
            }

            LocationPoint lp = el.Location as LocationPoint;

            if (lp != null) {
                // Try design options
                //List<Element> roomlst = get_Elements( el.Document, typeof(Room));
                List<Element> roomlst = new FilteredElementCollector(el.Document).OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
                // Try rooms from primary design option
                foreach (Element roomel in roomlst) {
                    Room priroom = roomel as Room;
                    if (priroom == null) { continue; }
                    if (priroom.DesignOption == null) { continue; }
                    if (priroom.DesignOption.IsPrimary) {
                        // TODO should check whether priroom 
                        // and el phasing overlaps
                        if (priroom.IsPointInRoom(lp.Point)) {
                            //Debug.WriteLine( "priroom.IsPointInRoom != null");
                            return priroom;
                        }
                    }
                }

                // Emergency: try any room
                foreach (Element roomel in roomlst) {
                    Room room = roomel as Room;
                    if (room == null) { continue; }
                    // TODO should check whether room 
                    // and el phasing overlaps
                    if (room.IsPointInRoom(lp.Point)) {
                        //Debug.WriteLine("room.IsPointInRoom != null");
                        return room;
                    }
                }
            }
            // Nothing found
            //System.Windows.Forms.MessageBox.Show("Nothing found" );
            return null;
        }

        public static ICollection<ElementId> GetTypeBICInPhase(Document doc, Type thisType, BuiltInCategory bic, Phase phase) {
            // http://spiderinnet.typepad.com/blog/2011/07/
            // elementparameterfilter-using-filterelementidrule-to-filter-element-parameters-in-c.html
            //
            // • An ElementParameterFilter needs a filter rule, the FilterElementIdRule in this case.
            // • The FilterElementIdRule needs a parameter value provider (ParameterValueProvider) and 
            // a filter rule evaluator (FilterStringRuleEvaluator), specifically the FilterNumericEquals here.
            // • Do not feel surprised that the FilterNumericEquals evaluator also works with the FilterElementIdRule 
            // as the ElementId is effectively nothing more than an integer value. 
            // • The ParameterValueProvider needs an argument of parameter, as the phase created parameter
            // BuiltInParameter.PHASE_CREATED in this case.
            // • The parameter is represented by an ElementId, which is the numeric value of the specified BuiltInParameter. 
            // • A fast filter, ElementClassFilter, represented by its shortcut method (OfClass), is also used to
            // narrow down the FilteredElementCollector first. It not only speeds up the search but also makes sure only walls are returned.

            ParameterValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterElementIdRule rule1 = new FilterElementIdRule(provider, new FilterNumericEquals(), phase.Id);
            ElementParameterFilter filter1 = new ElementParameterFilter(rule1);

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(thisType);
            collector.OfCategory(bic);
            collector.WherePasses(filter1);
            return collector.ToElementIds();
        }

        // many Revit parameter values like power are stored in raw in metric
        // squares.
        public static double ConvertParmValueFromRaw(double rawParamVal) {
            double feet2Meter = 0.3048;
            return rawParamVal * feet2Meter * feet2Meter;
        }

    } // end class
} // end namespace
