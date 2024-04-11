using PrivacyFinalProject.Helpers;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
		ServerFunctions s = new ServerFunctions();
		public LoginView()
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

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
			// Get Credentials
			String firstName = txtUserFirst.Text;
            String lastName = txtUserLast.Text;
            String password = txtPass.Password;

            // Ensure fields are not empty
            if (firstName.Length > 0 && lastName.Length > 0 && password.Length > 0)
            {
				s.ConnectToServer();
				// Validate Credentials in DB
				byte[] buffer = Encoding.UTF8.GetBytes($"[LOGIN]{firstName},{lastName},{password}");
				s.stream.Write(buffer, 0, buffer.Length);
				s.stream.Flush();

				int bytesRead = await s.stream.ReadAsync(buffer, 0, buffer.Length);
				string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

				if (message == "True")
                {
					// Create and show the ChatView window.
					ChatView chatView = new ChatView(firstName, lastName, ref s);
					chatView.Show();

					// Bring the new window to the foreground.
					chatView.Activate();

					// Close the current window or hide it before showing the new window.
					this.Close(); // Use this if you want to close the current window.
                }
				else
				{
                    s.client.Close();
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
            //this.Close(); // Use this if you want to close the current window.
            this.Hide(); // Use this if you just want to hide the current window.
        }
    }
}