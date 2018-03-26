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
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Listen(100);

            Socket newSocket = sock.Accept();

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

            sock.Close();
            sock.Disconnect(true);
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

                //Adresse du serveur auquel on se connecte
                String host = Console.ReadLine(); // 10.4.184.141
                //Port random
                int port = 5000;

                //Socket
                //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //Ca c'est pour udp
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Ca c'est pour tcp
                s.Connect(host, port); 

                //Coucou, toi je sais pas encore qui t'es
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), port);

                //C'est quoi ton p'tit nom ? Et ton adresse IP ?
                Console.WriteLine("Ton nom : ");


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
