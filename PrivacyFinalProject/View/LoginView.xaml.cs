using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
using PrivacyFinalProject.Helpers;
using System.Net.Sockets;

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
            DataBase.CreateDatabase();

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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            // Get Credentials
            String firstName = txtUserFirst.Text;
            String lastName = txtUserLast.Text;
            String password = txtPass.Password;

            // Ensure fields are not empty
            if (firstName.Length > 0 && lastName.Length > 0 && password.Length > 0)
            {

                // Validate Credentials in DB

                if (DataBase.CheckPassword(firstName, lastName, password))
                {

                    // Create and show the ChatView window.
                    ChatView chatView = new ChatView(firstName, lastName);
                    chatView.Show();

                    // Bring the new window to the foreground.
                    chatView.Activate();

                    // Close the current window or hide it before showing the new window.
                    this.Close(); // Use this if you want to close the current window.
                                  // this.Hide(); // Use this if you just want to hide the current window.
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }

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