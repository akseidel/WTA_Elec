using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.IO;
using System.Runtime.InteropServices;

namespace WTA_Elec {
    class LightingPowerDensityTotalizer {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int SetWindowText(
          IntPtr hWnd,
          string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(
          IntPtr hwndParent,
          IntPtr hwndChildAfter,
          string lpszClass,
          string lpszWindow);

        public static void SetStatusBarText(IntPtr mainWindow, string text) {
            IntPtr mainWindowHandle = IntPtr.Zero;
            IntPtr statusBar = FindWindowEx(mainWindow, IntPtr.Zero, "msctls_statusbar32", "");
            if (statusBar != IntPtr.Zero) {
                SetWindowText(statusBar, text);
            }
        }

        public void StartLightFinding(UIApplication m_app) {
            UIDocument uidoc = m_app.ActiveUIDocument;
            Document doc = uidoc.Document;
            string ThisProjectFile = System.IO.Path.GetFileNameWithoutExtension(doc.PathName);
            string rootExportFolder = "c:\\temp\\";
            string outPutTail = "_LPD.csv";
            string exportToCVSFileName;
            string pNamePWR = "INPUT POWER";
            string strHead;
            IntPtr revitHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            bool detailRpt = true;
           

            // phase basis is the phase on which element status is determined for inclusion
            Phase phsBasis = FamilyUtils.GetDesiredPhaseBasis(doc);

            if (!Directory.Exists(rootExportFolder)) {
                try {
                    Directory.CreateDirectory(rootExportFolder);
                } catch (Exception) {
                    TaskDialog.Show("Cannot create the output folder " + rootExportFolder, "Exiting");
                    return;
                }
            }

            TaskDialog td = new TaskDialog("Find Lights");
            td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            td.Title = "Writing Lighting Information To CSV File";
            td.TitleAutoPrefix = false;
            td.AllowCancellation = true;
            td.MainInstruction = "Select either verbose or brief output. Verbose includes a room by room fixture breakdown. Brief provides only the power totals.";
            td.MainContent = "The files will be CSV format written to " + rootExportFolder + ". They will be timestamp named and opened for you, presumably by Excel.";

            td.CommonButtons = TaskDialogCommonButtons.Cancel; 
            td.DefaultButton = TaskDialogResult.Cancel;

            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Brief Report");
            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Verbose Report");

            TaskDialogResult tdRes = td.Show();
            if (tdRes == TaskDialogResult.Cancel) { return; } 
            if (TaskDialogResult.CommandLink1 == tdRes) { //  "Brief Report"
                detailRpt = false;
            }
            if (TaskDialogResult.CommandLink2 == tdRes) { //  "Verbose Report"
                detailRpt = true;
            }
            
            if (detailRpt) {
                strHead = "Room Number,Room Name,Floor Level,Area SF,Item Name,Item Count,Item Watts,Total Watts,Pwr Density Contribution";
                exportToCVSFileName = rootExportFolder + ThisProjectFile + "_VERBOSE" + outPutTail;
            } else {
                strHead = "Room Number,Room Name,Area SF,L.Pwr. Density w/sf,Tot. W";
                exportToCVSFileName = rootExportFolder + ThisProjectFile + "_BRIEF" + outPutTail;
            }

            exportToCVSFileName = AssignNewUniqueFileName(exportToCVSFileName);
            //System.Windows.MessageBox.Show("Will write to: " + exportToCVSFileName, "FYI");


            FormMsgWPF waitItOut = new FormMsgWPF();
            waitItOut.SetMsg("Trust this, just wait it out if Revit goes 'Not Responding'.", "Believe It Or Not");
            waitItOut.Show();

            using (StreamWriter writer = new StreamWriter(exportToCVSFileName)) {
                writer.WriteLine("Data is for items existing and not demolished in phases up to and including: " + phsBasis.Name);
                writer.WriteLine(strHead);
                try {
                    // Make sure no other RVTs are open in Revit
                    if (MultipleRVTsOpen(m_app) == true) {
                        TaskDialog.Show("Process Stopped", "Please only have one file open when running this tool");
                        return;
                    }
                    // Iterate through each document
                    foreach (Document _doc in m_app.Application.Documents) {
                        // Only process links
                        if (_doc.IsLinked) {
                            #region  Create a room collection from rooms in the current link
                            RoomFilter roomFilter = new RoomFilter();
                            FilteredElementCollector filteredElemCol = new FilteredElementCollector(_doc);
                            filteredElemCol.WherePasses(roomFilter).WhereElementIsNotElementType();
                            #endregion

                            string strRoomData = "";

                            // Changed to this go get to work in Revit 2015
                            int eleCnt = filteredElemCol.ToList().Count();
                            // Originally was this, working in Revit 2016
                            // int eleCnt = filteredElemCol.GetElementCount();
                            #region ProcessCollection if any
                            if (eleCnt > 0) {
                                string strPurpose = "Rooms In " + System.IO.Path.GetFileNameWithoutExtension(_doc.PathName);
                                SetStatusBarText(revitHandle, "Starting scan ..." + strPurpose);

                                #region Iterate through each room
                                foreach (Room _room in filteredElemCol) {
                                    // Only process placed rooms
                                    if (IsRoomPlaced(_room) == false) { continue; }
                                    double selRmArea = _room.Area;
                                    // Get all LightingFixtures in the current room
                                    BuiltInCategory bic = BuiltInCategory.OST_LightingFixtures;
                                    List<FamilyInstance> famInstances = FamilyUtils.FamilyInstanceCategoryInThisRoom(_room, bic, m_app.ActiveUIDocument.Document, phsBasis);
                                    //System.Windows.MessageBox.Show("Have list for "+ _room.Name);
                                    if (famInstances != null) {
                                        Dictionary<string, int> dicFamInstances = new Dictionary<string, int>();
                                        Dictionary<string, double> dicLightFixTypeIP = new Dictionary<string, double>();
                                        if (famInstances != null) {
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
                                                    }
                                                }
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
                                                double pwrDensContribution = totalWattsForItem / _room.Area;
                                                totRoomLWatts = totRoomLWatts + totalWattsForItem;
                                                msgDetail = msgDetail + "\n" + 
                                                            itemName + "  cnt " +
                                                            itemCount.ToString() + " @ " +
                                                            itemWatts.ToString("0.00") +
                                                            " for " +
                                                            totalWattsForItem.ToString("0.0 w");
                                                if (detailRpt) {
                                                    msgDetail = _room.Number + "," +
                                                                _room.Name + "," +
                                                                _room.Level.Name + "," +
                                                                _room.Area.ToString("0.00") + "," +
                                                                itemName + "," +
                                                                itemCount.ToString() + "," +
                                                                itemWatts.ToString("0.00") + "," +
                                                                totalWattsForItem.ToString("0.00") + "," +
                                                                pwrDensContribution.ToString("0.00000000");
                                                    writer.WriteLine(msgDetail);
                                                }

                                            } else {
                                                /// item key not in dictionary!
                                            }
                                        }
                                        double lightFixPWRDensity = totRoomLWatts / selRmArea;
                                        string msgMain =
                                               "Rm Area: " + selRmArea.ToString("0.00 sf") + "  L Pwr. Density: " + lightFixPWRDensity.ToString("0.00 w/sf")
                                               + "\n"
                                               + msgDetail;

                                        string strLine = _room.Number + "," + _room.Name + "," + selRmArea.ToString("0.00") + "," + lightFixPWRDensity.ToString("0.00") + "," + totRoomLWatts.ToString();
                                        string strProgressWPF = "Room: " + _room.Number + " Name: " + _room.Name + " sf:" + selRmArea.ToString("0.00") + " w/sf:" + lightFixPWRDensity.ToString("0.00");
                                        strProgressWPF = strProgressWPF + " ... wait it out if Revit goes Not Responding";
                                        SetStatusBarText(revitHandle, strProgressWPF);
                                        strRoomData = strRoomData + Environment.NewLine + strLine;
                                        if (!detailRpt) { writer.WriteLine(strLine); }
                                    } // end if family instances
                                } // end for each room 
                                #endregion
                                string msgFullBody = strHead + strRoomData;
                                #region Debug Show Results
                                //string msgFullBody = strHead + strRoomData;
                                //TaskDialog thisDialog = new TaskDialog(System.IO.Path.GetFileNameWithoutExtension(_doc.PathName));
                                //thisDialog.TitleAutoPrefix = false;
                                //thisDialog.MainIcon = TaskDialogIcon.TaskDialogIconNone;
                                //thisDialog.CommonButtons = TaskDialogCommonButtons.Close;
                                //thisDialog.MainContent = "";
                                //thisDialog.MainInstruction = msgFullBody;
                                //TaskDialogResult tResult = thisDialog.Show();
                                #endregion
                            }
                            #endregion
                            SetStatusBarText(revitHandle, "Did do that one, whatever it was.");
                        } // end if linked
                    } // end foreach document
                    SetStatusBarText(revitHandle, "Done with all linked Revit documents.");
                } catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.Message + "\n" + ex.ToString(), "Error At StartLightFinding");
                }
            } // end using streamwriter

            waitItOut.Close();
            // Open in what system wants to use, probably Excel
            FileInfo fileInfo = new FileInfo(exportToCVSFileName);
            if (fileInfo.Exists) {
                System.Diagnostics.Process.Start(exportToCVSFileName);
            }

        }

        private bool MultipleRVTsOpen(UIApplication m_app) {
            int intCntr = 0;
            try {
                // Iterate through each document
                foreach (Document doc in m_app.Application.Documents) {
                    // Skip linked RVTs and families
                    if (!doc.IsLinked && !doc.IsFamilyDocument) {
                        intCntr++;
                        if (intCntr > 1)
                            return true;
                    }
                }
                return false;
            } catch {
                return true;
            }
        }

        public bool IsRoomPlaced(Room _Room) {
            try {
                // Make sure the room does not contain a location or area value
                if (null != _Room.Location && Math.Round(_Room.Area) != 0.0)
                    return true;
                else
                    return false;
            } catch {
                return false;
            }
        }

        // Given a full path file name will return an indexed unique new file name.
        // 
        private string AssignNewUniqueFileName(string _orgName) {
            var thereIsOneAlready = false;
            var retryCnt = 100; // safety valve
            string _orgNameWOExt = System.IO.Path.GetFileNameWithoutExtension(_orgName);
            string path = System.IO.Path.GetDirectoryName(_orgName);
            string ext = System.IO.Path.GetExtension(_orgName);
            string _newName = path + "\\" + _orgNameWOExt + "_" + GetTimestamp(DateTime.Now) + ext;
            // going to start name indx from the current numver of similar files 
            int count = Directory.GetFiles(path, "*" + _orgNameWOExt + "*", SearchOption.TopDirectoryOnly).Count();
            thereIsOneAlready = File.Exists(_newName); // success needs to be false to drop out
            while (thereIsOneAlready && retryCnt > 0) {
                _newName = path + "\\" + _orgNameWOExt + "_" + GetTimestamp(DateTime.Now) + ext;
                thereIsOneAlready = File.Exists(_newName); // success needs to be false to drop out
                retryCnt -= 1;
            }
            return _newName;
        }

        public static String GetTimestamp(DateTime value) {
            return value.ToString("yyyyMMddHHmmss");
        }

    } // end class 
} // end namespace
