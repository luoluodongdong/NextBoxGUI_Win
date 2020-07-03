namespace NextBox
{
    partial class UnitView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.titleCheckBox = new System.Windows.Forms.CheckBox();
            this.snLabel = new System.Windows.Forms.Label();
            this.testStateLabel = new System.Windows.Forms.Label();
            this.messageLabel = new System.Windows.Forms.Label();
            this.settingBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleCheckBox
            // 
            this.titleCheckBox.AutoSize = true;
            this.titleCheckBox.Checked = true;
            this.titleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.titleCheckBox.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleCheckBox.Location = new System.Drawing.Point(8, 3);
            this.titleCheckBox.Name = "titleCheckBox";
            this.titleCheckBox.Size = new System.Drawing.Size(77, 25);
            this.titleCheckBox.TabIndex = 0;
            this.titleCheckBox.Text = "Unit-1";
            this.titleCheckBox.UseVisualStyleBackColor = true;
            this.titleCheckBox.CheckStateChanged += new System.EventHandler(this.titleCheckBox_CheckStateChanged);
            // 
            // snLabel
            // 
            this.snLabel.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.snLabel.Location = new System.Drawing.Point(3, 37);
            this.snLabel.Name = "snLabel";
            this.snLabel.Size = new System.Drawing.Size(294, 29);
            this.snLabel.TabIndex = 1;
            this.snLabel.Text = "SN123456789012345678901";
            this.snLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // testStateLabel
            // 
            this.testStateLabel.Font = new System.Drawing.Font("微软雅黑", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.testStateLabel.ForeColor = System.Drawing.Color.Silver;
            this.testStateLabel.Location = new System.Drawing.Point(8, 75);
            this.testStateLabel.Name = "testStateLabel";
            this.testStateLabel.Size = new System.Drawing.Size(278, 111);
            this.testStateLabel.TabIndex = 2;
            this.testStateLabel.Text = "Test...";
            this.testStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.testStateLabel.Click += new System.EventHandler(this.testStateLabel_Click);
            this.testStateLabel.MouseEnter += new System.EventHandler(this.testStateLabel_MouseEnter);
            this.testStateLabel.MouseLeave += new System.EventHandler(this.testStateLabel_MouseLeave);
            // 
            // messageLabel
            // 
            this.messageLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.messageLabel.Location = new System.Drawing.Point(17, 193);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(250, 110);
            this.messageLabel.TabIndex = 3;
            this.messageLabel.Text = "1.Test item 1";
            // 
            // settingBtn
            // 
            this.settingBtn.BackgroundImage = global::NextBox.Properties.Resources.settings_284px;
            this.settingBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.settingBtn.Location = new System.Drawing.Point(241, 296);
            this.settingBtn.Name = "settingBtn";
            this.settingBtn.Size = new System.Drawing.Size(44, 41);
            this.settingBtn.TabIndex = 4;
            this.settingBtn.UseVisualStyleBackColor = true;
            this.settingBtn.Click += new System.EventHandler(this.settingBtn_Click);
            // 
            // UnitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Khaki;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.settingBtn);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.testStateLabel);
            this.Controls.Add(this.snLabel);
            this.Controls.Add(this.titleCheckBox);
            this.Name = "UnitView";
            this.Size = new System.Drawing.Size(296, 346);
            this.Load += new System.EventHandler(this.UnitView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox titleCheckBox;
        private System.Windows.Forms.Label snLabel;
        private System.Windows.Forms.Label testStateLabel;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Button settingBtn;
    }
}
