using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketProgrammingTutorial1_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient clientSocket = new TcpClient();
            Console.WriteLine(" >> Client Started");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];

            clientSocket.Connect(ipAddress, 11000);
            Console.WriteLine(" >> Client Socket Program -  server Connected.... ");

            while (true)
            {
                string clientResponse = Console.ReadLine();

                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(clientResponse + "$");

                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
                string returnedData = Encoding.ASCII.GetString(inStream);
                returnedData = returnedData.Substring(0, returnedData.IndexOf("$"));
                Console.WriteLine(" >> " + returnedData);
            }
        }
    }
}
