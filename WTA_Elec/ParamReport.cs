using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;

namespace WTA_Elec {
    public partial class ParamReport : System.Windows.Forms.Form {
        UIDocument _thisuidoc;
        Document _thisDoc;
        Dictionary<string, BuiltInCategory> bicDict = new Dictionary<string, BuiltInCategory>();
        Dictionary<Guid, IList<string>> pGuid2Fams = new Dictionary<Guid, IList<string>>();
        BuiltInCategory thisBic;
        String bicName = "Lighting Fixtures";
        String defTitle = "Parameter Reporter";

        public ParamReport(UIDocument thisUIDoc) {
            InitializeComponent();
            _thisuidoc = thisUIDoc;
            _thisDoc = thisUIDoc.Document;
        }


        private void buttonQuit2_Click(object sender, EventArgs e) {
            Close();
        }

        private void ParamReport_Load(object sender, EventArgs e) {
            Categories categories = _thisDoc.Settings.Categories;
            foreach (Category c in categories) {
                if (null != c) {
                    comboBoxBIC.Items.Add(c.Name);
                    bicDict[c.Name] = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), c.Id.ToString());
                }
            }
            comboBoxBIC.Sorted = true;
            if (comboBoxBIC.Items.Contains(bicName)) {
                comboBoxBIC.Text = bicName;
            }
        }

        private void GoThroughFamilies(BuiltInCategory _bic) {
            // Collect all families of desired category
            FilteredElementCollector collectorFams = new FilteredElementCollector(_thisDoc);

            var theFamilyTypes = collectorFams
                                 .OfClass(typeof(FamilySymbol))
                                 .OfCategory(_bic)
                                 .Distinct()
                                 .ToList();

            // The 'Symbol' is the Family's Type. The Type's Family is the Family name.
            //  MessageBox.Show("There are " + theFamilyTypes.Count.ToString());
            try {
                dataGridViewFamList.Rows.Clear();
                foreach (FamilySymbol FamType in theFamilyTypes) {
                    this.Text = defTitle + "    Collecting: " + FamType.Name;
                    var indx = dataGridViewFamList.Rows.Add();
                    dataGridViewFamList.Rows[indx].Cells[0].Value = FamType.Family.Name;
                    dataGridViewFamList.Rows[indx].Cells[1].Value = FamType.Name;
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                throw;
            }

            this.Text = defTitle;

            Dictionary<string, IList<string>> pDefsInUse = new Dictionary<string, IList<string>>();
            try {
                foreach (FamilySymbol FamTyp in theFamilyTypes) {
                    List<Parameter> pFIFixType = (from Parameter p in FamTyp.Parameters select p).ToList();
                    foreach (Parameter p in pFIFixType) {
                        this.Text = defTitle + "    Scanning: " + FamType.Name;
                        if (p.UserModifiable && p.IsShared) {
                            string pDN = p.Definition.Name;
                            if (!pDefsInUse.ContainsKey(pDN)) {
                                pDefsInUse.Add(pDN, new List<string> { p.GUID.ToString() });
                            } else {
                                if (!pDefsInUse[pDN].Contains(p.GUID.ToString())) {
                                    // MessageBox.Show("Found Duplicate => " + p.Definition.Name);
                                    pDefsInUse[pDN].Add(p.GUID.ToString());
                                }
                            }
                            // record the families that are using this GUID
                            if (!pGuid2Fams.ContainsKey(p.GUID)) {
                                pGuid2Fams.Add(p.GUID, new List<string> { FamTyp.Family.Name });
                            } else {
                                if (!pGuid2Fams[p.GUID].Contains(FamTyp.Family.Name)) {
                                    // MessageBox.Show("Found Duplicate => " + p.Definition.Name);
                                    pGuid2Fams[p.GUID].Add(FamTyp.Family.Name);
                                }
                            }
                        }
                    }
                } // end each famtyp

                this.Text = defTitle;

                dataGridViewParamRept.Rows.Clear();
                foreach (var item in pDefsInUse.OrderBy(i => i.Key)) {
                    string pDefName = item.Key;
                    IList<string> pstrVal = item.Value;
                    string strGuids = "";
                    foreach (string strV in pstrVal) {
                        strGuids = strGuids + strV + " ";
                    }
                    dataGridViewParamRept.Rows.Add();
                    int indx = dataGridViewParamRept.RowCount - 1;
                    dataGridViewParamRept.Rows[indx].Cells[0].Value = pDefName;
                    dataGridViewParamRept.Rows[indx].Cells[1].Value = strGuids.TrimEnd();
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
            dataGridViewFamList.ClearSelection();
            dataGridViewParamRept.ClearSelection();
        }

        private void comboBoxBIC_SelectedIndexChanged(object sender, EventArgs e) {
            String bicName = comboBoxBIC.Text;
            try {
                thisBic = bicDict[bicName];
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            GoThroughFamilies(thisBic);
        }

        private void contextMenuStripGUID_Opening(object sender, CancelEventArgs e) {
            toolStripComboBoxGUIDS.Items.Clear();
            int sr = dataGridViewParamRept.SelectedRows.Count;

            if (sr == 1) {
                string gs = dataGridViewParamRept.SelectedRows[0].Cells[1].Value.ToString();
                IList<String> glst = gs.Split(' ').ToList();
                foreach (String gdItm in glst) {
                    toolStripComboBoxGUIDS.Items.Add(gdItm);
                }
                toolStripComboBoxGUIDS.SelectedIndex = 0;
            }
        }

        private void SelectOnlyFamiliesUsingGUID(Guid _thisGuid) {
            IList<string> lstFam = pGuid2Fams[_thisGuid];
            dataGridViewFamList.ClearSelection();
            foreach (string famN in lstFam) {
                foreach (DataGridViewRow thisRow in dataGridViewFamList.Rows) {
                    if (famN.Equals(thisRow.Cells[0].Value.ToString())) {
                        thisRow.Selected = true;
                    }
                }
            }
        }

        private void toolStripComboBoxGUIDS_SelectedIndexChanged(object sender, EventArgs e) {
            string gItm = toolStripComboBoxGUIDS.SelectedItem.ToString();
            Guid thisGuid = Guid.Parse(gItm);
            SelectOnlyFamiliesUsingGUID(thisGuid);
        }

        private void ShowForParmSelection(int _row) {
            string gs = dataGridViewParamRept.Rows[_row].Cells[1].Value.ToString();
            IList<String> glst = gs.Split(' ').ToList();
            Guid thisGuid = Guid.Parse(glst[0]);
            SelectOnlyFamiliesUsingGUID(thisGuid);
        }

        private void dataGridViewParamRept_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (dataGridViewParamRept.SelectedRows.Count > 0) {
                dataGridViewFamList.ClearSelection();
            }
            int row = e.RowIndex;
            ShowForParmSelection(row);
        }

        private void dataGridViewParamRept_KeyUp(object sender, KeyEventArgs e) {
            if (dataGridViewParamRept.SelectedRows.Count > 0) {
                dataGridViewFamList.ClearSelection();
            }
            int row = dataGridViewParamRept.SelectedCells[0].RowIndex;
            ShowForParmSelection(row);
        }
    }
}
