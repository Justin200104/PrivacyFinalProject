using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace PrivacyFinalProject.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {

		private TcpClient client;
		private NetworkStream stream;
		private byte[] buffer = new byte[1024];
		public LoginView()
        {
            InitializeComponent();
		}

		private void ConnectToServer()
		{
			client = new TcpClient();
			client.Connect("127.0.0.1", 5537);
            Console.WriteLine("Connected to server");
		}

		//private void RecvMsg()
		//{
		//	while (true)
  //          {
		//		int bytesRead = stream.Read(buffer, 0, buffer.Length);
		//		if (bytesRead == 0)
		//		{
		//			break;
		//		}

		//		string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
		//		Dispatcher.Invoke(() =>
		//		{
		//			// display to screen
		//		});
		//	}
		//}

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
            throw new NotImplementedException();

            // use this to log the suer in after account auth
			//ConnectToServer();
		}

		private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            this.Hide();

            CreateAccountView createAccountView = new CreateAccountView();
            createAccountView.Show();

            createAccountView.Activate();

        }
    }
}
