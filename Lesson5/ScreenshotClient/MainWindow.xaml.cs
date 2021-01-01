using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenshotClient
{
    public partial class MainWindow : Window
    {
        private const int Port = 2020;
        private const int MillisecondsTimeout = 3000;
        private const int BlockSize = 8096;
        private const int TimeoutWithoutPackets = 50;
        static string host = "127.0.0.1";

        bool sendScreenShot = false;
        UdpClient udpClient = null;
        string filename;
        string fullPath;
        int counter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Start.
            sendScreenShot = true;

            udpClient = new UdpClient();
            udpClient.Connect(host, Port);

            Task task = new Task(SendPicture);
            task.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Stop.
            sendScreenShot = false;

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }

        void SendPicture()
        {
            do
            {
                filename = $"ScreenCapture- { DateTime.Now.ToString("ddMMyyyy-hhmmss") }.png";
                fullPath = Directory.GetCurrentDirectory() + "\\screenshots\\" + filename;

                using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                            Screen.PrimaryScreen.Bounds.Height))
                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                     Screen.PrimaryScreen.Bounds.Y,
                                     0, 0,
                                     bmpScreenCapture.Size,
                                     CopyPixelOperation.SourceCopy);
                    bmpScreenCapture.Save(fullPath);
                }

                // Send file to server.
                var fileNameByte = Encoding.UTF8.GetBytes(filename);
                udpClient.Send(fileNameByte, fileNameByte.Length);

                FileInfo fileInfo = new FileInfo(fullPath);
                var fileSize = fileInfo.Length;
                var FileSizeInBytes = Encoding.UTF8.GetBytes(fileSize.ToString());

                udpClient.Send(FileSizeInBytes, FileSizeInBytes.Length);

                var binaryFile = File.ReadAllBytes(fullPath);

                byte[] buffer;
                for (int i = 0; i < binaryFile.Length;)
                {
                    int k = BlockSize < (binaryFile.Length - i) ? BlockSize : binaryFile.Length - i;

                    if (k != BlockSize)
                    {

                    }
                    buffer = binaryFile.Skip(i).Take(k).ToArray();
                    udpClient.Send(buffer, buffer.Length);

                    i += k;

                    Thread.Sleep(TimeoutWithoutPackets);
                }

                counter++;

                Dispatcher.Invoke(() => count.Content = counter);

                // Remove sended file.
                File.Delete(fullPath);

                // Sleep.
                Thread.Sleep(MillisecondsTimeout);

            } while (sendScreenShot);
        }
    }
}
