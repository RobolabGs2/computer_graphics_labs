namespace Lab02
{
    partial class MainForm
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
            this.chooseImageButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainPictureDisplay = new System.Windows.Forms.PictureBox();
            this.mainPanel = new System.Windows.Forms.GroupBox();
            this.taskButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.solutionPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureDisplay)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // chooseImageButton
            // 
            this.chooseImageButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.chooseImageButton.Location = new System.Drawing.Point(3, 586);
            this.chooseImageButton.Name = "chooseImageButton";
            this.chooseImageButton.Size = new System.Drawing.Size(523, 82);
            this.chooseImageButton.TabIndex = 0;
            this.chooseImageButton.Text = "Выбрать картинку";
            this.chooseImageButton.UseVisualStyleBackColor = true;
            this.chooseImageButton.Click += new System.EventHandler(this.chooseImageButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // mainPictureDisplay
            // 
            this.mainPictureDisplay.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPictureDisplay.Location = new System.Drawing.Point(3, 22);
            this.mainPictureDisplay.Name = "mainPictureDisplay";
            this.mainPictureDisplay.Size = new System.Drawing.Size(523, 564);
            this.mainPictureDisplay.TabIndex = 2;
            this.mainPictureDisplay.TabStop = false;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.taskButtonsPanel);
            this.mainPanel.Controls.Add(this.chooseImageButton);
            this.mainPanel.Controls.Add(this.mainPictureDisplay);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(529, 1230);
            this.mainPanel.TabIndex = 3;
            this.mainPanel.TabStop = false;
            // 
            // taskButtonsPanel
            // 
            this.taskButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.taskButtonsPanel.Location = new System.Drawing.Point(3, 668);
            this.taskButtonsPanel.Name = "taskButtonsPanel";
            this.taskButtonsPanel.Size = new System.Drawing.Size(523, 535);
            this.taskButtonsPanel.TabIndex = 3;
            // 
            // solutionPanel
            // 
            this.solutionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.solutionPanel.BackColor = System.Drawing.SystemColors.Control;
            this.solutionPanel.Location = new System.Drawing.Point(532, 12);
            this.solutionPanel.Name = "solutionPanel";
            this.solutionPanel.Size = new System.Drawing.Size(1395, 1206);
            this.solutionPanel.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1927, 1230);
            this.Controls.Add(this.solutionPanel);
            this.Controls.Add(this.mainPanel);
            this.Name = "MainForm";
            this.Text = "Лабораторная №2";
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureDisplay)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button chooseImageButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox mainPictureDisplay;
        private System.Windows.Forms.GroupBox mainPanel;
        private System.Windows.Forms.FlowLayoutPanel taskButtonsPanel;
        private System.Windows.Forms.FlowLayoutPanel solutionPanel;
    }
}

