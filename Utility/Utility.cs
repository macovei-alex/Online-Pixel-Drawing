using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace Utility
{
	public enum FileDialogMode
	{
		save,
		open
	}

	public enum CommandCode : byte
	{
		SendObject,
		LoadImage,
		SendString,
		Close,
		ClearBoard
	}

	public class Const
	{
		// ClientMainForm, ClientColorPicker, ServerForm
		public const int WIDTH = 800;
		public const int HEIGHT = 600;
		public const int SQUARE_SIZE = 20;
		public const int MARGIN = 10;
		public const int LINE_WIDTH = 1;
		public const int HIGHLIGHT_WIDTH = 2;
		public const int SQUARE_ROWS = 900;
		public const int SQUARE_COLS = 1200;
		public const float ZOOM_MIN = 0.1f;
		public const float ZOOM_MAX = 3.0f;
		public const float ZOOM_MIN_TO_HIGHLIGHT_CURSOR = 0.5f;
		public const int MAX_STR_COUNT = 100;
		public static readonly Color COLOR_BLACK = Color.FromArgb(255, 0, 0, 0);
		public static readonly Color COLOR_WHITE = Color.FromArgb(255, 255, 255, 255);
		public static readonly Color LINE_COLOR = COLOR_BLACK;
		public static readonly TransferObject CONTROL_OBJECT_NULL = new TransferObject(9999, 9999, Color.FromArgb(0, 0, 0, 0));
		public const string CONNECTION_ACCEPTED = "Connection successful";
		public const string CONNECTION_DENIED = "Connection unsuccessful";

		// ClientColorPicker
		public const int WIDTH2 = 400;
		public const int HEIGHT2 = 450;
		public const int COLOR_SIZE = 50;
		public static readonly Color COLOR_PICKER_CANCEL = Color.FromArgb(0, 255, 255, 255);

		// ClientLoginForm
		public const int WIDTH3 = 500;
		public const int HEIGHT3 = 150;
		public const int LOGINFORM_TEXTBOX_WIDTH = 300;
		public const int OFFSET = 30;

		// ClientForm, ServerForm
		public const int TEXTBOX_WIDTH = 710;
		public const int TEXTBOX_HEIGHT = 100;
		public const int UPDATE_CHECK_RATE = 10;
		public const int ZOOM_BUTTON_WIDTH = 35;
		public const int ZOOM_BUTTON_HEIGHT = 30;
		public const int CONNECT_BUTTON_WIDTH = 80;
		public const int CONNECT_BUTTON_HEIGHT = 25;

		// TransferObject
		public const int OBJECT_BYTE_COUNT = 25;
	}

	public class Func
	{
		private static readonly Random random = new Random();
		private static readonly SHA256 sha256Hash = SHA256.Create();

		public static int Random(int min = int.MinValue, int max = int.MaxValue)
		{
			return random.Next(min, max);
		}

		public static string HashString(string toHash)
		{
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));
			StringBuilder hashedString = new StringBuilder();

			for (int i = 0; i < bytes.Length; i++)
			{
				hashedString.Append(bytes[i].ToString("x2"));
			}

			return hashedString.ToString();
		}

		public static Color[][] CreateBoard()
		{
			Color[][] board = new Color[Const.SQUARE_ROWS][];
			for (int i = 0; i < Const.SQUARE_ROWS; i++)
			{
				board[i] = new Color[Const.SQUARE_COLS];
				for (int j = 0; j < Const.SQUARE_COLS; j++)
				{
					board[i][j] = Const.COLOR_WHITE;
				}
			}
			return board;
		}

		public static void SendCommandCode(TcpClient receiver, CommandCode code)
		{
			byte[] commandByte = new byte[1];
			commandByte[0] = (byte)code;
			receiver.GetStream().Write(commandByte, 0, 1);
		}

		public static CommandCode ReceiveCommandCode(TcpClient receiver)
		{
			byte[] buffer = new byte[1];
			receiver.GetStream().Read(buffer, 0, 1);
			return (CommandCode)buffer[0];
		}

		public static void SendString(TcpClient receiver, string str)
		{
			if (str.Count() > Const.MAX_STR_COUNT)
				str = str.Substring(0, Const.MAX_STR_COUNT);
			else
				str = str.PadRight(Const.MAX_STR_COUNT, '\0');
			byte[] bytes = Encoding.ASCII.GetBytes(str);
			receiver.GetStream().Write(bytes, 0, bytes.Length);
		}

		public static string ReceiveString(TcpClient receiver)
		{
			byte[] stringBytes = new byte[Const.MAX_STR_COUNT];
			receiver.GetStream().Read(stringBytes, 0, stringBytes.Length);

			string str = Encoding.ASCII.GetString(stringBytes);
			str = str.TrimEnd('\0');

			return str;
		}

		public static bool SendObject(TcpClient receiver, TransferObject obj)
		{
			try
			{
				byte[] bytes = obj.ObjectToBytes();
				receiver.GetStream().Write(bytes, 0, Const.OBJECT_BYTE_COUNT);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static TransferObject ReceiveObject(TcpClient sender)
		{
			try
			{
				byte[] bytes = new byte[Const.OBJECT_BYTE_COUNT];
				sender.GetStream().Read(bytes, 0, Const.OBJECT_BYTE_COUNT);
				TransferObject obj = TransferObject.BytesToObject(bytes);
				return obj;
			}
			catch
			{
				return null;
			}
		}

		public static void DefineColors(ref List<Color> colorList, ref Dictionary<Color, Brush> brushDictionary)
		{
			colorList = new List<Color>
			{
				Color.FromArgb(255, 255, 255, 255),  // White
				Color.FromArgb(255, 128, 0, 0),      // Maroon
				Color.FromArgb(255, 139, 0, 0),      // Dark Red
				Color.FromArgb(255, 165, 42, 42),    // Brown
				Color.FromArgb(255, 178, 34, 34),    // Firebrick
				Color.FromArgb(255, 220, 20, 60),    // Crimson
				Color.FromArgb(255, 255, 0, 0),      // Red
				Color.FromArgb(255, 255, 99, 71),    // Tomato

				Color.FromArgb(255, 255, 69, 0),     // Orange Red
				Color.FromArgb(255, 255, 140, 0),    // Dark Orange
				Color.FromArgb(255, 255, 165, 0),    // Orange
				Color.FromArgb(255, 255, 215, 0),    // Gold
				Color.FromArgb(255, 218, 165, 32),   // Goldenrod
				Color.FromArgb(255, 255, 255, 0),    // Yellow
				Color.FromArgb(255, 154, 205, 50),   // Yellow Green
				Color.FromArgb(255, 85, 107, 47),    // Olive Drab

				Color.FromArgb(255, 107, 142, 35),   // Olive
				Color.FromArgb(255, 0, 100, 0),      // Dark Green
				Color.FromArgb(255, 0, 128, 0),      // Green
				Color.FromArgb(255, 0, 255, 0),      // Lime
				Color.FromArgb(255, 50, 205, 50),    // Lime Green
				Color.FromArgb(255, 144, 238, 144),  // Light Green
				Color.FromArgb(255, 0, 250, 154),    // Medium Sea Green
				Color.FromArgb(255, 173, 255, 47),   // Green Yellow

				Color.FromArgb(255, 0, 128, 128),    // Teal
				Color.FromArgb(255, 0, 139, 139),    // Dark Cyan
				Color.FromArgb(255, 0, 255, 255),    // Cyan
				Color.FromArgb(255, 0, 206, 209),    // Dark Turquoise
				Color.FromArgb(255, 64, 224, 208),   // Turquoise
				Color.FromArgb(255, 72, 209, 204),   // Medium Turquoise
				Color.FromArgb(255, 127, 255, 212),  // Aquamarine
				Color.FromArgb(255, 0, 191, 255),    // Deep Sky Blue

				Color.FromArgb(255, 30, 144, 255),   // Dodger Blue
				Color.FromArgb(255, 0, 0, 255),      // Blue
				Color.FromArgb(255, 0, 0, 205),      // Medium Blue
				Color.FromArgb(255, 0, 0, 139),      // Dark Blue
				Color.FromArgb(255, 25, 25, 112),    // Midnight Blue
				Color.FromArgb(255, 0, 0, 128),      // Navy
				Color.FromArgb(255, 138, 43, 226),   // Blue Violet
				Color.FromArgb(255, 75, 0, 130),     // Indigo

				Color.FromArgb(255, 72, 61, 139),    // Dark Slate Blue
				Color.FromArgb(255, 106, 90, 205),   // Slate Blue
				Color.FromArgb(255, 123, 104, 238),  // Medium Slate Blue
				Color.FromArgb(255, 147, 112, 219),  // Medium Purple
				Color.FromArgb(255, 139, 0, 139),    // Dark Magenta
				Color.FromArgb(255, 148, 0, 211),    // Dark Orchid
				Color.FromArgb(255, 186, 85, 211),   // Medium Orchid
				Color.FromArgb(255, 216, 191, 216),  // Thistle

				Color.FromArgb(255, 255, 192, 203),  // Pink
				Color.FromArgb(255, 255, 182, 193),  // Light Pink
				Color.FromArgb(255, 255, 105, 180),  // Hot Pink
				Color.FromArgb(255, 255, 20, 147),   // Deep Pink
				Color.FromArgb(255, 219, 112, 147),  // Pale Violet Red
				Color.FromArgb(255, 199, 21, 133),   // Medium Violet Red
				Color.FromArgb(255, 255, 240, 245),  // Lavender Blush
				Color.FromArgb(255, 255, 228, 225),  // Misty Rose

				Color.FromArgb(255, 255, 250, 205),  // Lemon Chiffon
				Color.FromArgb(255, 255, 250, 220),  // Empty
				Color.FromArgb(255, 255, 255, 240),  // Ivory
				Color.FromArgb(255, 250, 250, 210),  // Light Goldenrod Yellow
				Color.FromArgb(255, 255, 255, 224),  // Light Yellow
				Color.FromArgb(255, 255, 255, 0),    // Yellow
				Color.FromArgb(255, 255, 215, 0),    // Gold
				Color.FromArgb(255, 255, 140, 0),    // Dark Orange

				Color.FromArgb(255, 218, 165, 32),   // Goldenrod
				Color.FromArgb(255, 184, 134, 11),   // Dark Goldenrod
				Color.FromArgb(255, 210, 105, 30),   // Chocolate
				Color.FromArgb(255, 139, 69, 19),    // Saddle Brown
				Color.FromArgb(255, 160, 82, 45),    // Sienna
				Color.FromArgb(255, 165, 42, 42),    // Brown
				Color.FromArgb(255, 128, 0, 0),      // Maroon
				Color.FromArgb(255, 0, 0, 0)         // Black
			};

			brushDictionary = new Dictionary<Color, Brush>();
			foreach (var color in colorList)
			{
				brushDictionary[color] = new SolidBrush(color);
			}
		}
	}
}

