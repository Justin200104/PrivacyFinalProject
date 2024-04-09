using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public ChatView()
        {
            InitializeComponent();
            InitializeParticipants();
            GenerateConversation();
        }

        private void InitializeParticipants()
        {
            for (int i = 0; i < 10; i++) {
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
            string senderName = GetRandomTestDummy();
            string message = GetRandomMessage();
            AddChatMessage(senderName, message);
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
            chatMessagesListBox.Items.Add(formattedMessage);
        }
    }
}
