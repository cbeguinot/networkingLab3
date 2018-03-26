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
            int port = 5000;

            TcpListener server = new TcpListener(IPAddress.Any, port);

            server.Start();

            TcpClient client = server.AcceptTcpClient();

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

                if(nbOfData == 0)
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

        static void Main(string[] args)
        {

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
