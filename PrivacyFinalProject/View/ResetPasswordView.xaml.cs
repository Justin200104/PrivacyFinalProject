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
                // Ensure passwords are equal
                if (password != resetPassword)
                {

                    // Reset the account password in the database
                    if (DataBase.ResetPassword(firstName, lastName, password, resetPassword)) {
                        // Create and show the LoginView window.
                        LoginView loginView = new LoginView();
                        loginView.Show();

                        // Bring the new window to the foreground.
                        loginView.Activate();

                        // Close the current window or hide it before showing the new window.
                        this.Close(); // Use this if you want to close the current window.
                                      // this.Hide(); // Use this if you just want to hide the current window.
                    }
                    else { return; }

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

