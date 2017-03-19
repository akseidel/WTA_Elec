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
            // first , narrow down to elements of the given type and category
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            // the optional argument
            if (targetCategory.HasValue) {
                collector.OfCategory(targetCategory.Value);
            }
            //TaskDialog.Show("MSG", targetType.ToString());
            //TaskDialog.Show("MSG", targetFamilyName);
            //TaskDialog.Show("MSG", targetTypeName);
            //TaskDialog.Show("MSG", targetCategory.ToString());
            // parse the collection for the given names using LINQ query
            var targetElems =
                from element in collector
                where element.Name.Equals(targetTypeName) &&
                element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(targetFamilyName)
                select element;
            // put result as list of element for accessing
            IList<Element> elems = targetElems.ToList();
            if (elems.Count > 0) {
                //SayMsg("Found Family", targetFamilyName + "\n" + targetTypeName);
                return elems[0];
            }
            SayMsg("Item To Plunk Not Found  -  Family Load Needed For:",
                targetCategory.ToString().Replace("OST_", "") + " family:\n"
                   + targetFamilyName + "\n"
                   + "having a type:\n" + targetTypeName + "\n\nOk, lets try to find it.");

            string fileToFind = targetFamilyName + ".rfa";
            string sDir = "N:\\CAD\\BDS PRM " + rvtDoc.Application.VersionNumber + "\\WTA Families";
            List<string> candidates = new List<string>();
            DirSearch(sDir, fileToFind, ref candidates);
            if (candidates.Count > 0) {
                // set Revit to be topmost temporairly
                SetWindowPos(ComponentManager.ApplicationWindow, new IntPtr(-1), -1, -1, -1, -1, 1);
                string args = string.Format("/Select, \"{0}\"", candidates[0]);
                // explorer will open but no tbe on top of Revit's Taskdialog
                System.Diagnostics.Process.Start("explorer.exe", args);
                string cand = null;
                foreach (string c in candidates) {
                    cand = cand + "\n" + c;
                }
                SayMsg("Found These Candidiates (Probably Just One)", cand + "\n\nExplorer is open at the folder holding the first candidate file. "
                                           + "It is the selected file.\n\n"
                                           + "READ ALL OF THIS BEFORE DOING ANYTHING. After closing this message, drag that file anywhere into the Revit Project Browser view."
                       + " Make sure not to drop it on the Properties view. That will open the dragged family in the Family Editor. You will be quite confused."
                       + " That missing family will also not have been added to the project.");
            } else {
                SayMsg("No Candidiates Found", "You are on your own now.");
            }
            // set Revit back to not topmost
            SetWindowPos(ComponentManager.ApplicationWindow, new IntPtr(-2), -1, -1, -1, -1, 1);
            return null;
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

        static FilteredElementCollector
         GetElementsOfType(Autodesk.Revit.DB.Document doc, Type type, BuiltInCategory bic) {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(bic);
            collector.OfClass(type);
            return collector;
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
        }

        public static string BICListMsg(ICollection<BuiltInCategory> _bicCats) {
            string strCats = "";
            foreach (BuiltInCategory iCat in _bicCats) {
                strCats = strCats + iCat.ToString().Replace("OST_", "") + ", ";
            }
            return strCats;
        }

        //public static void RandomColorPair(ref System.Drawing.Color ClrA, ref System.Drawing.Color ClrB) {
        //    Random rand = new Random();
        //    int randInt = rand.Next(0, 4);
        //    switch (randInt) {
        //        case 0:
        //            ClrA = System.Drawing.Color.Aqua;
        //            ClrB = System.Drawing.Color.Aquamarine;
        //            break;
        //        case 1:
        //            ClrA = System.Drawing.Color.PapayaWhip;
        //            ClrB = System.Drawing.Color.PeachPuff;
        //            break;
        //        case 2:
        //            ClrA = System.Drawing.Color.Bisque;
        //            ClrB = System.Drawing.Color.BlanchedAlmond;
        //            break;
        //        case 3:
        //            ClrA = System.Drawing.Color.Lavender;
        //            ClrB = System.Drawing.Color.LavenderBlush;
        //            break;
        //        default:
        //            ClrA = System.Drawing.Color.Aqua;
        //            ClrB = System.Drawing.Color.Aquamarine;
        //            break;
        //    }
        //}
    }
}
