namespace Lab06
{
    partial class MainForm
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
            this.splitList = new System.Windows.Forms.SplitContainer();
            this.splitTools = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitList)).BeginInit();
            this.splitList.Panel2.SuspendLayout();
            this.splitList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTools)).BeginInit();
            this.splitTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitList
            // 
            this.splitList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitList.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitList.Location = new System.Drawing.Point(0, 0);
            this.splitList.Name = "splitList";
            // 
            // splitList.Panel2
            // 
            this.splitList.Panel2.Controls.Add(this.splitTools);
            this.splitList.Size = new System.Drawing.Size(1099, 724);
            this.splitList.SplitterDistance = 233;
            this.splitList.TabIndex = 0;
            // 
            // splitTools
            // 
            this.splitTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTools.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitTools.Location = new System.Drawing.Point(0, 0);
            this.splitTools.Name = "splitTools";
            this.splitTools.Size = new System.Drawing.Size(862, 724);
            this.splitTools.SplitterDistance = 595;
            this.splitTools.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1099, 724);
            this.Controls.Add(this.splitList);
            this.Name = "MainForm";
            this.Text = "Lab06";
            this.splitList.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitList)).EndInit();
            this.splitList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTools)).EndInit();
            this.splitTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitList;
        private System.Windows.Forms.SplitContainer splitTools;
    }
}

