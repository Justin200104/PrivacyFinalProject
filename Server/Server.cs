using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Security.Cryptography;	

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
			Console.WriteLine($"[{GetTime()}] Starting Server");

			DataBase.CreateDatabase();

			while (true)
			{
				TcpClient client = server.AcceptTcpClient();
				//Console.WriteLine("Client connected");

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
                string encryptedMsg = AES.EncryptString(message);
                if (client != sender)
				{
					Console.WriteLine($"{client}: OriginalMessage: {message}\n{client}: Encrypted Message:{encryptedMsg}");

					client.SendMessage(encryptedMsg);
				}
				else
				{
                    Console.WriteLine($"{client}: OriginalMessage: {message}\n{client}: Encrypted Message:{encryptedMsg}");
                }
			}
		}

		/// <summary>
		/// Remove client from list of clients
		/// </summary>
		/// <param name="client"></param>
		public void RemoveClient(ConnectedClient client, string username)
		{
			if(username != null)
				Console.WriteLine($"[{GetTime()}] {username} has disconnected");

			clients.Remove(client);
		}

		public void BroadcastClientList()
		{
			string clientList = "[CLIENTLIST]" + string.Join(",", clients.Where(c => !string.IsNullOrEmpty(c.username)).Select(c => c.username));
			BroadcastMessage(clientList, null);
		}

		public String GetTime()
		{
			String currentTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
			return currentTime;
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
				
                string message = AES.DecryptString(Encoding.UTF8.GetString(buffer, 0, bytesRead));
               
                if (message.StartsWith("[LOGIN]"))
				{
					String[] login = message.Substring(7).Split(',');

					String firstName = login[0];
					String lastName = login[1];
					String password = login[2];

                    String ProtectedFirstName = Pseudoanonymization.HashString(AES.EncryptString(firstName));
                    String ProtectedLastName = Pseudoanonymization.HashString(AES.EncryptString(lastName));
                    String ProtectedPassword = Pseudoanonymization.HashString(AES.EncryptString(password));

					Console.WriteLine($"[{server.GetTime()}] {ProtectedFirstName} {ProtectedLastName} is attempting to login");

                    bool success = DataBase.CheckPassword(ProtectedFirstName, ProtectedLastName, ProtectedPassword);
					String res = success.ToString();
					
					byte[] loginbuf = Encoding.UTF8.GetBytes(AES.EncryptString(res));
					await stream.WriteAsync(loginbuf, 0, loginbuf.Length);
					await stream.FlushAsync();

					if(success)
						Console.WriteLine($"[{server.GetTime()}] Login successful");
					else
						Console.WriteLine($"[{server.GetTime()}] Login failed");
				}
				else if(message.StartsWith("[LOGINSUCCESS]"))
				{
					username = message.Substring(14);
					Console.WriteLine($"[{server.GetTime()}] {username} has connected to the server");
					server.BroadcastClientList();
				}
				else if(message.StartsWith("[CREATEACCOUNT]"))
				{
					String[] create = message.Substring(15).Split(',');

                    String firstName = create[0];
                    String lastName = create[1];
                    String password = create[2];

                    String ProtectedFirstName = Pseudoanonymization.HashString(AES.EncryptString(firstName));
                    String ProtectedLastName = Pseudoanonymization.HashString(AES.EncryptString(lastName));
                    String ProtectedPassword = Pseudoanonymization.HashString(AES.EncryptString(password));          

					Console.WriteLine($"[{server.GetTime()}] {ProtectedFirstName} {ProtectedLastName} is attempting to create an account");	

					DataBase.InsertData(ProtectedFirstName, ProtectedLastName, ProtectedPassword);
					Console.WriteLine($"[{server.GetTime()}] Create account success");
				}
				else if (message.StartsWith("[RESETPASSWORD]"))
				{
					String[] create = message.Substring(15).Split(',');
                    String firstName = create[0];
                    String lastName = create[1];
                    String password = create[2];
                    String resetpass = create[3];

                    String ProtectedFirstName = Pseudoanonymization.HashString(AES.EncryptString(firstName));
                    String ProtectedLastName = Pseudoanonymization.HashString(AES.EncryptString(lastName));
                    String ProtectedPassword = Pseudoanonymization.HashString(AES.EncryptString(password));
					String ProtectedResetPass = Pseudoanonymization.HashString(AES.EncryptString(resetpass));

					Console.WriteLine($"[{server.GetTime()}] {ProtectedFirstName} {ProtectedLastName} is attempting to reset password");

                    bool success = DataBase.ResetPassword(ProtectedFirstName, ProtectedLastName, ProtectedPassword, ProtectedResetPass);
					String res = success.ToString();
					byte[] resetbuf = Encoding.UTF8.GetBytes(AES.EncryptString(res));

					await stream.WriteAsync(resetbuf, 0, resetbuf.Length);
					await stream.FlushAsync();

					if(success)
						Console.WriteLine($"[{server.GetTime()}] Password reset success");
					else
						Console.WriteLine($"[{server.GetTime()}] Password reset unsuccessful");
				}
				else if (message.StartsWith("[DELETEACCOUNT]"))
				{
					String[] create = message.Substring(15).Split(',');

					String firstName = create[0];
					String lastName = create[1];
					String password = create[2];

					String ProtectedFirstName = Pseudoanonymization.HashString(AES.EncryptString(firstName));
					String ProtectedLastName = Pseudoanonymization.HashString(AES.EncryptString(lastName));
					String ProtectedPassword = Pseudoanonymization.HashString(AES.EncryptString(password));

					Console.WriteLine($"[{server.GetTime()}] Attempting to delete {ProtectedFirstName} {ProtectedLastName}");

					bool success = DataBase.DeleteAccount(ProtectedFirstName, ProtectedLastName, ProtectedPassword);
					String res = success.ToString();
					byte[] deletebuf = Encoding.UTF8.GetBytes(AES.EncryptString(res));

					await stream.WriteAsync(deletebuf, 0, deletebuf.Length);
					await stream.FlushAsync();

					if (success)
						Console.WriteLine($"[{server.GetTime()}] Account deletion success");
					else
						Console.WriteLine($"[{server.GetTime()}] Account deletion unsuccessful");
				}
				else
				{
					server.BroadcastMessage($"[{server.GetTime()}] [ {username} ]: {message}", this);
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