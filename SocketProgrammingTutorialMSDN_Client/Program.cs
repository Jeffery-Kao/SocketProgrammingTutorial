using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketProgrammingTutorialMSDN_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
            Console.ReadLine();
        }

        private static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                while (true)
                {
                    try
                    {
                        sender.Connect(remoteEP);

                        Console.WriteLine("Socket connected to  {0}", sender.RemoteEndPoint.ToString());
                        string txString = Console.ReadLine();
                        byte[] msg = Encoding.ASCII.GetBytes( txString + "<EOF>");

                        int bytesSent = sender.Send(msg);

                        int bytesRec = sender.Receive(bytes);

                        string rxData = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine("Echoed test = {0}", rxData);

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
