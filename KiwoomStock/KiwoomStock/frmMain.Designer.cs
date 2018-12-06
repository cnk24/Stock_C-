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
            this.dgvWallet = new System.Windows.Forms.DataGridView();
            this.btnConnect = new System.Windows.Forms.CheckBox();
            this.lbMsg = new System.Windows.Forms.Label();
            this.logComm = new System.Windows.Forms.ListBox();
            this.logBuySell = new System.Windows.Forms.ListBox();
            this.btnWallet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWallet)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvWallet
            // 
            this.dgvWallet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWallet.Location = new System.Drawing.Point(0, 0);
            this.dgvWallet.Name = "dgvWallet";
            this.dgvWallet.RowTemplate.Height = 23;
            this.dgvWallet.Size = new System.Drawing.Size(361, 310);
            this.dgvWallet.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnConnect.Location = new System.Drawing.Point(677, 0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 30);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.CheckedChanged += new System.EventHandler(this.btnConnect_CheckedChanged);
            // 
            // lbMsg
            // 
            this.lbMsg.Location = new System.Drawing.Point(547, 4);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.Size = new System.Drawing.Size(124, 23);
            this.lbMsg.TabIndex = 5;
            this.lbMsg.Text = "연결하세요";
            this.lbMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logComm
            // 
            this.logComm.FormattingEnabled = true;
            this.logComm.ItemHeight = 12;
            this.logComm.Location = new System.Drawing.Point(0, 316);
            this.logComm.Name = "logComm";
            this.logComm.Size = new System.Drawing.Size(781, 244);
            this.logComm.TabIndex = 6;
            // 
            // logBuySell
            // 
            this.logBuySell.FormattingEnabled = true;
            this.logBuySell.ItemHeight = 12;
            this.logBuySell.Location = new System.Drawing.Point(367, 31);
            this.logBuySell.Name = "logBuySell";
            this.logBuySell.Size = new System.Drawing.Size(414, 280);
            this.logBuySell.TabIndex = 7;
            // 
            // btnWallet
            // 
            this.btnWallet.Location = new System.Drawing.Point(367, 0);
            this.btnWallet.Name = "btnWallet";
            this.btnWallet.Size = new System.Drawing.Size(110, 30);
            this.btnWallet.TabIndex = 8;
            this.btnWallet.Text = "잔고";
            this.btnWallet.UseVisualStyleBackColor = true;
            this.btnWallet.Click += new System.EventHandler(this.btnWallet_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnWallet);
            this.Controls.Add(this.logBuySell);
            this.Controls.Add(this.logComm);
            this.Controls.Add(this.lbMsg);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.dgvWallet);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kiwoom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWallet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvWallet;
        private System.Windows.Forms.CheckBox btnConnect;
        private System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.ListBox logComm;
        private System.Windows.Forms.ListBox logBuySell;
        private System.Windows.Forms.Button btnWallet;
    }
}

