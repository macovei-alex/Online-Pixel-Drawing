namespace Client
{
	partial class ClientMainForm
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
			this.components = new System.ComponentModel.Container();
			this.DrawingBoard = new System.Windows.Forms.PictureBox();
			this.LocationLabel = new System.Windows.Forms.Label();
			this.ZoomLabel = new System.Windows.Forms.Label();
			this.ZoomInButton = new System.Windows.Forms.Button();
			this.ZoomOutButton = new System.Windows.Forms.Button();
			this.SaveButton = new System.Windows.Forms.Button();
			this.OpenButton = new System.Windows.Forms.Button();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.ConnectionStatusLabel = new System.Windows.Forms.Label();
			this.ClearBoardButton = new System.Windows.Forms.Button();
			this.ColorPicker = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.DrawingBoard)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ColorPicker)).BeginInit();
			this.SuspendLayout();
			// 
			// DrawingBoard
			// 
			this.DrawingBoard.Location = new System.Drawing.Point(155, 12);
			this.DrawingBoard.Name = "DrawingBoard";
			this.DrawingBoard.Size = new System.Drawing.Size(876, 490);
			this.DrawingBoard.TabIndex = 0;
			this.DrawingBoard.TabStop = false;
			this.DrawingBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_Paint);
			this.DrawingBoard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseClick);
			this.DrawingBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
			this.DrawingBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseMove);
			this.DrawingBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseUp);
			// 
			// LocationLabel
			// 
			this.LocationLabel.AutoSize = true;
			this.LocationLabel.Location = new System.Drawing.Point(673, 40);
			this.LocationLabel.Name = "LocationLabel";
			this.LocationLabel.Size = new System.Drawing.Size(91, 16);
			this.LocationLabel.TabIndex = 1;
			this.LocationLabel.Text = "Location label";
			// 
			// ZoomLabel
			// 
			this.ZoomLabel.AutoSize = true;
			this.ZoomLabel.Location = new System.Drawing.Point(686, 56);
			this.ZoomLabel.Name = "ZoomLabel";
			this.ZoomLabel.Size = new System.Drawing.Size(75, 16);
			this.ZoomLabel.TabIndex = 2;
			this.ZoomLabel.Text = "Zoom label";
			// 
			// ZoomInButton
			// 
			this.ZoomInButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.ZoomInButton.Location = new System.Drawing.Point(686, 76);
			this.ZoomInButton.Name = "ZoomInButton";
			this.ZoomInButton.Size = new System.Drawing.Size(25, 23);
			this.ZoomInButton.TabIndex = 3;
			this.ZoomInButton.TabStop = false;
			this.ZoomInButton.Text = "+";
			this.ZoomInButton.UseVisualStyleBackColor = true;
			this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
			// 
			// ZoomOutButton
			// 
			this.ZoomOutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.ZoomOutButton.Location = new System.Drawing.Point(726, 76);
			this.ZoomOutButton.Name = "ZoomOutButton";
			this.ZoomOutButton.Size = new System.Drawing.Size(25, 23);
			this.ZoomOutButton.TabIndex = 4;
			this.ZoomOutButton.TabStop = false;
			this.ZoomOutButton.Text = "-";
			this.ZoomOutButton.UseVisualStyleBackColor = true;
			this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(676, 105);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(75, 23);
			this.SaveButton.TabIndex = 5;
			this.SaveButton.TabStop = false;
			this.SaveButton.Text = "Save";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// OpenButton
			// 
			this.OpenButton.Location = new System.Drawing.Point(676, 134);
			this.OpenButton.Name = "OpenButton";
			this.OpenButton.Size = new System.Drawing.Size(75, 23);
			this.OpenButton.TabIndex = 6;
			this.OpenButton.TabStop = false;
			this.OpenButton.Text = "Open";
			this.OpenButton.UseVisualStyleBackColor = true;
			this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
			// 
			// ConnectButton
			// 
			this.ConnectButton.Location = new System.Drawing.Point(563, 477);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(75, 23);
			this.ConnectButton.TabIndex = 7;
			this.ConnectButton.TabStop = false;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.Location = new System.Drawing.Point(676, 477);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(75, 23);
			this.DisconnectButton.TabIndex = 8;
			this.DisconnectButton.TabStop = false;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
			// 
			// ConnectionStatusLabel
			// 
			this.ConnectionStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectionStatusLabel.Location = new System.Drawing.Point(580, 518);
			this.ConnectionStatusLabel.Name = "ConnectionStatusLabel";
			this.ConnectionStatusLabel.Size = new System.Drawing.Size(100, 23);
			this.ConnectionStatusLabel.TabIndex = 9;
			this.ConnectionStatusLabel.Text = "Status:";
			this.ConnectionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ClearBoardButton
			// 
			this.ClearBoardButton.Location = new System.Drawing.Point(676, 163);
			this.ClearBoardButton.Name = "ClearBoardButton";
			this.ClearBoardButton.Size = new System.Drawing.Size(75, 23);
			this.ClearBoardButton.TabIndex = 0;
			this.ClearBoardButton.TabStop = false;
			this.ClearBoardButton.Text = "Clear board";
			this.ClearBoardButton.UseVisualStyleBackColor = true;
			this.ClearBoardButton.Click += new System.EventHandler(this.ClearBoardButton_Click);
			// 
			// ColorPicker
			// 
			this.ColorPicker.Location = new System.Drawing.Point(35, 12);
			this.ColorPicker.Name = "ColorPicker";
			this.ColorPicker.Size = new System.Drawing.Size(100, 526);
			this.ColorPicker.TabIndex = 11;
			this.ColorPicker.TabStop = false;
			this.ColorPicker.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorPicker_Paint);
			this.ColorPicker.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorPicker_MouseClick);
			// 
			// ClientMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 553);
			this.Controls.Add(this.ColorPicker);
			this.Controls.Add(this.ClearBoardButton);
			this.Controls.Add(this.ConnectionStatusLabel);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.OpenButton);
			this.Controls.Add(this.SaveButton);
			this.Controls.Add(this.ZoomOutButton);
			this.Controls.Add(this.ZoomInButton);
			this.Controls.Add(this.ZoomLabel);
			this.Controls.Add(this.LocationLabel);
			this.Controls.Add(this.DrawingBoard);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(100, 100);
			this.Name = "ClientMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Drawing Board";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientMainForm_FormClosing);
			this.Load += new System.EventHandler(this.ClientMainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.DrawingBoard)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ColorPicker)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox DrawingBoard;
		private System.Windows.Forms.Label LocationLabel;
		private System.Windows.Forms.Label ZoomLabel;
		private System.Windows.Forms.Button ZoomInButton;
		private System.Windows.Forms.Button ZoomOutButton;
		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.Button OpenButton;
		private System.Windows.Forms.Button ConnectButton;
		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Label ConnectionStatusLabel;
		private System.Windows.Forms.Button ClearBoardButton;
		private System.Windows.Forms.PictureBox ColorPicker;
	}
}

