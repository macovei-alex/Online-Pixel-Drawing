using System.Windows.Forms;

namespace Client
{
	partial class ClientLoginForm
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
			this.UsernameTextBox = new System.Windows.Forms.TextBox();
			this.PasswordTextBox = new System.Windows.Forms.TextBox();
			this.ConfirmButton = new System.Windows.Forms.Button();
			this.UsernameLabel = new System.Windows.Forms.Label();
			this.PasswordLabel = new System.Windows.Forms.Label();
			this.TogglePasswordVisibilityButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// UsernameTextBox
			// 
			this.UsernameTextBox.Location = new System.Drawing.Point(66, 35);
			this.UsernameTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.UsernameTextBox.Name = "UsernameTextBox";
			this.UsernameTextBox.Size = new System.Drawing.Size(124, 26);
			this.UsernameTextBox.TabIndex = 0;
			// 
			// PasswordTextBox
			// 
			this.PasswordTextBox.Location = new System.Drawing.Point(66, 95);
			this.PasswordTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.PasswordChar = '*';
			this.PasswordTextBox.Size = new System.Drawing.Size(124, 26);
			this.PasswordTextBox.TabIndex = 1;
			// 
			// ConfirmButton
			// 
			this.ConfirmButton.Location = new System.Drawing.Point(98, 166);
			this.ConfirmButton.Margin = new System.Windows.Forms.Padding(4);
			this.ConfirmButton.Name = "ConfirmButton";
			this.ConfirmButton.Size = new System.Drawing.Size(94, 29);
			this.ConfirmButton.TabIndex = 2;
			this.ConfirmButton.Text = "Confirm";
			this.ConfirmButton.UseVisualStyleBackColor = true;
			this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
			// 
			// UsernameLabel
			// 
			this.UsernameLabel.AutoSize = true;
			this.UsernameLabel.Location = new System.Drawing.Point(16, 41);
			this.UsernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.UsernameLabel.Name = "UsernameLabel";
			this.UsernameLabel.Size = new System.Drawing.Size(91, 20);
			this.UsernameLabel.TabIndex = 3;
			this.UsernameLabel.Text = "Username:";
			// 
			// PasswordLabel
			// 
			this.PasswordLabel.AutoSize = true;
			this.PasswordLabel.Location = new System.Drawing.Point(4, 99);
			this.PasswordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.PasswordLabel.Name = "PasswordLabel";
			this.PasswordLabel.Size = new System.Drawing.Size(88, 20);
			this.PasswordLabel.TabIndex = 4;
			this.PasswordLabel.Text = "Password:";
			// 
			// TogglePasswordVisibilityButton
			// 
			this.TogglePasswordVisibilityButton.BackColor = System.Drawing.SystemColors.Control;
			this.TogglePasswordVisibilityButton.FlatAppearance.BorderSize = 0;
			this.TogglePasswordVisibilityButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TogglePasswordVisibilityButton.Location = new System.Drawing.Point(197, 95);
			this.TogglePasswordVisibilityButton.Margin = new System.Windows.Forms.Padding(0);
			this.TogglePasswordVisibilityButton.Name = "TogglePasswordVisibilityButton";
			this.TogglePasswordVisibilityButton.Size = new System.Drawing.Size(33, 26);
			this.TogglePasswordVisibilityButton.TabIndex = 5;
			this.TogglePasswordVisibilityButton.TabStop = false;
			this.TogglePasswordVisibilityButton.UseVisualStyleBackColor = false;
			this.TogglePasswordVisibilityButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TogglePasswordVisibilityButton_MouseClick);
			// 
			// ClientLoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1000, 562);
			this.Controls.Add(this.TogglePasswordVisibilityButton);
			this.Controls.Add(this.PasswordLabel);
			this.Controls.Add(this.UsernameLabel);
			this.Controls.Add(this.ConfirmButton);
			this.Controls.Add(this.PasswordTextBox);
			this.Controls.Add(this.UsernameTextBox);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Location = new System.Drawing.Point(100, 100);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ClientLoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Login";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientLoginForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox UsernameTextBox;
		private System.Windows.Forms.TextBox PasswordTextBox;
		private System.Windows.Forms.Button ConfirmButton;
		private System.Windows.Forms.Label UsernameLabel;
		private System.Windows.Forms.Label PasswordLabel;
		private Button TogglePasswordVisibilityButton;
	}
}