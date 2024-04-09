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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton==MouseButtonState.Pressed)
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            // Get Credentials


            // Validate Credentials


            // Create and show the ChatView window.
            ChatView chatView = new ChatView();
            chatView.Show();

            // Bring the new window to the foreground.
            chatView.Activate();

            // Close the current window or hide it before showing the new window.
            this.Close(); // Use this if you want to close the current window.
            // this.Hide(); // Use this if you just want to hide the current window.
            
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the ResetPasswordView window.
            ResetPasswordView resetPasswordView = new ResetPasswordView();
            resetPasswordView.Show();

            // Bring the new window to the foreground.
            resetPasswordView.Activate();

            // Close the current window or hide it before showing the new window.
            this.Close(); // Use this if you want to close the current window.
            // this.Hide(); // Use this if you just want to hide the current window.
        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the CreateAccountView window.
            CreateAccountView createAccountView = new CreateAccountView();
            createAccountView.Show();

            // Bring the new window to the foreground.
            createAccountView.Activate();

            // Close the current window or hide it before showing the new window.
            this.Close(); // Use this if you want to close the current window.
            // this.Hide(); // Use this if you just want to hide the current window.
        }
    }
}
