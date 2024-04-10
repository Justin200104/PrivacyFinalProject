using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PrivacyFinalProject
{
	public class Server
	{
		private TcpListener server;
		private List<ConnectedClient> clients = new List<ConnectedClient>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		public Server(string ipAddress, int port)
		{
			server = new TcpListener(IPAddress.Parse(ipAddress), port);
		}

		/// <summary>
		/// Start server to start accepting clients
		/// </summary>
		public void StartServer()
		{
			server.Start();
			Console.WriteLine("Starting Server");

			while (true)
			{
				TcpClient client = server.AcceptTcpClient();
				Console.WriteLine("Client connected");

				ConnectedClient connectedClient = new ConnectedClient(client, this);
				clients.Add(connectedClient);

				Thread clientThread = new Thread(new ThreadStart(connectedClient.StartListening));
				clientThread.Start();
			}
		}

		/// <summary>
		/// Broadcast sent message to all currently connected clients
		/// </summary>
		/// <param name="message"></param>
		/// <param name="sender"></param>
		public void BroadcastMessage(string message, ConnectedClient sender)
		{
			foreach (ConnectedClient client in clients)
			{
				if (client != sender)
				{
					client.SendMessage(message);
				}
			}
		}

		/// <summary>
		/// Remove client from list of clients
		/// </summary>
		/// <param name="client"></param>
		public void RemoveClient(ConnectedClient client)
		{
			clients.Remove(client);
		}
	}

	public class ConnectedClient
	{
		private TcpClient client;
		private Server server;
		private NetworkStream stream;
		private string username;

		/// <summary>
		/// Constructor for a client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="server"></param>
		public ConnectedClient(TcpClient client, Server server)
		{
			this.client = client;
			this.server = server;
			stream = client.GetStream();
		}

		/// <summary>
		/// Listener for the client connecting to the chatroom. Listens for incoming messages
		/// </summary>
		public void StartListening()
		{
			byte[] buffer = new byte[1024];
			while (true)
			{
				int bytesRead = stream.Read(buffer, 0, buffer.Length);
				if (bytesRead == 0)
				{
					break;
				}

				string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				if (username == null)
				{
					username = message;
					Console.WriteLine($"{username} has connected to the server");
				}
				else
				{
					server.BroadcastMessage($"{username}: {message}", this);
				}
			}

			server.RemoveClient(this);
			client.Close();
		}

		/// <summary>
		/// Method to let user send a message to the chatroom
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(string message)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Server server = new Server("127.0.0.1", 5537);
			server.StartServer();
		}
	}
}