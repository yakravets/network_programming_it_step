using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Client
{
    public partial class MainWindow : Window
    {
        private const int PORT = 2020;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var contact = new ContactDTO
            {
                Email = tbEmail.Text,
                Name = tbName.Text,
                Phone = tbPhone.Text
            };
            
            SendCommand("Create");

            var client = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client.GetStream())
            {

                var serializer = new XmlSerializer(contact.GetType());
                serializer.Serialize(stream, contact);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();

            SendCommand("Read");

            var client = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client.GetStream())
            {

                var serializer = new XmlSerializer(typeof(List<string>));
                var contact = (List<string>)serializer.Deserialize(stream);

                for (int i = 0; i < contact.Count; i++)
                {
                    list.Items.Add(contact[i]);
                }
            }
        }
        private void SendCommand(string command)
        {
            var client = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client.GetStream())
            {
                var serializer1 = new XmlSerializer(typeof(string));
                serializer1.Serialize(stream, command);
            }
            client.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (list.SelectedIndex == -1)
            {
                return;
            }

            SendCommand("Delete");

            var client = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client.GetStream())
            {

                var serializer1 = new XmlSerializer(typeof(int));
                serializer1.Serialize(stream, list.SelectedIndex);
            }
            
            Button_Click_1(sender, e);

        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex == -1)
            {
                return;
            }

            SendCommand("ReadIndex");

            var client = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client.GetStream())
            {

                var serializer1 = new XmlSerializer(typeof(int));
                serializer1.Serialize(stream, list.SelectedIndex);
            }

            ReceiveContact();
            
        }
        public void ReceiveContact()
        {
            SendCommand("SelectionChanged");

            var client2 = new TcpClient(Dns.GetHostName(), PORT);
            using (var stream = client2.GetStream())
            {

                var serializer2 = new XmlSerializer(typeof(ContactDTO));
                var contact = (ContactDTO)serializer2.Deserialize(stream);
                tbEmail.Text = contact.Email;
                tbName.Text = contact.Name;
                tbPhone.Text = contact.Phone;
            }
        }
    }
}
