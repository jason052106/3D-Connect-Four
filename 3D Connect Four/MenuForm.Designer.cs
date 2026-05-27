namespace _3D_Connect_Four
{
    partial class MenuForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnPvP = new System.Windows.Forms.Button();
            this.btnPvE = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(60, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "歡迎來到立體四子棋";
            // 
            // btnPvP
            // 
            this.btnPvP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPvP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPvP.Location = new System.Drawing.Point(60, 120);
            this.btnPvP.Name = "btnPvP";
            this.btnPvP.Size = new System.Drawing.Size(260, 50);
            this.btnPvP.TabIndex = 1;
            this.btnPvP.Text = "本地雙人對戰 (玩家 vs 玩家)";
            this.btnPvP.UseVisualStyleBackColor = true;
            // 
            // btnPvE
            // 
            this.btnPvE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPvE.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPvE.Location = new System.Drawing.Point(60, 190);
            this.btnPvE.Name = "btnPvE";
            this.btnPvE.Size = new System.Drawing.Size(260, 50);
            this.btnPvE.TabIndex = 2;
            this.btnPvE.Text = "單人模式 (玩家 vs 電腦AI)";
            this.btnPvE.UseVisualStyleBackColor = true;
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPvE);
            this.Controls.Add(this.btnPvP);
            this.Controls.Add(this.label1);
            this.Name = "MenuForm";
            this.Text = "MenuForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPvP;
        private System.Windows.Forms.Button btnPvE;
    }
}