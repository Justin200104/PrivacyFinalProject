using System.Windows;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using PrivacyFinalProject.Helpers;
using System;
using System.Net.Security;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        ServerFunctions SF;
		private string loggedInUser;
       
		public ChatView(String firstName, String lastName, ref ServerFunctions s)
        {
			SF = s;
			// Start receiving messages in a separate thread
			Thread receiveThread = new Thread(new ThreadStart(RecvMsgs));
			receiveThread.Start();

			PseudonominizeUsername(firstName[0], lastName[0]);
			
			InitializeComponent();
		}

		async public void RecvMsgs()
		{
			try
			{
				while (true)
                {
                    byte[] buffer = new byte[1024];
					int bytesRead = await SF.stream.ReadAsync(buffer, 0, buffer.Length);
					if (bytesRead == 0)
                    {
                        break;
                    }

					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					if (message.StartsWith("[CLIENTLIST]"))
					{
						string[] clients = message.Substring(12).Split(',');
						SF.connectedClients.Clear();
						SF.connectedClients.AddRange(clients);
						Dispatcher.Invoke(() =>
						{
							participantsListBox.ItemsSource = null;
							participantsListBox.ItemsSource = SF.connectedClients;
						});
					}
					else
					{
						Dispatcher.Invoke(() =>
						{
							chatMessagesListBox.Items.Add(message);
						});
					}
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions
				Console.WriteLine($"Error: {ex.Message}");
			}
			finally
			{
				SF.client.Close();
			}

		}
		private string[] ReadDictionaryFromFile(string filePath)
        {
            string[] dictionary = null;
            try
            {
                // Read the file
                string jsonContent = File.ReadAllText(filePath);

                // Deserialize JSON to string array
                dictionary = JsonConvert.DeserializeObject<string[]>(jsonContent);
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error reading dictionary file: {ex.Message}");
            }
            return dictionary;
        }


        private string GetWordStartingWith(string[] dictionary, char startChar, Random rand)
        {
            // Filter dictionary words starting with the given character
            var filteredWords = dictionary.Where(word => word.StartsWith(char.ToUpper(startChar).ToString(), StringComparison.OrdinalIgnoreCase)).ToArray();
            if (filteredWords.Length == 0)
            {
                // If no word starts with the given character, choose a random word
                return dictionary[rand.Next(0, dictionary.Length)];
            }
            // Choose a random word from the filtered list
            return filteredWords[rand.Next(0, filteredWords.Length)];
        }


        async private void PseudonominizeUsername(char firstInitial, char lastInitial)
        {

            // Seed Time
            DateTime now = DateTime.Now;
            int seed1 = now.Hour * 3600 + now.Minute * 60 + now.Second;
            int seed2 = now.Millisecond;

            // Shift initials by time value on Ascii table
            char shiftedFirstInitial = (char)(((firstInitial + seed1) - 'A') % 26 + 'A');
            char shiftedLastInitial = (char)(((lastInitial + seed2) - 'A') % 26 + 'A');

            // Read dictionary from JSON file
            string[] dictionary = ReadDictionaryFromFile("../../../Resources/DictionaryWords.json");

            Random rand1 = new Random(seed1);
            Random rand2 = new Random(seed2);

            // Pick words from the shifted characters
            string firstWord = GetWordStartingWith(dictionary, shiftedFirstInitial, rand1);
            string lastWord = GetWordStartingWith(dictionary, shiftedLastInitial, rand2);

            // Concatenate dictionary words
            string pseudonimizedName = $"{firstWord}_{lastWord}";

            // Set LoggedInUser
            loggedInUser = pseudonimizedName;
			// Send this participant to the server;
			byte[] buffer = Encoding.UTF8.GetBytes($"[LOGINSUCCESS]{pseudonimizedName}");
			await SF.stream.WriteAsync(buffer, 0, buffer.Length);
			await SF.stream.FlushAsync();
		}

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
			SF.stream.Close();
			Application.Current.Shutdown();
        }

        private void btnLeaveChat_Click(object sender, RoutedEventArgs e)
        {
            // Log the user out
            loggedInUser = "";
			SF.stream.Close();

			// Create and show the LoginView window.
			LoginView loginView = new LoginView();
            loginView.Show();

            // Bring the new window to the foreground.
            loginView.Activate();

            // Close the current window or hide it before showing the new window.
            this.Close(); // Use this if you want to close the current window.
            // this.Hide(); // Use this if you just want to hide the current window.
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {

            // Get the message
            String message = txtMessage.Text;

            if (message != "")
            {
                // Sent the message to the server
                AddChatMessage(loggedInUser, message);
                // Reset the text box
                txtMessage.Text = String.Empty;
            }
            else { return; }

        }

        public void AddChatMessage(string sender, string message)
        {
            var formattedMessage = $"[{SF.GetTime()}] [ {sender} ]: {message}";

			// Send the message to the server instead of adding it client-side
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			SF.stream.Write(buffer, 0, buffer.Length);
			SF.stream.Flush();

			txtMessage.Text = "";

			chatMessagesListBox.Items.Add(formattedMessage);
        }
    }
}