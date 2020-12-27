using Client;
using DAL;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server
{

    class Program
    {
        private const int port = 2020;
        private static IPAddress ip;
        private static TcpListener server;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            StartService();
        }

        private static void StartService()
        {
            int index = 0;
            var dbHelper = new DBHelper();

            ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]);
            
            server = new TcpListener(ip, port);
            server.Start();
            
            string clienCommand;
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for connecting...");
                    var client = server.AcceptTcpClient();

                    Console.WriteLine("Connected");
                    using (var stream = client.GetStream())
                    {
                        var serializer1 = new XmlSerializer(typeof(string));
                        clienCommand = (string)serializer1.Deserialize(stream);
                        Console.WriteLine(clienCommand);
                    }

                    client = server.AcceptTcpClient();
                    using (var stream = client.GetStream())
                    {
                        if (clienCommand == "Create")
                        {
                            var serializer2 = new XmlSerializer(typeof(ContactDTO));
                            var contact = (ContactDTO)serializer2.Deserialize(stream);
                            Contact c = new Contact
                            {
                                Email = contact.Email,
                                Name = contact.Name,
                                Phone = contact.Phone
                            };
                            dbHelper.AddContact(c);
                        }
                        else if (clienCommand == "Read")
                        {
                            var serializer = new XmlSerializer(typeof(List<string>));
                            var res2 = dbHelper.GetName();
                            serializer.Serialize(stream, res2);
                        }
                        else if (clienCommand == "Delete")
                        {
                            var serializer2 = new XmlSerializer(typeof(int));
                            index = (int)serializer2.Deserialize(stream);
                            Contact c = dbHelper.GetContact(index);
                            dbHelper.DeleteContact(c);
                        }
                        else if (clienCommand == "ReadIndex")
                        {
                            var serializer2 = new XmlSerializer(typeof(int));
                            index = (int)serializer2.Deserialize(stream);
                        }
                        else
                        {
                            Contact contact = dbHelper.GetContact(index);
                            ContactDTO DTOValue = new ContactDTO()
                            {
                                Email = contact.Email,
                                Name = contact.Name,
                                Phone = contact.Phone
                            };
                            var serializer = new XmlSerializer(typeof(ContactDTO));
                            serializer.Serialize(stream, DTOValue);
                        }
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
