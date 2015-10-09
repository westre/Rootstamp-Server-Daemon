namespace RSD {
    partial class MainForm {
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
            this.messageLog = new System.Windows.Forms.ListBox();
            this.gameServerPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // messageLog
            // 
            this.messageLog.FormattingEnabled = true;
            this.messageLog.Location = new System.Drawing.Point(13, 13);
            this.messageLog.Name = "messageLog";
            this.messageLog.Size = new System.Drawing.Size(351, 329);
            this.messageLog.TabIndex = 0;
            // 
            // gameServerPanel
            // 
            this.gameServerPanel.Location = new System.Drawing.Point(371, 13);
            this.gameServerPanel.Name = "gameServerPanel";
            this.gameServerPanel.Size = new System.Drawing.Size(200, 329);
            this.gameServerPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 361);
            this.Controls.Add(this.gameServerPanel);
            this.Controls.Add(this.messageLog);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox messageLog;
        private System.Windows.Forms.Panel gameServerPanel;
    }
}

