namespace Server
{
	partial class ServerForm
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
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.RichTextBox = new System.Windows.Forms.RichTextBox();
			this.SaveButton = new System.Windows.Forms.Button();
			this.OpenButton = new System.Windows.Forms.Button();
			this.LocationLabel = new System.Windows.Forms.Label();
			this.ZoomLabel = new System.Windows.Forms.Label();
			this.ZoomInButton = new System.Windows.Forms.Button();
			this.ZoomOutButton = new System.Windows.Forms.Button();
			this.ClearBoardButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// PictureBox
			// 
			this.PictureBox.Location = new System.Drawing.Point(100, 0);
			this.PictureBox.Name = "PictureBox";
			this.PictureBox.Size = new System.Drawing.Size(800, 600);
			this.PictureBox.TabIndex = 0;
			this.PictureBox.TabStop = false;
			this.PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_Paint);
			this.PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
			this.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseMove);
			this.PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseUp);
			// 
			// TextBox
			// 
			this.RichTextBox.BackColor = System.Drawing.Color.DarkBlue;
			this.RichTextBox.ForeColor = System.Drawing.Color.White;
			this.RichTextBox.Location = new System.Drawing.Point(0, 721);
			this.RichTextBox.Multiline = true;
			this.RichTextBox.Name = "TextBox";
			this.RichTextBox.ReadOnly = true;
			this.RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.RichTextBox.Size = new System.Drawing.Size(1000, 100);
			this.RichTextBox.TabIndex = 1;
			this.RichTextBox.TabStop = false;
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(867, 692);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(75, 23);
			this.SaveButton.TabIndex = 2;
			this.SaveButton.TabStop = false;
			this.SaveButton.Text = "Save";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseClick);
			// 
			// OpenButton
			// 
			this.OpenButton.Location = new System.Drawing.Point(867, 732);
			this.OpenButton.Name = "OpenButton";
			this.OpenButton.Size = new System.Drawing.Size(75, 23);
			this.OpenButton.TabIndex = 3;
			this.OpenButton.TabStop = false;
			this.OpenButton.Text = "Open";
			this.OpenButton.UseVisualStyleBackColor = true;
			this.OpenButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OpenButton_MouseClick);
			// 
			// LocationLabel
			// 
			this.LocationLabel.AutoSize = true;
			this.LocationLabel.Location = new System.Drawing.Point(683, 36);
			this.LocationLabel.Name = "LocationLabel";
			this.LocationLabel.Size = new System.Drawing.Size(91, 16);
			this.LocationLabel.TabIndex = 1;
			this.LocationLabel.Text = "Location label";
			// 
			// ZoomLabel
			// 
			this.ZoomLabel.AutoSize = true;
			this.ZoomLabel.Location = new System.Drawing.Point(686, 78);
			this.ZoomLabel.Name = "ZoomLabel";
			this.ZoomLabel.Size = new System.Drawing.Size(75, 16);
			this.ZoomLabel.TabIndex = 4;
			this.ZoomLabel.Text = "Zoom label";
			// 
			// ZoomInButton
			// 
			this.ZoomInButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.ZoomInButton.Location = new System.Drawing.Point(907, 534);
			this.ZoomInButton.Name = "ZoomInButton";
			this.ZoomInButton.Size = new System.Drawing.Size(75, 23);
			this.ZoomInButton.TabIndex = 5;
			this.ZoomInButton.TabStop = false;
			this.ZoomInButton.Text = "+";
			this.ZoomInButton.UseVisualStyleBackColor = true;
			this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
			// 
			// ZoomOutButton
			// 
			this.ZoomOutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.ZoomOutButton.Location = new System.Drawing.Point(907, 564);
			this.ZoomOutButton.Name = "ZoomOutButton";
			this.ZoomOutButton.Size = new System.Drawing.Size(75, 23);
			this.ZoomOutButton.TabIndex = 6;
			this.ZoomOutButton.TabStop = false;
			this.ZoomOutButton.Text = "-";
			this.ZoomOutButton.UseVisualStyleBackColor = true;
			this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
			// 
			// ClearBoardButton
			// 
			this.ClearBoardButton.Location = new System.Drawing.Point(867, 780);
			this.ClearBoardButton.Name = "ClearBoardButton";
			this.ClearBoardButton.Size = new System.Drawing.Size(75, 23);
			this.ClearBoardButton.TabIndex = 7;
			this.ClearBoardButton.TabStop = false;
			this.ClearBoardButton.Text = "Clear board";
			this.ClearBoardButton.UseVisualStyleBackColor = true;
			this.ClearBoardButton.Click += new System.EventHandler(this.ClearBoardButton_Click);
			// 
			// ServerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(982, 828);
			this.Controls.Add(this.ClearBoardButton);
			this.Controls.Add(this.ZoomOutButton);
			this.Controls.Add(this.ZoomInButton);
			this.Controls.Add(this.ZoomLabel);
			this.Controls.Add(this.LocationLabel);
			this.Controls.Add(this.OpenButton);
			this.Controls.Add(this.SaveButton);
			this.Controls.Add(this.RichTextBox);
			this.Controls.Add(this.PictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Location = new System.Drawing.Point(100, 0);
			this.Name = "ServerForm";
			this.Text = "Server drawing board";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
			this.Load += new System.EventHandler(this.ServerForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox PictureBox;
		private System.Windows.Forms.RichTextBox RichTextBox;
		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.Button OpenButton;
		private System.Windows.Forms.Label LocationLabel;
		private System.Windows.Forms.Label ZoomLabel;
		private System.Windows.Forms.Button ZoomInButton;
		private System.Windows.Forms.Button ZoomOutButton;
		private System.Windows.Forms.Button ClearBoardButton;
	}
}

