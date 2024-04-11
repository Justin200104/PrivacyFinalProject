using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PrivacyFinalProject.Helpers
{
	static class Constants
	{
		public const string IP = "127.0.0.1";
		public const int Port = 5537;
	}
	public class ServerFunctions
	{
		public TcpClient client;
		public NetworkStream stream;
		public List<string> connectedClients = new List<string>();
		public void ConnectToServer()
		{
			client = new TcpClient();
			client.Connect(Constants.IP, Constants.Port);
			stream = client.GetStream();
			Console.WriteLine("Connected to server");
		}
	}
}
