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
        static void Server()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 5000);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;
            byte[] data = new byte[1024];
            for (;;)
            {
                int recv = sock.ReceiveFrom(data, ref ep);
                String Message = Encoding.UTF8.GetString(data, 0, recv);
                if (Message == "quit")
                {
                    break;
                }

                Console.WriteLine("Pauline: " + Message);
            }
        }

        static void Main(string[] args)
        {

            String choice = Console.ReadLine();

            if (choice == "1")
            {
                // You are the server
                Console.WriteLine("You are the server");

                Thread t = new Thread(Server);
                t.Start();
            }
            else
            {
                // You are the client
                Console.WriteLine("You are the client");

                String host = Console.ReadLine(); // 10.4.184.141
                int port = 5000;
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //s.Connect(host, port);

                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), 5000);

                for (;;)
                {

                    String Message = Console.ReadLine();

                    s.SendTo(Encoding.UTF8.GetBytes(Message), iep);

                    if (Message == "quit") break;
                }

            }
            Console.WriteLine("Quitting the Tchat");
        }
    }
}
