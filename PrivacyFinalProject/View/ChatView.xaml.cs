using System.Windows;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        private string[] testDummies = { "TestDummy1", "TestDummy2", "TestDummy3", "TestDummy4", "TestDummy5" };
        private Random random = new Random();
        private List<Tuple<string, string>> conversation = new List<Tuple<string, string>>();
        private string loggedInUser;
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];

        public ChatView(String firstName, String lastName)
        {
            InitializeComponent();
            InitializeParticipants();
            GenerateConversation();
            ConnectToServer();

            PseudonominizeUsername(firstName[0], lastName[0]);

        }

        private void ConnectToServer()
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 5537);
            Console.WriteLine("Connected to server");
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


        private void PseudonominizeUsername(char firstInitial, char lastInitial)
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

        }

        private void InitializeParticipants()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (string dummy in testDummies)
                {
                    participantsListBox.Items.Add(dummy);
                }
            }
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
            Application.Current.Shutdown();
        }

        private void btnLeaveChat_Click(object sender, RoutedEventArgs e)
        {
            // Log the user out
            loggedInUser = "";

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

                string senderName = GetRandomTestDummy();
                message = GetRandomMessage();
                AddChatMessage(senderName, message);
            }
            else { return; }

        }

        private string GetRandomTestDummy()
        {
            return testDummies[random.Next(testDummies.Length)];
        }

        private void GenerateConversation()
        {
            // Generate a dynamic conversation between random test dummies
            for (int i = 0; i < 10; i++)
            {
                string sender = GetRandomTestDummy();
                string message = GetRandomMessage();
                conversation.Add(new Tuple<string, string>(sender, message));
            }
        }

        private string GetRandomMessage()
        {
            string[] chatMessages = {
                "Hey, how's it going?",
                "Not bad, just busy with work. You?",
                "Same here. It never ends!",
                "Tell me about it. We should grab coffee sometime.",
                "That sounds like a plan. Let's do it next week?",
                "Sure, I'll ping you when I'm free.",
                "Great! Looking forward to it."
            };

            return chatMessages[random.Next(chatMessages.Length)];
        }

        public void AddChatMessage(string sender, string message)
        {
            var formattedMessage = $"{sender}: {message}";

            // Send the message to the server instead of adding it client-side

            chatMessagesListBox.Items.Add(formattedMessage);
        }
    }
}