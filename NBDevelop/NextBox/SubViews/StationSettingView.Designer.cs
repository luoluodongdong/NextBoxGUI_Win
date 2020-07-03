namespace NextBox
{
    partial class StationSettingView
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
            this.backBtn = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.bgPictureBox = new System.Windows.Forms.PictureBox();
            this.loopCheckBox = new System.Windows.Forms.CheckBox();
            this.loopCountTB = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.bgPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // backBtn
            // 
            this.backBtn.BackgroundImage = global::NextBox.Properties.Resources.chevron_left_75px;
            this.backBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backBtn.Location = new System.Drawing.Point(19, 21);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(27, 34);
            this.backBtn.TabIndex = 0;
            this.backBtn.UseVisualStyleBackColor = true;
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleLabel.Location = new System.Drawing.Point(52, 24);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(184, 31);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "Station Setting";
            // 
            // bgPictureBox
            // 
            this.bgPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bgPictureBox.BackgroundImage = global::NextBox.Properties.Resources.Settings_1037px;
            this.bgPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bgPictureBox.Location = new System.Drawing.Point(678, 396);
            this.bgPictureBox.Name = "bgPictureBox";
            this.bgPictureBox.Size = new System.Drawing.Size(219, 201);
            this.bgPictureBox.TabIndex = 2;
            this.bgPictureBox.TabStop = false;
            // 
            // loopCheckBox
            // 
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.loopCheckBox.Location = new System.Drawing.Point(52, 303);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(60, 21);
            this.loopCheckBox.TabIndex = 3;
            this.loopCheckBox.Text = "Loop:";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            this.loopCheckBox.CheckedChanged += new System.EventHandler(this.loopCheckBox_CheckedChanged);
            // 
            // loopCountTB
            // 
            this.loopCountTB.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.loopCountTB.Location = new System.Drawing.Point(109, 301);
            this.loopCountTB.Name = "loopCountTB";
            this.loopCountTB.Size = new System.Drawing.Size(72, 23);
            this.loopCountTB.TabIndex = 4;
            this.loopCountTB.Text = "1";
            // 
            // StationSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loopCountTB);
            this.Controls.Add(this.loopCheckBox);
            this.Controls.Add(this.bgPictureBox);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.backBtn);
            this.Name = "StationSettingView";
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.StationSettingView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bgPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button backBtn;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.PictureBox bgPictureBox;
        private System.Windows.Forms.CheckBox loopCheckBox;
        private System.Windows.Forms.TextBox loopCountTB;
    }
}
