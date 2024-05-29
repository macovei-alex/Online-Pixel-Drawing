using System.Drawing;
using System.Net.Sockets;

namespace Utility
{
	public class TransferObject
	{
		public int Row { get; set; }
		public int Column { get; set; }
		public Color Color { get; set; }

		public TransferObject()
		{
			Row = 0;
			Column = 0;
			Color = Color.Empty;
		}

		public TransferObject(int row, int column, Color color)
		{
			Row = row;
			Column = column;
			Color = color;
		}

		public byte[] ObjectToBytes()
		{
			byte[] byteArray = new byte[Const.OBJECT_BYTE_COUNT];

			string objString = ToString();
			for (int i = 0; i < Const.OBJECT_BYTE_COUNT; i++)
			{
				byteArray[i] = (byte)objString[i];
			}
			return byteArray;
		}

		public static TransferObject BytesToObject(byte[] bytes)
		{
			string objString = string.Empty;
			for (int i = 0; i < Const.OBJECT_BYTE_COUNT; i++)
			{
				objString += (char)bytes[i];
			}

			TransferObject newObj = StringToObject(objString);
			return newObj;
		}

		public override string ToString()
		{
			string row = Row.ToString().PadLeft(4, '0');
			string column = Column.ToString().PadLeft(4, '0');
			string A = Color.A.ToString().PadLeft(3, '0');
			string R = Color.R.ToString().PadLeft(3, '0');
			string G = Color.G.ToString().PadLeft(3, '0');
			string B = Color.B.ToString().PadLeft(3, '0');

			return $"{row},{column},{A},{R},{G},{B}";
		}

		public static TransferObject StringToObject(string str)
		{
			string[] values = str.Split(',');

			byte A = byte.Parse(values[2]);
			byte R = byte.Parse(values[3]);
			byte G = byte.Parse(values[4]);
			byte B = byte.Parse(values[5]);
			TransferObject newObj = new TransferObject
			{
				Row = int.Parse(values[0]),
				Column = int.Parse(values[1]),
				Color = Color.FromArgb(A, R, G, B)
			};

			return newObj;
		}

		public override bool Equals(object obj)
		{
			bool ret = false;
			if (obj is TransferObject)
			{
				ret = true;
				TransferObject b = obj as TransferObject;
				if (this.Row != b.Row)
					ret = false;
				else if (Column != b.Column)
					ret = false;
				else if (Color != b.Color)
					ret = false;
			}
			return ret;
		}
	}
}
