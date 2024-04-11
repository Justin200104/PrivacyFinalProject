using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PrivacyFinalProject
{
	static class Constants
	{
		public const string IP = "127.0.0.1";
		public const int Port = 5537;
	}

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

			DataBase.CreateDatabase();

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
		public void RemoveClient(ConnectedClient client, string username)
		{
			Console.WriteLine($"{username} has disconnected");
			clients.Remove(client);
		}

		public void BroadcastClientList()
		{
			string clientList = "[CLIENTLIST]" + string.Join(",", clients.Where(c => !string.IsNullOrEmpty(c.username)).Select(c => c.username));
			BroadcastMessage(clientList, null);
		}

	}

	public class ConnectedClient
	{
		private TcpClient client;
		private Server server;
		private NetworkStream stream;
		public string username;

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
		async public void StartListening()
		{
			byte[] buffer = new byte[1024];
			while (true)
			{
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
				if (bytesRead == 0)
				{
					break;
				}

				string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				if (message.StartsWith("[LOGIN]"))
				{
					String[] login = message.Substring(7).Split(',');

					String firstName = login[0];
					String lastName = login[1];
					String password = login[2];

					bool success = DataBase.CheckPassword(firstName, lastName, password);
					String res = success.ToString();

					byte[] loginbuf = Encoding.UTF8.GetBytes(res);
					await stream.WriteAsync(loginbuf, 0, loginbuf.Length);
					await stream.FlushAsync();

				}
				else if(message.StartsWith("[LOGINSUCCESS]"))
				{
					username = message.Substring(14);
					Console.WriteLine($"{username} has connected to the server");
					server.BroadcastClientList();
				}
				else if(message.StartsWith("[CREATEACCOUNT]"))
				{
					String[] create = message.Substring(15).Split(',');

					String firstName = create[0];
					String lastName = create[1];
					String password = create[2];

					DataBase.InsertData(firstName, lastName, password);
					Console.WriteLine("Create account success");
				}
				else if (message.StartsWith("[RESETPASSWORD]"))
				{
					Console.WriteLine("[RESETPASSWORD] hit");
				}
				else
				{
					server.BroadcastMessage($"[ {username} ]: {message}", this);
				}
				await stream.FlushAsync();
			}

			server.RemoveClient(this, this.username);
			server.BroadcastClientList();
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
			Server server = new Server(Constants.IP, Constants.Port);
			server.StartServer();
		}
	}
}