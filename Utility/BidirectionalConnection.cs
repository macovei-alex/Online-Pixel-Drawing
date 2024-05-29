using System;
using System.IO.Ports;
using System.Net.Sockets;

namespace Utility
{
	public class BidirectionalConnection
	{
		public TcpClient ClientToServer { get; }
		public TcpClient ServerToClient { get; }
		public string Username { get; set; }

		public BidirectionalConnection() { }

		public BidirectionalConnection(TcpClient clientToServer, TcpClient serverToClient)
		{
			ClientToServer = clientToServer;
			ServerToClient = serverToClient;
		}

		public bool Close()
		{
			try
			{
				ClientToServer.Close();
				ServerToClient.Close();
				return true;
			}
			catch
			{
				return false;
			}

		}
	}
}
