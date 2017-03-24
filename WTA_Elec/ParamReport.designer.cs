namespace WTA_Elec {
    partial class ParamReport {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonQuit2 = new System.Windows.Forms.Button();
            this.panelParamGridView = new System.Windows.Forms.Panel();
            this.dataGridViewParamRept = new System.Windows.Forms.DataGridView();
            this.ParmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripGUID = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripComboBoxGUIDS = new System.Windows.Forms.ToolStripComboBox();
            this.panelFamList = new System.Windows.Forms.Panel();
            this.dataGridViewFamList = new System.Windows.Forms.DataGridView();
            this.FamSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FamType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBoxBIC = new System.Windows.Forms.ComboBox();
            this.labelBIC = new System.Windows.Forms.Label();
            this.splitContainerPanelsHolder = new System.Windows.Forms.SplitContainer();
            this.toolTipReporter = new System.Windows.Forms.ToolTip(this.components);
            this.panelParamGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParamRept)).BeginInit();
            this.contextMenuStripGUID.SuspendLayout();
            this.panelFamList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFamList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPanelsHolder)).BeginInit();
            this.splitContainerPanelsHolder.Panel1.SuspendLayout();
            this.splitContainerPanelsHolder.Panel2.SuspendLayout();
            this.splitContainerPanelsHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonQuit2
            // 
            this.buttonQuit2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit2.BackColor = System.Drawing.Color.Red;
            this.buttonQuit2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonQuit2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonQuit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQuit2.Location = new System.Drawing.Point(831, 12);
            this.buttonQuit2.Name = "buttonQuit2";
            this.buttonQuit2.Size = new System.Drawing.Size(98, 23);
            this.buttonQuit2.TabIndex = 49;
            this.buttonQuit2.Text = "Quit";
            this.buttonQuit2.UseVisualStyleBackColor = false;
            this.buttonQuit2.Click += new System.EventHandler(this.buttonQuit2_Click);
            // 
            // panelParamGridView
            // 
            this.panelParamGridView.Controls.Add(this.dataGridViewParamRept);
            this.panelParamGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelParamGridView.Location = new System.Drawing.Point(0, 0);
            this.panelParamGridView.Name = "panelParamGridView";
            this.panelParamGridView.Size = new System.Drawing.Size(524, 675);
            this.panelParamGridView.TabIndex = 50;
            // 
            // dataGridViewParamRept
            // 
            this.dataGridViewParamRept.AllowUserToAddRows = false;
            this.dataGridViewParamRept.AllowUserToDeleteRows = false;
            this.dataGridViewParamRept.AllowUserToOrderColumns = true;
            this.dataGridViewParamRept.AllowUserToResizeRows = false;
            this.dataGridViewParamRept.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewParamRept.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewParamRept.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewParamRept.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParmName,
            this.GUID});
            this.dataGridViewParamRept.ContextMenuStrip = this.contextMenuStripGUID;
            this.dataGridViewParamRept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewParamRept.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewParamRept.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewParamRept.Name = "dataGridViewParamRept";
            this.dataGridViewParamRept.ReadOnly = true;
            this.dataGridViewParamRept.RowHeadersVisible = false;
            this.dataGridViewParamRept.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewParamRept.Size = new System.Drawing.Size(524, 675);
            this.dataGridViewParamRept.TabIndex = 64;
            this.toolTipReporter.SetToolTip(this.dataGridViewParamRept, "The first GUID is automatically used for\r\nreporting. Right click to select any ot" +
        "her GUID\r\nthat is aslo associated to the parmeter name. ");
            this.dataGridViewParamRept.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewParamRept_CellClick);
            this.dataGridViewParamRept.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewParamRept_KeyUp);
            // 
            // ParmName
            // 
            this.ParmName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ParmName.DefaultCellStyle = dataGridViewCellStyle2;
            this.ParmName.HeaderText = "Parameter Name In Family";
            this.ParmName.Name = "ParmName";
            this.ParmName.ReadOnly = true;
            // 
            // GUID
            // 
            this.GUID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GUID.DefaultCellStyle = dataGridViewCellStyle3;
            this.GUID.HeaderText = "Associated GUID(S)";
            this.GUID.Name = "GUID";
            this.GUID.ReadOnly = true;
            // 
            // contextMenuStripGUID
            // 
            this.contextMenuStripGUID.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxGUIDS});
            this.contextMenuStripGUID.Name = "contextMenuStripGUID";
            this.contextMenuStripGUID.ShowImageMargin = false;
            this.contextMenuStripGUID.Size = new System.Drawing.Size(396, 158);
            this.contextMenuStripGUID.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripGUID_Opening);
            // 
            // toolStripComboBoxGUIDS
            // 
            this.toolStripComboBoxGUIDS.DropDownHeight = 60;
            this.toolStripComboBoxGUIDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.toolStripComboBoxGUIDS.DropDownWidth = 360;
            this.toolStripComboBoxGUIDS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBoxGUIDS.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripComboBoxGUIDS.IntegralHeight = false;
            this.toolStripComboBoxGUIDS.Name = "toolStripComboBoxGUIDS";
            this.toolStripComboBoxGUIDS.Size = new System.Drawing.Size(360, 150);
            this.toolStripComboBoxGUIDS.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxGUIDS_SelectedIndexChanged);
            // 
            // panelFamList
            // 
            this.panelFamList.Controls.Add(this.dataGridViewFamList);
            this.panelFamList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFamList.Location = new System.Drawing.Point(0, 0);
            this.panelFamList.Name = "panelFamList";
            this.panelFamList.Size = new System.Drawing.Size(398, 675);
            this.panelFamList.TabIndex = 64;
            // 
            // dataGridViewFamList
            // 
            this.dataGridViewFamList.AllowUserToAddRows = false;
            this.dataGridViewFamList.AllowUserToDeleteRows = false;
            this.dataGridViewFamList.AllowUserToResizeRows = false;
            this.dataGridViewFamList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewFamList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewFamList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFamList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FamSymbol,
            this.FamType});
            this.dataGridViewFamList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewFamList.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewFamList.Name = "dataGridViewFamList";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewFamList.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewFamList.RowHeadersVisible = false;
            this.dataGridViewFamList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewFamList.Size = new System.Drawing.Size(398, 675);
            this.dataGridViewFamList.TabIndex = 0;
            // 
            // FamSymbol
            // 
            this.FamSymbol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FamSymbol.HeaderText = "Family";
            this.FamSymbol.Name = "FamSymbol";
            // 
            // FamType
            // 
            this.FamType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FamType.HeaderText = "Type Within Family";
            this.FamType.Name = "FamType";
            // 
            // comboBoxBIC
            // 
            this.comboBoxBIC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBIC.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxBIC.FormattingEnabled = true;
            this.comboBoxBIC.Location = new System.Drawing.Point(145, 10);
            this.comboBoxBIC.Name = "comboBoxBIC";
            this.comboBoxBIC.Size = new System.Drawing.Size(211, 21);
            this.comboBoxBIC.TabIndex = 65;
            this.comboBoxBIC.SelectedIndexChanged += new System.EventHandler(this.comboBoxBIC_SelectedIndexChanged);
            // 
            // labelBIC
            // 
            this.labelBIC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBIC.Location = new System.Drawing.Point(4, 13);
            this.labelBIC.Name = "labelBIC";
            this.labelBIC.Size = new System.Drawing.Size(139, 23);
            this.labelBIC.TabIndex = 66;
            this.labelBIC.Text = "Builit In Family Category:";
            // 
            // splitContainerPanelsHolder
            // 
            this.splitContainerPanelsHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerPanelsHolder.Location = new System.Drawing.Point(3, 41);
            this.splitContainerPanelsHolder.Name = "splitContainerPanelsHolder";
            // 
            // splitContainerPanelsHolder.Panel1
            // 
            this.splitContainerPanelsHolder.Panel1.Controls.Add(this.panelFamList);
            // 
            // splitContainerPanelsHolder.Panel2
            // 
            this.splitContainerPanelsHolder.Panel2.Controls.Add(this.panelParamGridView);
            this.splitContainerPanelsHolder.Size = new System.Drawing.Size(926, 675);
            this.splitContainerPanelsHolder.SplitterDistance = 398;
            this.splitContainerPanelsHolder.TabIndex = 67;
            // 
            // ParamReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(941, 728);
            this.Controls.Add(this.splitContainerPanelsHolder);
            this.Controls.Add(this.labelBIC);
            this.Controls.Add(this.comboBoxBIC);
            this.Controls.Add(this.buttonQuit2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParamReport";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Parameter Reporter";
            this.Load += new System.EventHandler(this.ParamReport_Load);
            this.panelParamGridView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParamRept)).EndInit();
            this.contextMenuStripGUID.ResumeLayout(false);
            this.panelFamList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFamList)).EndInit();
            this.splitContainerPanelsHolder.Panel1.ResumeLayout(false);
            this.splitContainerPanelsHolder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPanelsHolder)).EndInit();
            this.splitContainerPanelsHolder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonQuit2;
        private System.Windows.Forms.Panel panelParamGridView;
        private System.Windows.Forms.Panel panelFamList;
        private System.Windows.Forms.DataGridView dataGridViewParamRept;
        private System.Windows.Forms.ComboBox comboBoxBIC;
        private System.Windows.Forms.Label labelBIC;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGUID;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxGUIDS;
        private System.Windows.Forms.DataGridView dataGridViewFamList;
        private System.Windows.Forms.DataGridViewTextBoxColumn FamSymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn FamType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GUID;
        private System.Windows.Forms.SplitContainer splitContainerPanelsHolder;
        private System.Windows.Forms.ToolTip toolTipReporter;
    }
}