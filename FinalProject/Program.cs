using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalProject
{
    class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();

        public static void broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }

        private static void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;

            NetworkStream stream = client.GetStream();
            
            var pi = stream.GetType().GetProperty("Socket", BindingFlags.NonPublic | BindingFlags.Instance);
            var socketIp = ((Socket)pi.GetValue(stream, null)).RemoteEndPoint.ToString();
            Console.WriteLine(socketIp);

            broadcast("New user connected from: " + socketIp);

            int nbOfData = 0;

            String name = "Un utilisateur";

            broadcast(name + " a rejoint le tchat...");

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
                else
                {
                    String res = name + ": " + responseData;
                    broadcast(res);
                }

                if (responseData == "quit")
                {
                    break;
                }

                nbOfData++;
            }

            broadcast(name + " a quitté le tchat...");

            stream.Close();

            client.Close();
        }

        static void Server()
        {
            int idOfClients = 1;

            int port = 5000;

            TcpListener server = new TcpListener(IPAddress.Any, port);
            
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (_lock) list_clients.Add(idOfClients, client);
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
                idOfClients++;
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

                Thread thread = new Thread(o => ReceiveData((TcpClient)o));

                thread.Start(tcpClient);

                //Pour udp
                //s.Connect(host, port);
                //IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), port);

                //Thread t = new Thread(ReceiveBroadcast);

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
