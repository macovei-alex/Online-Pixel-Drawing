using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Utility;

namespace Server
{
	public partial class ServerForm : Form
	{
		int squareSize;
		float zoomFactor;
		Point translation;
		bool mousePressed;

		Point lastMouseAbsLocation;

		Color[][] board;
		List<Color> colorList;
		Dictionary<Color, Brush> brushDictionary;
		Pen linePen, highlightPen;

		TcpListener clientToServerListener, serverToClientListener;
		List<BidirectionalConnection> connections;

		Thread receiveThread, listeningThread;
		bool formClosed, trySave;
		ManualResetEvent mainThread_InvertedSignal, receiveThread_InvertedSignal, listeningThread_InvertedSignal;

		public ServerForm()
		{
			InitializeComponent();
			PictureBox.MouseWheel += new MouseEventHandler(PictureBox_MouseWheel);

			Location = new Point(
				Screen.FromHandle(Handle).Bounds.X + 100,
				Screen.FromHandle(Handle).Bounds.Y + 50);
			Size = new Size(
				Const.WIDTH + 2 * Const.MARGIN + Const.LINE_WIDTH + 15,
				Const.HEIGHT + 3 * Const.MARGIN + Const.TEXTBOX_HEIGHT + Const.LINE_WIDTH + 40);
			PictureBox.Size = new Size(
				Const.WIDTH + Const.LINE_WIDTH,
				Const.HEIGHT + Const.LINE_WIDTH);
			PictureBox.Location = new Point(
				Const.MARGIN,
				Const.MARGIN);
			RichTextBox.Size = new Size(
				Const.TEXTBOX_WIDTH,
				Const.TEXTBOX_HEIGHT);
			RichTextBox.Location = new Point(
				Const.MARGIN,
				Const.HEIGHT + 2 * Const.MARGIN);
			LocationLabel.Location = new Point(
				Const.WIDTH - 70,
				20);
			ZoomLabel.Location = new Point(
				LocationLabel.Location.X,
				LocationLabel.Location.Y + LocationLabel.Height);
			ZoomInButton.Location = new Point(
				ZoomLabel.Location.X,
				ZoomLabel.Location.Y + ZoomLabel.Height);
			ZoomInButton.Size = new Size(
				Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);
			ZoomOutButton.Location = new Point(
				ZoomInButton.Location.X + ZoomInButton.Width,
				ZoomInButton.Location.Y);
			ZoomOutButton.Size = new Size(
				ZoomInButton.Width,
				ZoomInButton.Height);
			SaveButton.Location = new Point(
				RichTextBox.Location.X + RichTextBox.Width + 2 * Const.MARGIN,
				RichTextBox.Location.Y);
			SaveButton.Size = new Size(
				2 * Const.ZOOM_BUTTON_WIDTH,
				Const.ZOOM_BUTTON_HEIGHT);
			OpenButton.Location = new Point(
				SaveButton.Location.X,
				SaveButton.Location.Y + SaveButton.Height);
			OpenButton.Size = new Size(
				SaveButton.Width,
				SaveButton.Height);
			ClearBoardButton.Location = new Point(
				OpenButton.Location.X,
				OpenButton.Location.Y + OpenButton.Height);
			ClearBoardButton.Size = new Size(
				OpenButton.Width,
				OpenButton.Height);
		}

		private void Log(string sender, string message)
		{
			RichTextBox.BeginInvoke(new Action(() => RichTextBox.AppendText($"[{sender}] {message}" + Environment.NewLine)));
		}

		private void ServerForm_Load(object sender, EventArgs e)
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
			mousePressed = false;

			try
			{
				clientToServerListener = new TcpListener(
					IPAddress.Parse(Properties.Settings.Default.ServerAddress),
					Properties.Settings.Default.ClientToServerPort);
				serverToClientListener = new TcpListener(
					IPAddress.Parse(Properties.Settings.Default.ServerAddress),
					Properties.Settings.Default.ServerToClientPort);
				connections = new List<BidirectionalConnection>();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				trySave = false;
				Close();
				return;
			}
			receiveThread_InvertedSignal = new ManualResetEvent(true);
			mainThread_InvertedSignal = new ManualResetEvent(true);
			listeningThread_InvertedSignal = new ManualResetEvent(true);
			trySave = true;
			formClosed = false;

			listeningThread = new Thread(ListeningForNewClients);
			listeningThread.Start();
			receiveThread = new Thread(ReceiveHandler);
			receiveThread.Start();
		}

		private void ServerForm_FormClosing(object sender, FormClosingEventArgs args)
		{
			if (trySave)
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
					args.Cancel = true;
					return;
				}
			}

			formClosed = true;

			try
			{
				receiveThread.Join();

				clientToServerListener.Stop();
				serverToClientListener.Stop();
				listeningThread.Join();

				for (int i = 0; i < connections.Count; i++)
				{
					Func.SendCommandCode(connections[i].ServerToClient, CommandCode.Close);
					connections[i].Close();
				}
			}
			catch { }
		}

		private void ListeningForNewClients()
		{
			clientToServerListener.Start();
			serverToClientListener.Start();
			try
			{
				while (!formClosed)
				{
					TcpClient clientToServer = clientToServerListener.AcceptTcpClient();
					TcpClient serverToClient = serverToClientListener.AcceptTcpClient();

					var newConnection = new BidirectionalConnection(clientToServer, serverToClient);
					string username = Func.ReceiveString(newConnection.ClientToServer);
					string password = Func.ReceiveString(newConnection.ClientToServer);
					if (TryLoginAs(username, password))
					{
						Func.SendString(newConnection.ServerToClient, Const.CONNECTION_ACCEPTED);
						newConnection.Username = username;

						Func.SendCommandCode(newConnection.ServerToClient, CommandCode.LoadImage);
						for (int i = 0; i < Const.SQUARE_ROWS; i++)
						{
							for (int j = 0; j < Const.SQUARE_COLS; j++)
							{
								if (board[i][j] != Const.COLOR_WHITE)
								{
									TransferObject obj = new TransferObject(i, j, board[i][j]);
									Func.SendObject(newConnection.ServerToClient, obj);
								}
							}
						}
						Func.SendObject(newConnection.ServerToClient, Const.CONTROL_OBJECT_NULL);

						mainThread_InvertedSignal.WaitOne();
						listeningThread_InvertedSignal.Reset();
						receiveThread_InvertedSignal.WaitOne();

						connections.Add(newConnection);

						listeningThread_InvertedSignal.Set();

						Log("Listening_thread", $"New client with username ({newConnection.Username}) added");
					}
					else
					{
						Func.SendString(newConnection.ServerToClient, Const.CONNECTION_DENIED);
					}

				}
			}
			catch
			{
				Log("Listening_Thread", "Listeners forcibly closed");
			}
		}

		private bool TryLoginAs(string username, string password)
		{
			if (username == "root")
				return CheckLegimtimateUser(username, password);

			foreach (var connection in connections)
			{
				if (connection.Username == username)
					return false;
			}

			return CheckLegimtimateUser(username, password);
		}

		private bool CheckLegimtimateUser(string usernameToCheck, string passwordToCheck)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Properties.Settings.Default.UsersCredentialsFilePath);
			XmlNodeList nodes = doc.SelectNodes("/Root/User");
			passwordToCheck = Func.HashString(passwordToCheck);

			foreach (XmlNode node in nodes)
			{
				string username = node.SelectSingleNode("Username").InnerText;
				string password = node.SelectSingleNode("Password").InnerText;

				if (username == usernameToCheck && passwordToCheck == password)
					return true;
			}

			return false;
		}

		private void ReceiveHandler()
		{
			List<BidirectionalConnection> connectionsToRemove = new List<BidirectionalConnection>();

			while (!formClosed)
			{
				mainThread_InvertedSignal.WaitOne();
				receiveThread_InvertedSignal.Reset();
				listeningThread_InvertedSignal.WaitOne();

				for (int i = 0; i < connections.Count; i++)
				{
					if (connections[i].ClientToServer.GetStream().DataAvailable)
					{
						CommandCode code = Func.ReceiveCommandCode(connections[i].ClientToServer);
						switch (code)
						{
							case CommandCode.Close:
								connectionsToRemove.Add(connections[i]);
								break;

							case CommandCode.SendObject:
								TransferObject obj = Func.ReceiveObject(connections[i].ClientToServer);
								board[obj.Row][obj.Column] = obj.Color;
								Log("Receiving thread", $"Object received from ({connections[i].Username}): {obj}");
								Invoke(new Action(() => Refresh()));

								for (int j = 0; j < connections.Count; j++)
								{
									if (i != j)
									{
										Func.SendCommandCode(connections[j].ServerToClient, CommandCode.SendObject);
										Func.SendObject(connections[j].ServerToClient, obj);
									}
								}
								break;
						}
					}
				}

				foreach (var connection in connectionsToRemove)
				{
					Log("Receiving thread", $"Client ({connection.Username}) disconnected");
					connections.Remove(connection);
				}
				connectionsToRemove.Clear();

				receiveThread_InvertedSignal.Set();

				Thread.Sleep(1000 / Const.UPDATE_CHECK_RATE);
			}
		}


		private void PictureBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.TranslateTransform(-translation.X, -translation.Y);

			int firstRow = Math.Max(0, translation.Y / squareSize);
			int firstCol = Math.Max(0, translation.X / squareSize);
			int lastRow = Math.Min(firstRow + Const.HEIGHT / squareSize + 1, Const.SQUARE_ROWS);
			int lastCol = Math.Min(firstCol + Const.WIDTH / squareSize + 1, Const.SQUARE_COLS);

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

		private void PictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			Point newMouseAbsLocation = new Point(e.X + translation.X, e.Y + translation.Y);
			if (mousePressed)
			{
				int dx = newMouseAbsLocation.X - lastMouseAbsLocation.X;
				int dy = newMouseAbsLocation.Y - lastMouseAbsLocation.Y;
				translation = new Point(translation.X - dx, translation.Y - dy);
				Refresh();
			}
			else
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

		private void PictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			mousePressed = true;
		}

		private void PictureBox_MouseUp(object sender, MouseEventArgs e)
		{
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
						+ (lastMouseAbsLocation.Y / squareSize).ToString();
			Refresh();
		}

		private void SaveButton_MouseClick(object sender, MouseEventArgs e)
		{
			SaveFileDialog saveFileDialog = (SaveFileDialog)CreateDialog(FileDialogMode.save);
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				SaveImage(saveFileDialog.FileName);
		}

		private void OpenButton_MouseClick(object sender, MouseEventArgs e)
		{

			OpenFileDialog openFileDialog = (OpenFileDialog)CreateDialog(FileDialogMode.open);
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			receiveThread_InvertedSignal.WaitOne();
			mainThread_InvertedSignal.Reset();
			listeningThread_InvertedSignal.WaitOne();

			board = Func.CreateBoard();

			using (StreamReader reader = new StreamReader(openFileDialog.FileName))
			{
				foreach (var connection in connections)
					Func.SendCommandCode(connection.ServerToClient, CommandCode.LoadImage);

				while (!reader.EndOfStream)
				{
					string objString = reader.ReadLine();
					TransferObject obj = TransferObject.StringToObject(objString);
					board[obj.Row][obj.Column] = obj.Color;
					foreach (var connection in connections)
					{
						Func.SendObject(connection.ServerToClient, obj);
					}
				}

				foreach (var connection in connections)
					Func.SendObject(connection.ServerToClient, Const.CONTROL_OBJECT_NULL);
			}

			mainThread_InvertedSignal.Set();

			Log("LoadButton_Handler", $"Image opened from {openFileDialog.FileName}");
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

			Log("Image_Saver", $"Image saved at {saveFilePath}");
		}

		private void ClearBoardButton_Click(object sender, EventArgs e)
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
			if (questionResult != DialogResult.Cancel)
				ClearBoard();
		}

		private void ClearBoard()
		{
			receiveThread_InvertedSignal.WaitOne();
			mainThread_InvertedSignal.Reset();
			listeningThread_InvertedSignal.WaitOne();

			foreach (var connection in connections)
			{
				Func.SendCommandCode(connection.ServerToClient, CommandCode.ClearBoard);
			}
			board = Func.CreateBoard();

			mainThread_InvertedSignal.Set();

			Log("ClearBoardButton_Click", "Board has been cleared");
		}
	}
}
