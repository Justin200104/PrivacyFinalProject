using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PrivacyFinalProject.Helpers;
using System.Security.Cryptography;
using System;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class DeleteAccountView : Window
    {
		ServerFunctions SF = new ServerFunctions();
		public DeleteAccountView()
        {
            InitializeComponent();
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

        async private void btnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {

            // Get the Credentials
            String firstName = txtUserFirst.Text;
            String lastName = txtUserLast.Text;
            String password = txtPass.Password;
            String confirmPassword = txtConfirmPass.Password;

            // Ensure fields are not empty
            if (firstName.Length > 0 && lastName.Length > 0 && password.Length > 0 && confirmPassword.Length > 0)
            {
                // Ensure passwords are equal
                if (password == confirmPassword)
                {
                    SF.ConnectToServer();

                    string createAccountString = $"[DELETEACCOUNT]{firstName},{lastName},{password}";
                    string strBuffer = AES.EncryptString(createAccountString);
                    byte[] buffer = Encoding.UTF8.GetBytes(strBuffer);
                    SF.stream.Write(buffer, 0, buffer.Length);
                    SF.stream.Flush();

					int bytesRead = await SF.stream.ReadAsync(buffer, 0, buffer.Length);

					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					message = AES.DecryptString(message);
                    if (message == "True")
                    {
						// Create and show the LoginView window.
						LoginView loginView = new LoginView();
						loginView.Show();

						// Bring the new window to the foreground.
						loginView.Activate();

						SF.client.Close();
						// Close the current window or hide it before showing the new window.
						this.Close(); // Use this if you want to close the current window.
	                    // this.Hide(); // Use this if you just want to hide the current window.
					}
                    else
                    {
						SF.client.Close();
						return;
                    }
				}
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the LoginView window.
            LoginView loginView = new LoginView();
            loginView.Show();

            // Bring the new window to the foreground.
            loginView.Activate();

            // Close the current window or hide it before showing the new window.
            this.Close(); // Use this if you want to close the current window.
            // this.Hide(); // Use this if you just want to hide the current window.
        }
    }
}