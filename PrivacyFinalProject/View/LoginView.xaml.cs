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

                string loginString = $"[LOGIN]{firstName},{lastName},{password}";

                //send account to server
                byte[] buffer = Encoding.UTF8.GetBytes(AES.EncryptString(loginString));
				s.stream.Write(buffer, 0, buffer.Length);
				s.stream.Flush();

				int bytesRead = await s.stream.ReadAsync(buffer, 0, buffer.Length);
                
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                message = AES.DecryptString(message);
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

		private void btnDeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			// Create and show the DeleteAccountView window.
			DeleteAccountView deleteAccountView = new DeleteAccountView();
			deleteAccountView.Show();

			// Bring the new window to the foreground.
			deleteAccountView.Activate();

			// Close the current window or hide it before showing the new window.
			//this.Close(); // Use this if you want to close the current window.
			this.Hide(); // Use this if you just want to hide the current window.
		}

		private void btnPrivacyPolicy_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string pdfFileName = "SECU74010_Project_Privacy_Policy.pdf"; // Update with privacy policy PDF file name
				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
				string pdfFilePath = System.IO.Path.Combine(projectDirectory, "Resources", pdfFileName);

				System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
				{
					FileName = pdfFilePath,
					UseShellExecute = true
				};

				System.Diagnostics.Process.Start(psi);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to open the PDF file. Error: " + ex.Message);
			}
		}
	}
}