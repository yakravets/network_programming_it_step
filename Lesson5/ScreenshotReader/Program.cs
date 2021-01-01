using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ScreenshotReader
{
    class Program
    {
        const int PORT = 2020;

        private static string directoryPath = null;

        static void Main(string[] args)
        {            
            Console.Title = "Server:";

            if (!Directory.Exists("screens"))
            {
                Directory.CreateDirectory("screens");
                Console.WriteLine("Directory \"screens\" created success");
            }

            directoryPath = Directory.GetCurrentDirectory() + "\\screens";

            Console.WriteLine("Wait...");

            var server = new UdpClient(PORT);
            try
            {
                IPEndPoint ep = null;

                while (true)
                {
                    var data = server.Receive(ref ep);
                    var fileName = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"Filename: {fileName}");

                    data = server.Receive(ref ep);
                    var fileSize = Int32.Parse(Encoding.UTF8.GetString(data));
                    
                    Console.WriteLine($"Size: {fileSize}");
                    Console.WriteLine($"Need packets: {fileSize / 8096.0 }");

                    var buffer = new byte[fileSize];

                    int offset = 0;
                    int packet = 0;
                    do
                    {
                        data = server.Receive(ref ep);
                        for (int j = 0; j < data.Length; j++)
                        {
                            try
                            {
                                buffer[j + offset] = data[j];
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }                            
                        }

                        offset += data.Length;
                        Console.WriteLine($"Offset: {offset}");
                        Console.WriteLine($"Packet: {packet + 1}");
                        Console.WriteLine($"Packet size: {data.Length}");

                        packet++;

                    } while (fileSize > offset);


                    File.WriteAllBytes(directoryPath + "\\" + fileName, buffer);

                    Console.WriteLine("Received ok..");
                }
            }
            catch (SocketException e)
            {
                server.Close();
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
