namespace FFRKInspector.UI
{
    partial class FFRKViewRWs
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewRWs = new FFRKInspector.UI.DataGridViewEx();
            this.dgcUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcSBName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRWs)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRWs
            // 
            this.dataGridViewRWs.AllowUserToAddRows = false;
            this.dataGridViewRWs.AllowUserToDeleteRows = false;
            this.dataGridViewRWs.AllowUserToOrderColumns = true;
            this.dataGridViewRWs.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(213)))), ((int)(((byte)(180)))));
            this.dataGridViewRWs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewRWs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRWs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcUserId,
            this.dgcSBName});
            this.dataGridViewRWs.Location = new System.Drawing.Point(19, 3);
            this.dataGridViewRWs.Name = "dataGridViewRWs";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(210)))), ((int)(((byte)(228)))));
            this.dataGridViewRWs.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewRWs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRWs.ShowEditingIcon = false;
            this.dataGridViewRWs.Size = new System.Drawing.Size(485, 625);
            this.dataGridViewRWs.TabIndex = 0;
            // 
            // dgcUserId
            // 
            this.dgcUserId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcUserId.HeaderText = "User ID";
            this.dgcUserId.MinimumWidth = 50;
            this.dgcUserId.Name = "dgcUserId";
            this.dgcUserId.ReadOnly = true;
            this.dgcUserId.Width = 68;
            // 
            // dgcSBName
            // 
            this.dgcSBName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcSBName.HeaderText = "SB Name";
            this.dgcSBName.MinimumWidth = 60;
            this.dgcSBName.Name = "dgcSBName";
            this.dgcSBName.ReadOnly = true;
            this.dgcSBName.Width = 77;
            // 
            // FFRKViewRWs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewRWs);
            this.Name = "FFRKViewRWs";
            this.Size = new System.Drawing.Size(816, 631);
            this.Load += new System.EventHandler(this.FFRKViewInventory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRWs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewEx dataGridViewRWs;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSBName;
    }
}
