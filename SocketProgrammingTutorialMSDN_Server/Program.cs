using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketProgrammingTutorialMSDN_Server
{
    class Program
    {
        public static string data = null;

        static void Main(string[] args)
        {
            StartServer();
            Console.ReadLine();
        }

        private static void StartServer()
        {
            byte[] bytes = new byte[1024];

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listner = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listner.Bind(localEndPoint);
                listner.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");

                    Socket handler = listner.Accept();
                    data = null;
                    Console.WriteLine("Connected to Remote End Point: " + handler.RemoteEndPoint);

                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
