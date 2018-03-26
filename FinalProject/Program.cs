using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalProject
{
    class Program
    {
        private static void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;

            NetworkStream stream = client.GetStream();

            Console.WriteLine("New user");

            int nbOfData = 0;

            String name = "";

            for (;;)
            {
                Byte[] data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                if (nbOfData == 0)
                {
                    name = responseData;
                }

                if (responseData == "quit")
                {
                    break;
                }

                Console.WriteLine(name + ": " + responseData);

                nbOfData++;
            }

            stream.Close();

            client.Close();
        }

        static void Server()
        {
            int port = 5000;

            TcpListener server = new TcpListener(IPAddress.Any, port);
            
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
            }
            
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1 pour serveur, 2 pour client : ");
            String choice = Console.ReadLine();

            if (choice == "1")
            {
                // You are the server
                Console.WriteLine("You are the server");

                Server();
            }
            else
            {
                // You are the client
                Console.WriteLine("You are the client");

                //Adresse du serveur auquel on se connecte
                String host = Console.ReadLine(); // 10.4.184.141
                int port = 5000; //Port random

                //Socket
                        //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //Ca c'est pour udp
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Ca c'est pour tcp

                //Pour tcp
                TcpClient tcpClient = new TcpClient(host, port);
                NetworkStream stream = tcpClient.GetStream();
            
                        //Pour udp
                        //s.Connect(host, port);
                        //IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), port);

                Console.WriteLine("Ton nom : ");
                
                String name = Console.ReadLine();
                Byte[] n_data = System.Text.Encoding.ASCII.GetBytes(name);
                stream.Write(n_data, 0, n_data.Length);

                for (;;)
                {
                    Console.Write(name + " : ");
                    String Message = Console.ReadLine();
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(Message);
                    stream.Write(data, 0, data.Length);
                    //s.SendTo(Encoding.UTF8.GetBytes(Message), iep);

                    if (Message == "quit") break;
                }

                stream.Close();

            }
            Console.WriteLine("Quitting the Tchat");
        }
    }
}
