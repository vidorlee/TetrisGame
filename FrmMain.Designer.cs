namespace TetrisGame
{
    partial class FrmMain
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
            tetrisPanel = new TetrisPanel();
            propertyGrid1 = new PropertyGrid();
            splitter1 = new Splitter();
            btnExit = new Button();
            SuspendLayout();
            // 
            // tetrisPanel
            // 
            tetrisPanel.BackColor = Color.Black;
            tetrisPanel.Cols = 12;
            tetrisPanel.DisplayMode = DisplayMode.Horizontal;
            tetrisPanel.Dock = DockStyle.Fill;
            tetrisPanel.Level = 1;
            tetrisPanel.Location = new Point(0, 0);
            tetrisPanel.MinimumSize = new Size(480, 240);
            tetrisPanel.Name = "tetrisPanel";
            tetrisPanel.Padding = new Padding(3);
            tetrisPanel.Rows = 21;
            tetrisPanel.Size = new Size(491, 480);
            tetrisPanel.Speed = 1;
            tetrisPanel.TabIndex = 0;
            tetrisPanel.GameOver += tetrisPanel_RowFillFull;
            tetrisPanel.AutoSizeChanged += tetrisPanel_GameOver;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Right;
            propertyGrid1.Location = new Point(496, 0);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.SelectedObject = tetrisPanel;
            propertyGrid1.Size = new Size(304, 480);
            propertyGrid1.TabIndex = 1;
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Right;
            splitter1.Location = new Point(491, 0);
            splitter1.MinExtra = 500;
            splitter1.MinSize = 300;
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(5, 480);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackColor = Color.FromArgb(255, 192, 192);
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.MouseDownBackColor = Color.Red;
            btnExit.FlatAppearance.MouseOverBackColor = Color.Yellow;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Location = new Point(768, 0);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(32, 25);
            btnExit.TabIndex = 3;
            btnExit.Text = "X";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += buttonExit_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnExit;
            ClientSize = new Size(800, 480);
            Controls.Add(btnExit);
            Controls.Add(tetrisPanel);
            Controls.Add(splitter1);
            Controls.Add(propertyGrid1);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            Name = "FrmMain";
            Text = "FrmMain";
            WindowState = FormWindowState.Maximized;
            Load += FrmMain_Load;
            KeyDown += FrmMain_KeyDown;
            KeyUp += FrmMain_KeyUp;
            ResumeLayout(false);
        }

        #endregion

        private TetrisPanel tetrisPanel;
        private PropertyGrid propertyGrid1;
        private Splitter splitter1;
        private Button btnExit;
    }
}