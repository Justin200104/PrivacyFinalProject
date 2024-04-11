using System.Text;
using System.Windows;
using System.Windows.Input;
using PrivacyFinalProject.Helpers;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for ResetPasswordView.xaml
    /// </summary>
    public partial class ResetPasswordView : Window
    {
        ServerFunctions SF = new ServerFunctions();
		public ResetPasswordView()
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

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Get the Credentials
            String firstName = txtUserFirst.Text;
            String lastName = txtUserLast.Text;
            String password = txtPass.Password;
            String resetPassword = txtResetPass.Password;

            // Ensure fields are not empty
            if (firstName.Length > 0 && lastName.Length > 0 && password.Length > 0 && resetPassword.Length > 0)
            {
                // Ensure passwords are not equal
                if (password != resetPassword)
                {
                    SF.ConnectToServer();
					//send account to server
					byte[] buffer = Encoding.UTF8.GetBytes($"[RESETPASSWORD]{firstName},{lastName},{password},{resetPassword}");
					SF.stream.Write(buffer, 0, buffer.Length);
					SF.stream.Flush();

					int bytesRead = SF.stream.Read(buffer, 0, buffer.Length);
					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

					if (message == "True")
					{
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