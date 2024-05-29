using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using Utility;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Client
{
	public partial class ClientMainForm : Form
	{
		int squareSize;
		float zoomFactor;
		Point translation;
		bool isDragging = false;
		bool mousePressed = false;

		ClientLoginForm loginForm;
		bool loginFormIsOpen;
		public string username, password;

		Point lastMouseAbsLocation;
		Point selectedColorPos;

		Color selectedColor;
		Color[][] board;
		List<Color> colorList;
		Dictionary<Color, Brush> brushDictionary;
		Pen linePen, highlightPen;

		BidirectionalConnection connection;
		Thread receivingThread;
		bool formClosed, serverClosed, isConnected;

		Tuple<Size, Point> DrawingBoardDefaultCoords, DrawingBoardFullScreenCoords;
		bool fullScreenMode;

		public ClientMainForm()
		{
			InitializeComponent();
			DrawingBoard.MouseWheel += new MouseEventHandler(PictureBox_MouseWheel);
			KeyDown += new KeyEventHandler(ClientMainForm_KeyDown);

			Location = new Point(
				Screen.FromHandle(Handle).Bounds.X + 100,
				Screen.FromHandle(Handle).Bounds.Y + 100);
			Size = new Size(
				Const.WIDTH + Const.WIDTH2 + 3 * Const.MARGIN + Const.LINE_WIDTH + 15,
				Const.HEIGHT + 2 * Const.MARGIN + Const.LINE_WIDTH + 40);

			ColorPicker.Location = new Point(
				Const.MARGIN,
				Const.MARGIN);
			ColorPicker.Size = new Size(
				Const.WIDTH2,
				Const.HEIGHT2);

			DrawingBoardDefaultCoords = new Tuple<Size, Point>(
				new Size(
					Const.WIDTH + Const.LINE_WIDTH,
					Const.HEIGHT + Const.LINE_WIDTH),
				new Point(
					Const.WIDTH2 + 2 * Const.MARGIN,
					Const.MARGIN));
			DrawingBoardFullScreenCoords = new Tuple<Size, Point>(
				new Size(
					Width - 2 * Const.MARGIN,
					Height - 2 * Const.MARGIN),
				new Point(
					Const.MARGIN,
					Const.MARGIN));

			DrawingBoard.Size = DrawingBoardDefaultCoords.Item1;
			DrawingBoard.Location = DrawingBoardDefaultCoords.Item2;

			LocationLabel.Location = new Point(
				DrawingBoard.Location.X + DrawingBoard.Width - 70,
				DrawingBoard.Location.Y + 10);

			ZoomLabel.Location = new Point(
				LocationLabel.Location.X,
				LocationLabel.Location.Y + ZoomLabel.Height);

			ZoomInButton.Location = new Point(
				ZoomLabel.Location.X,
				ZoomLabel.Location.Y + ZoomLabel.Height);
			ZoomInButton.Size = new Size(
				Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);

			ZoomOutButton.Location = new Point(
				ZoomLabel.Location.X + ZoomInButton.Width,
				ZoomLabel.Location.Y + ZoomLabel.Height);
			ZoomOutButton.Size = new Size(
				Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);

			ConnectButton.Location = new Point(
				DrawingBoard.Location.X + DrawingBoard.Width - 160,
				DrawingBoard.Location.Y + DrawingBoard.Height - 50);
			ConnectButton.Size = new Size(
				Const.CONNECT_BUTTON_WIDTH,
				Const.CONNECT_BUTTON_HEIGHT);

			DisconnectButton.Location = new Point(
				ConnectButton.Location.X + ConnectButton.Width,
				ConnectButton.Location.Y);
			DisconnectButton.Size = new Size(
				Const.CONNECT_BUTTON_WIDTH,
				Const.CONNECT_BUTTON_HEIGHT);

			ConnectionStatusLabel.Location = new Point(
				ConnectButton.Location.X,
				ConnectButton.Location.Y + ConnectButton.Height);
			ConnectionStatusLabel.Size = new Size(
				2 * Const.CONNECT_BUTTON_WIDTH,
				Const.CONNECT_BUTTON_HEIGHT);

			SaveButton.Location = new Point(
				ZoomInButton.Location.X,
				ZoomInButton.Location.Y + ZoomInButton.Height);
			SaveButton.Size = new Size(
				2 * Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);

			OpenButton.Location = new Point(
				SaveButton.Location.X,
				SaveButton.Location.Y + SaveButton.Height);
			OpenButton.Size = new Size(
				2 * Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);

			ClearBoardButton.Location = new Point(
				OpenButton.Location.X,
				OpenButton.Location.Y + OpenButton.Height);
			ClearBoardButton.Size = new Size(
				2 * Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);
		}

		private void ClientMainForm_Load(object sender, EventArgs e)
		{
			Func.DefineColors(ref colorList, ref brushDictionary);
			board = Func.CreateBoard();

			linePen = new Pen(Const.LINE_COLOR, Const.LINE_WIDTH);
			highlightPen = new Pen(Const.LINE_COLOR, 2);
			zoomFactor = 1.0f;
			ZoomLabel.Text = (100 * zoomFactor).ToString() + "%";
			squareSize = (int)(Const.SQUARE_SIZE * zoomFactor);
			translation = new Point(0, 0);
			lastMouseAbsLocation = new Point(0, 0);
			LocationLabel.Text = "0, 0";

			lastMouseAbsLocation = new Point(0, 0);
			selectedColor = Const.COLOR_WHITE;
			selectedColorPos = new Point(0, 0);
			fullScreenMode = false;

			loginFormIsOpen = false;
			username = string.Empty;
			password = string.Empty;

			formClosed = false;
			serverClosed = false;
			isConnected = false;
			ConnectionStatusLabel_Set();
			connection = null;
			receivingThread = null;

			LocationLabel.Show();

			if (isConnected)
			{
				ConnectToServer();
			}
		}

		private void ClientMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DialogResult questionResult = MessageBox.Show("Do you want to save the current image?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (questionResult == DialogResult.Yes)
			{
				SaveFileDialog saveFile = (SaveFileDialog)CreateDialog(FileDialogMode.save);
				if (saveFile.ShowDialog(this) == DialogResult.OK)
					SaveImage(saveFile.FileName);
				else
					questionResult = DialogResult.Cancel;
			}
			if (questionResult == DialogResult.Cancel)
			{
				e.Cancel = true;
				return;
			}

			formClosed = true;
			if (connection != null)
				DisconnectFromServer();
		}

		private bool ConnectToServer()
		{
			if (isConnected)
				return false;

			if (username == string.Empty)
			{
				loginFormIsOpen = true;
				loginForm = new ClientLoginForm(this);

				ManualResetEvent signal = new ManualResetEvent(false);
				loginForm.FormClosed += (s, args) =>
				{
					loginFormIsOpen = false;
					signal.Set();
				};

				Thread loginFormThread = new Thread(() => Application.Run(loginForm));
				loginFormThread.Start();

				signal.WaitOne();

				if (username == string.Empty)
					return false;
			}

			try
			{
				receivingThread = null;
				connection = new BidirectionalConnection(new TcpClient(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.ClientToServerPort), new TcpClient(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.ServerToClientPort));
				isConnected = true;
				Invoke(new Action(() => ConnectionStatusLabel_Set()));
				serverClosed = false;

				Func.SendString(connection.ClientToServer, username);
				Func.SendString(connection.ClientToServer, password);
				string returnMessage = Func.ReceiveString(connection.ServerToClient);
				if (returnMessage == Const.CONNECTION_ACCEPTED)
				{
					receivingThread = new Thread(ReceiveHandler);
					receivingThread.Start();
					return true;
				}
				else if (returnMessage == Const.CONNECTION_DENIED)
				{
					username = string.Empty;
					throw new Exception("The server denied your request");
				}
			}
			catch (Exception ex)
			{
				isConnected = false;
				Invoke(new Action(() => ConnectionStatusLabel_Set()));
				receivingThread?.Join();
				receivingThread = null;
				connection?.Close();
				connection = null;
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private void DisconnectFromServer(ManualResetEvent eventSignal = null)
		{
			if (!isConnected)
				return;

			if (!serverClosed)
			{
				try
				{
					Func.SendCommandCode(connection.ClientToServer, CommandCode.Close);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			else
				MessageBox.Show("The server has been closed. You have been disconnected.");

			isConnected = false;
			Invoke(new Action(() => ConnectionStatusLabel_Set()));
			if (eventSignal == null)
			{
				receivingThread?.Join();
				receivingThread = null;
			}
			connection?.Close();
			connection = null;
			eventSignal?.Set();
		}

		private void ReceiveHandler()
		{
			while (isConnected && !formClosed)
			{
				if (connection.ServerToClient.GetStream().DataAvailable)
				{
					CommandCode code = Func.ReceiveCommandCode(connection.ServerToClient);
					switch (code)
					{
						case CommandCode.SendObject:
							TransferObject obj = Func.ReceiveObject(connection.ServerToClient);
							board[obj.Row][obj.Column] = obj.Color;
							Invoke(new Action(() => Refresh()));
							break;

						case CommandCode.LoadImage:
							board = Func.CreateBoard();
							OpenImageFromServer();
							Invoke(new Action(() => Refresh()));
							break;

						case CommandCode.Close:
							serverClosed = true;
							ManualResetEvent clientDisconnectedSignal = new ManualResetEvent(false);
							Invoke(new Action(() => DisconnectFromServer(clientDisconnectedSignal)));
							clientDisconnectedSignal.WaitOne();
							clientDisconnectedSignal.Close();
							break;

						case CommandCode.ClearBoard:
							board = Func.CreateBoard();
							Invoke(new Action(() => Refresh()));
							break;

						case CommandCode.SendString:
							string s = Func.ReceiveString(connection.ServerToClient);
							MessageBox.Show(s);
							break;

						default:
							MessageBox.Show($"Code received: {code}");
							break;
					}
				}

				Thread.Sleep(1000 / Const.UPDATE_CHECK_RATE);
			}
		}

		private void OpenImageFromServer()
		{
			TransferObject obj = Func.ReceiveObject(connection.ServerToClient);
			while (!obj.Equals(Const.CONTROL_OBJECT_NULL))
			{
				board[obj.Row][obj.Column] = obj.Color;
				obj = Func.ReceiveObject(connection.ServerToClient);
			}
		}

		private void PictureBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (isDragging)
				return;
			if (loginFormIsOpen)
				return;
			if (zoomFactor < Const.ZOOM_MIN_TO_HIGHLIGHT_CURSOR)
				return;

			Point absolutePos = new Point(e.X + translation.X, e.Y + translation.Y);
			int selectedRow = absolutePos.Y / squareSize;
			int selectedCol = absolutePos.X / squareSize;
			if (0 > selectedRow || selectedRow >= Const.SQUARE_ROWS
				|| 0 > selectedCol || selectedCol >= Const.SQUARE_COLS)
				return;

			if (e.Button == MouseButtons.Left)
			{
				board[selectedRow][selectedCol] = selectedColor;
			}
			else if (e.Button == MouseButtons.Right)
			{
				board[selectedRow][selectedCol] = Const.COLOR_WHITE;
			}

			Refresh();

			if (isConnected)
			{
				TransferObject obj = new TransferObject(selectedRow, selectedCol, selectedColor);
				try
				{
					Func.SendCommandCode(connection.ClientToServer, CommandCode.SendObject);
					Func.SendObject(connection.ClientToServer, obj);
				}
				catch
				{
					MessageBox.Show("Could not send the message to the server");
					serverClosed = true;
					DisconnectFromServer();
				}
			}
		}

		private void PictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			Point newMouseAbsLocation = new Point(e.X + translation.X, e.Y + translation.Y);
			if (mousePressed)
			{
				isDragging = true;
				int dx = newMouseAbsLocation.X - lastMouseAbsLocation.X;
				int dy = newMouseAbsLocation.Y - lastMouseAbsLocation.Y;
				translation = new Point(translation.X - dx, translation.Y - dy);
				Refresh();
			}
			else
			{
				if (!loginFormIsOpen)
				{
					if (newMouseAbsLocation.X / squareSize != lastMouseAbsLocation.X / squareSize
					|| newMouseAbsLocation.Y / squareSize != lastMouseAbsLocation.Y / squareSize)
					{
						lastMouseAbsLocation = newMouseAbsLocation;
						Refresh();
					}
					else
					{
						lastMouseAbsLocation = newMouseAbsLocation;
						LocationLabel.Text =
							(lastMouseAbsLocation.X / squareSize).ToString()
							+ ", "
							+ (lastMouseAbsLocation.Y / squareSize).ToString();
					}
				}
			}
		}

		private void PictureBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.TranslateTransform(-translation.X, -translation.Y);

			int firstRow = Math.Max(0, translation.Y / squareSize);
			int firstCol = Math.Max(0, translation.X / squareSize);
			int lastRow = Math.Min(firstRow + DrawingBoard.Height / squareSize + 2, Const.SQUARE_ROWS);
			int lastCol = Math.Min(firstCol + DrawingBoard.Width / squareSize + 2, Const.SQUARE_COLS);
			Brush fillBrush;
			for (int i = firstRow; i < lastRow; i++)
			{
				for (int j = firstCol; j < lastCol; j++)
				{
					fillBrush = brushDictionary[board[i][j]];
					e.Graphics.FillRectangle(fillBrush, new Rectangle(j * squareSize, i * squareSize, squareSize, squareSize));
				}
			}
			DrawLines(e.Graphics);
			if (zoomFactor >= Const.ZOOM_MIN_TO_HIGHLIGHT_CURSOR)
				HighlightMousePosition(e.Graphics);
		}

		private void DrawLines(Graphics g)
		{
			g.DrawLine(linePen, 0, 0, 0, Const.SQUARE_ROWS * squareSize);
			g.DrawLine(linePen, 0, 0, Const.SQUARE_COLS * squareSize, 0);
			g.DrawLine(linePen, 0, Const.SQUARE_ROWS * squareSize, Const.SQUARE_COLS * squareSize, Const.SQUARE_ROWS * squareSize);
			g.DrawLine(linePen, Const.SQUARE_COLS * squareSize, 0, Const.SQUARE_COLS * squareSize, Const.SQUARE_ROWS * squareSize);

		}

		private void HighlightMousePosition(Graphics g)
		{
			if (0 > lastMouseAbsLocation.X || lastMouseAbsLocation.X >= squareSize * Const.SQUARE_COLS)
				return;
			if (0 > lastMouseAbsLocation.Y || lastMouseAbsLocation.Y >= squareSize * Const.SQUARE_ROWS)
				return;

			g.DrawRectangle(highlightPen, lastMouseAbsLocation.X / squareSize * squareSize, lastMouseAbsLocation.Y / squareSize * squareSize, squareSize, squareSize);
		}

		private void PictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			mousePressed = true;
		}

		private void PictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			isDragging = false;
			mousePressed = false;
		}

		private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
		{
			float zoomFactorIncrement = 0;
			if (e.Delta > 0)
				zoomFactorIncrement = 0.1f;
			else if (e.Delta < 0)
				zoomFactorIncrement = -0.1f;

			ApplyZoom(lastMouseAbsLocation, zoomFactorIncrement);
		}

		private void ZoomInButton_Click(object sender, EventArgs e)
		{
			ApplyZoom(new Point(
				(int)(translation.X + Const.WIDTH / 2 * zoomFactor),
				(int)(translation.Y + Const.HEIGHT / 2 * zoomFactor)),
				0.1f);
		}

		private void ZoomOutButton_Click(object sender, EventArgs e)
		{
			ApplyZoom(new Point(
				(int)(translation.X + Const.WIDTH / 2 * zoomFactor),
				(int)(translation.Y + Const.HEIGHT / 2 * zoomFactor)),
				-0.1f);
		}

		private void ApplyZoom(Point focusPointAbsCoords, float zoomFactorIncrement)
		{
			bool lastMousePosUpdated = false;
			if (focusPointAbsCoords == lastMouseAbsLocation)
				lastMousePosUpdated = true;

			if (Const.ZOOM_MIN * 100 > Math.Round((zoomFactor + zoomFactorIncrement) * 100)
				|| Const.ZOOM_MAX * 100 < Math.Round((zoomFactor + zoomFactorIncrement) * 100))
				return;

			float lastZoom = zoomFactor;
			zoomFactor += zoomFactorIncrement;

			int dx, dy;
			dx = focusPointAbsCoords.X - translation.X;
			dy = focusPointAbsCoords.Y - translation.Y;

			focusPointAbsCoords.X = (int)(focusPointAbsCoords.X * zoomFactor / lastZoom);
			focusPointAbsCoords.Y = (int)(focusPointAbsCoords.Y * zoomFactor / lastZoom);

			translation.X = focusPointAbsCoords.X - dx;
			translation.Y = focusPointAbsCoords.Y - dy;

			squareSize = (int)(Const.SQUARE_SIZE * zoomFactor);
			if (lastMousePosUpdated)
			{
				lastMouseAbsLocation = focusPointAbsCoords;
			}

			ZoomLabel.Text = Math.Round(100 * zoomFactor).ToString() + "%";
			LocationLabel.Text =
						Math.Round(lastMouseAbsLocation.X / zoomFactor).ToString()
						+ ", "
						+ Math.Round(lastMouseAbsLocation.Y / zoomFactor).ToString();
			Refresh();
		}

		private void ConnectButton_Click(object sender, EventArgs e)
		{
			if (isConnected)
				return;
			if (loginFormIsOpen)
				return;

			DialogResult questionResult = MessageBox.Show("Do you want to save the current image?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (questionResult == DialogResult.Yes)
			{
				SaveFileDialog saveFile = (SaveFileDialog)CreateDialog(FileDialogMode.save);
				if (saveFile.ShowDialog(this) == DialogResult.OK)
					SaveImage(saveFile.FileName);
				else
					questionResult = DialogResult.Cancel;
			}
			if (questionResult != DialogResult.Cancel)
				ConnectToServer();
		}

		private void DisconnectButton_Click(object sender, EventArgs e)
		{
			if (loginFormIsOpen)
				return;

			if (isConnected)
				DisconnectFromServer();
		}

		private void ConnectionStatusLabel_Set()
		{
			if (isConnected)
			{
				ConnectionStatusLabel.Text = "Status: Connected";
				ConnectionStatusLabel.BackColor = Color.Green;
			}
			else
			{
				ConnectionStatusLabel.Text = "Status: Disconnected";
				ConnectionStatusLabel.BackColor = Color.Red;
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = (SaveFileDialog)CreateDialog(FileDialogMode.save);
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				SaveImage(saveFileDialog.FileName);
		}

		private void OpenButton_Click(object sender, EventArgs e)
		{
			if (isConnected)
				return;

			OpenFileDialog openFileDialog = (OpenFileDialog)CreateDialog(FileDialogMode.open);
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			OpenImageFromFile(openFileDialog.FileName);
		}

		private FileDialog CreateDialog(FileDialogMode mode)
		{
			if (mode == FileDialogMode.save)
				return new SaveFileDialog
				{
					InitialDirectory = Properties.Settings.Default.SaveDirectoryPath,
					Filter = "Text Files(*.txt) | *.txt",
					DefaultExt = "txt",
				};
			else if (mode == FileDialogMode.open)
				return new OpenFileDialog
				{
					InitialDirectory = Properties.Settings.Default.SaveDirectoryPath,
					Filter = "Text Files(*.txt) | *.txt",
					DefaultExt = "txt",
				};
			return null;
		}

		private void SaveImage(string saveFilePath)
		{
			using (StreamWriter writer = new StreamWriter(saveFilePath, false))
			{
				for (int i = 0; i < Const.SQUARE_ROWS; i++)
				{
					for (int j = 0; j < Const.SQUARE_COLS; j++)
					{
						if (board[i][j] != Const.COLOR_WHITE)
							writer.WriteLine(new TransferObject(i, j, board[i][j]).ToString());
					}
				}
			}
		}

		private void OpenImageFromFile(string fileName)
		{
			board = Func.CreateBoard();
			using (StreamReader reader = new StreamReader(fileName))
			{
				while (!reader.EndOfStream)
				{
					string objString = reader.ReadLine();
					TransferObject obj = TransferObject.StringToObject(objString);
					board[obj.Row][obj.Column] = obj.Color;
				}
			}
		}

		private void ClearBoardButton_Click(object sender, EventArgs e)
		{
			if (isConnected)
				return;

			DialogResult questionResult = MessageBox.Show("Do you want to save the current image?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (questionResult == DialogResult.Yes)
			{
				SaveFileDialog saveFile = (SaveFileDialog)CreateDialog(FileDialogMode.save);
				if (saveFile.ShowDialog(this) == DialogResult.OK)
					SaveImage(saveFile.FileName);
				else
					questionResult = DialogResult.Cancel;
			}
			if (questionResult != DialogResult.Cancel)
				board = Func.CreateBoard();
		}

		private void ColorPicker_Paint(object sender, PaintEventArgs e)
		{
			DrawColors(e.Graphics);
			e.Graphics.DrawRectangle(highlightPen, 1, 1, ColorPicker.Width - 2, ColorPicker.Height - 2);
			e.Graphics.DrawRectangle(highlightPen, selectedColorPos.X, selectedColorPos.Y, Const.COLOR_SIZE, Const.COLOR_SIZE);
		}

		private void DrawColors(Graphics g)
		{
			Brush fill;
			int columns = Const.WIDTH2 / Const.COLOR_SIZE;
			for (int i = 0; i < colorList.Count(); i++)
			{
				fill = new SolidBrush(colorList[i]);
				g.FillRectangle(fill, new Rectangle(
					i % columns * Const.COLOR_SIZE,
					i / columns * Const.COLOR_SIZE,
					Const.COLOR_SIZE,
					Const.COLOR_SIZE));
			}
		}

		private void ColorPicker_MouseClick(object sender, MouseEventArgs e)
		{
			int row = e.Y / Const.COLOR_SIZE, col = e.X / Const.COLOR_SIZE;
			int index = row * (Const.WIDTH2 / Const.COLOR_SIZE) + col;
			if (index < colorList.Count())
			{
				selectedColor = colorList[index];
				selectedColorPos = new Point(col * Const.COLOR_SIZE, row * Const.COLOR_SIZE);
				Refresh();
			}
		}

		private void ClientMainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F)
			{
				fullScreenMode = !fullScreenMode;
				if (fullScreenMode)
					FullScreenMode_Set();
				else
					FullScreenMode_Reset();
			}
		}

		private void FullScreenMode_Set()
		{
			for (int i = 0; i < Controls.Count; i++)
				Controls[i].Visible = false;
			DrawingBoard.Visible = true;

			DrawingBoard.Size = DrawingBoardFullScreenCoords.Item1;
			DrawingBoard.Location = DrawingBoardFullScreenCoords.Item2;
		}

		private void FullScreenMode_Reset()
		{
			for (int i = 0; i < Controls.Count; i++)
				Controls[i].Visible = true;

			DrawingBoard.Size = DrawingBoardDefaultCoords.Item1;
			DrawingBoard.Location = DrawingBoardDefaultCoords.Item2;
		}
	}
}
