﻿using System.IO;
using System.Windows;
using System.Windows.Input;
using PrivacyFinalProject.Helpers;

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccountView : Window
    {
        public CreateAccountView()
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

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
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

                    if (chkConsent.IsChecked ?? false)
                    {
                        // Create the account in the database
                        DataBase.InsertData(firstName, lastName, password);

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

        private void btnPrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfFileName = "SECU74010_Project.pdf"; // Update with privacy policy PDF file name
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
