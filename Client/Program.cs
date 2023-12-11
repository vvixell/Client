using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static string Username;

        static TcpClient Client;

        static int port = 13000;//11160;

        static void Main(string[] args)
        {
            Console.WriteLine("What is your username? :");
            Username = Console.ReadLine();

            Connect("127.0.0.1");
        }

        static void Connect(string server)
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(Leave);
                Client = new TcpClient(server, port);
                SendMessage(Username);

                Thread ReadStreamThread = new Thread(ReadClientStream);
                ReadStreamThread.Start();

                while (true)
                {
                    string message = Console.ReadLine();
                    SendMessage(message);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void SendMessage(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            Client.GetStream().Write(data, 0, data.Length);
        }

        static void ReadClientStream()
        {
            try
            {
                while (true)
                {
                    using (StreamReader sr = new StreamReader(Client.GetStream(), System.Text.Encoding.ASCII, true, 1, true))
                    {
                        string line = sr.ReadLine();

                        Console.WriteLine(line);
                    }
                }
            }
            catch(Exception e)
            {

            }
        }

        static void Leave(Object sender, EventArgs e)
        {
            SendMessage("/Leave");
            Client.Client.Close();
            Client.Close();
        }
    }
}
