using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;


namespace wpf_try
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        
        bool isUserConnect = false;
        int PORT = 8888;
        IPAddress server_ip = IPAddress.Parse("127.0.0.1");
        TcpClient client = null;
        Task task = null;
        void ConnectUser() {
            try
            {
                client = new TcpClient();
                client.Connect(server_ip, PORT);
                Connect.Content = "Disc";
                userMessage.Visibility = Visibility.Visible;
                MessageBox.Visibility = Visibility.Visible;
                isUserConnect = true;
            }
            catch (Exception ex)
            {
            };
        }
        void DisconnectUser() {
            client.Close();
            client = null;
            Connect.Content = "Con";
            userMessage.Visibility = Visibility.Hidden;
            MessageBox.Visibility = Visibility.Hidden;
            isUserConnect = false;
        }


        private void Connect_Click(object sender, RoutedEventArgs e) {
            if (isUserConnect)
            {
                DisconnectUser();
                task = null;
            }
            else
            {
                ConnectUser();
                task = new Task(hearingMsg);
                task.Start();
            }
        }

        public static void hearingMsg()
        {
            var mc = new MainWindow();
            NetworkStream stream = mc.client.GetStream();
            while (mc.task != null)
            {
                if (stream.DataAvailable)
                {
                    Byte[] readingData = new Byte[256];
                    String responseData = String.Empty;
                    StringBuilder completeMessage = new StringBuilder();
                    int numberOfBytesRead = 0;
                    do
                    {
                        numberOfBytesRead = stream.Read(readingData, 0, readingData.Length);
                        completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(readingData, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);
                    responseData = completeMessage.ToString();
                    mc.MessageBox.Text += responseData + "\n";
                }
            }
            stream.Close();

        }
        private void sendMsg_Click(object sender, RoutedEventArgs e)
        {
            NetworkStream stream = client.GetStream();
            Byte[] msg = Encoding.UTF8.GetBytes(userMessage.Text);
            stream.Write(msg, 0, msg.Length);
            MessageBox.Text += userMessage.Text + "\n";
            userMessage.Clear();
            stream.Flush();
            stream.Close();
        }
    }
}
