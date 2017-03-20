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
                Console.WriteLine("Who do you want to connect to?");
                string user = Console.ReadLine();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(user);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);


                while (true)
                {
                    try
                    {
                        Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        sender.Connect(remoteEP);

                        Console.WriteLine("Socket connected to  {0}", sender.RemoteEndPoint.ToString());
                        string txString = Console.ReadLine();
                        byte[] msg = Encoding.ASCII.GetBytes(txString + "<EOF>");

                        int bytesSent = sender.Send(msg);

                        int bytesRec = sender.Receive(bytes);

                        string rxData = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        rxData = ValidateRxData(rxData);

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

        private static string ValidateRxData(string rxData)
        {
            if (rxData.Contains("<EOF>"))
            {
                return rxData = rxData.Replace("<EOF>", string.Empty);
            }
            else
            {
                throw new Exception("No <EOF> Returned");
            }
        }
    }
}
