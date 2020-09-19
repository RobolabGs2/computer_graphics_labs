namespace Lab03
{
    partial class mainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.colorPictureBox = new System.Windows.Forms.PictureBox();
            this.toolPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPictureBox
            // 
            this.mainPictureBox.Location = new System.Drawing.Point(191, 12);
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.Size = new System.Drawing.Size(1005, 763);
            this.mainPictureBox.TabIndex = 0;
            this.mainPictureBox.TabStop = false;
            this.mainPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPictureBox_MouseDown);
            this.mainPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainPictureBox_MouseMove);
            this.mainPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainPictureBox_MouseUp);
            // 
            // colorPictureBox
            // 
            this.colorPictureBox.Location = new System.Drawing.Point(12, 12);
            this.colorPictureBox.Name = "colorPictureBox";
            this.colorPictureBox.Size = new System.Drawing.Size(173, 88);
            this.colorPictureBox.TabIndex = 2;
            this.colorPictureBox.TabStop = false;
            this.colorPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.colorPictureBox_MouseDown);
            // 
            // toolPanel
            // 
            this.toolPanel.Location = new System.Drawing.Point(12, 106);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(173, 669);
            this.toolPanel.TabIndex = 3;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1208, 787);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.colorPictureBox);
            this.Controls.Add(this.mainPictureBox);
            this.Name = "mainForm";
            this.Text = "Lab03";
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.PictureBox colorPictureBox;
        private System.Windows.Forms.FlowLayoutPanel toolPanel;
    }
}

