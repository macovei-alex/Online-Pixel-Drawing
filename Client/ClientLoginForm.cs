using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace Client
{
	public partial class ClientLoginForm : Form
	{
		ClientMainForm mainForm;
		bool askForConfirmation;
		bool passwordHidden;

		public ClientLoginForm(ClientMainForm mainForm)
		{
			InitializeComponent();
			Size = new Size(
				Const.WIDTH3 + 2 * Const.MARGIN + 15,
				Const.HEIGHT3 + 2 * Const.MARGIN + 40);
			Location = new Point(
				mainForm.Location.X + mainForm.Width / 2 - Width / 2,
				mainForm.Location.Y + mainForm.Height / 2 - Height / 2);

			UsernameLabel.Location = new Point(
				Const.MARGIN + 20,
				Const.OFFSET);

			UsernameTextBox.Size = new Size(
				Const.LOGINFORM_TEXTBOX_WIDTH,
				UsernameTextBox.Height);
			UsernameTextBox.Location = new Point(
				UsernameLabel.Location.X + UsernameLabel.Width + 10,
				UsernameLabel.Location.Y);

			PasswordLabel.Location = new Point(
				UsernameLabel.Location.X,
				UsernameLabel.Location.Y + Const.OFFSET);

			PasswordTextBox.Location = new Point(
				UsernameTextBox.Location.X,
				PasswordLabel.Location.Y);
			PasswordTextBox.Size = new Size(
				UsernameTextBox.Width,
				UsernameTextBox.Height);

			ConfirmButton.Location = new Point(
				Width / 2 - ConfirmButton.Width / 2,
				PasswordTextBox.Location.Y + PasswordTextBox.Height + 10);

			Image eyeOpenImage = Image.FromFile("Eye open.png");
			TogglePasswordVisibilityButton.Size = new Size(
				eyeOpenImage.Width + 1,
				eyeOpenImage.Height);
			TogglePasswordVisibilityButton.Location = new Point(
				PasswordTextBox.Location.X + PasswordTextBox.Width + Const.MARGIN,
				PasswordTextBox.Location.Y + PasswordTextBox.Height / 2 - TogglePasswordVisibilityButton.Height / 2);
			TogglePasswordVisibilityButton.Image = eyeOpenImage;

			UsernameTextBox.MaxLength = Const.MAX_STR_COUNT;
			PasswordTextBox.MaxLength = Const.MAX_STR_COUNT;

			this.mainForm = mainForm;
			askForConfirmation = true;
			passwordHidden = true;
		}

		private void ClientLoginForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (askForConfirmation)
			{
				DialogResult questionResult = MessageBox.Show($"Do you want to stop the log in process?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (questionResult == DialogResult.Yes)
				{
					mainForm.username = string.Empty;
					mainForm.password = string.Empty;
				}
				else
				{
					e.Cancel = true;
				}
				return;
			}
			mainForm.username = UsernameTextBox.Text;
			mainForm.password = PasswordTextBox.Text;
		}

		private void ConfirmButton_Click(object sender, EventArgs e)
		{
			askForConfirmation = false;
			Close();
		}

		private void TogglePasswordVisibilityButton_MouseClick(object sender, MouseEventArgs e)
		{
			if (passwordHidden)
			{
				PasswordTextBox.PasswordChar = '\0';
				TogglePasswordVisibilityButton.Image = Image.FromFile("Eye closed.png");
			}
			else
			{
				PasswordTextBox.PasswordChar = '*';
				TogglePasswordVisibilityButton.Image = Image.FromFile("Eye open.png");
			}

			PasswordTextBox.Focus();
			passwordHidden = !passwordHidden;
		}
	}
}
