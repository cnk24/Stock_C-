namespace KiwoomStock
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.axKiwoom = new AxKHOpenAPILib.AxKHOpenAPI();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.logBuySell = new System.Windows.Forms.RichTextBox();
            this.logComm = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.axKiwoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // axKiwoom
            // 
            this.axKiwoom.Enabled = true;
            this.axKiwoom.Location = new System.Drawing.Point(722, 3);
            this.axKiwoom.Name = "axKiwoom";
            this.axKiwoom.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKiwoom.OcxState")));
            this.axKiwoom.Size = new System.Drawing.Size(60, 21);
            this.axKiwoom.TabIndex = 0;
            // 
            // dgvItems
            // 
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Location = new System.Drawing.Point(12, 12);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowTemplate.Height = 23;
            this.dgvItems.Size = new System.Drawing.Size(361, 298);
            this.dgvItems.TabIndex = 1;
            // 
            // logBuySell
            // 
            this.logBuySell.Location = new System.Drawing.Point(379, 12);
            this.logBuySell.Name = "logBuySell";
            this.logBuySell.Size = new System.Drawing.Size(393, 298);
            this.logBuySell.TabIndex = 2;
            this.logBuySell.Text = "";
            // 
            // logComm
            // 
            this.logComm.Location = new System.Drawing.Point(12, 316);
            this.logComm.Name = "logComm";
            this.logComm.Size = new System.Drawing.Size(760, 234);
            this.logComm.TabIndex = 3;
            this.logComm.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.logComm);
            this.Controls.Add(this.logBuySell);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.axKiwoom);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kiwoom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axKiwoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxKHOpenAPILib.AxKHOpenAPI axKiwoom;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.RichTextBox logBuySell;
        private System.Windows.Forms.RichTextBox logComm;
    }
}

