using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketProgrammingTutorialMSDN_AsyncClient
{
    public class StateObject
    {
        public Socket workSocket = null;

        public const int BufferSize = 256;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

    }

    public class AsynchonousClient
    {
        private const int port = 11000;

        //The ManualReset Envent is to for different threads to know when each portion is done.
        private static ManualResetEvent connectDone = new ManualResetEvent(false); //False so that initially it is in a non signaled state, so calling waitOne() will pause it's execution at that point until set() is called.
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private static string response = string.Empty;
        private static Socket client = null;
        public static void StartClient()
        {
            try
            {
                Console.WriteLine("Please Enter the Server Name.....");
                string userInput = Console.ReadLine();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(userInput);
                IPAddress ipAddress = ipHostInfo.AddressList.Where(x => x.AddressFamily.Equals(AddressFamily.InterNetwork)).FirstOrDefault();
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                

                while (true)
                {
                    connectDone.Reset();
                    sendDone.Reset();
                    receiveDone.Reset();

                    client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnetCallback), client);
                    connectDone.WaitOne();

                    Console.WriteLine("Type a message and press ENTER to send it to the Server....");
                    string txString = Console.ReadLine();
                    Send(client, txString + "<EOF>");
                    sendDone.WaitOne();

                    Recieve(client);
                    receiveDone.WaitOne();

                    Console.WriteLine("Response Recieved: {0}", response);

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //Asynchonous method that is called once the connect is completed.
        private static void ConnetCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket Connected to {0}", client.RemoteEndPoint.ToString());

                connectDone.Set(); // connectDone ManualResetEvent will stop blocking the thread now.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Recieve(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(RecieveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RecieveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
            }
            else
            {
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                    //WriteRxLog(response);
                }
                receiveDone.Set();
            }
        }

        //private static void WriteRxLog(string response)
        //{
        //    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\Rx.Log", true))
        //    {
        //        DateTime Now = DateTime.Now;
        //        writer.Write(Environment.NewLine + Now.ToString());
        //        writer.Write(" ---> ");
        //        writer.Write(response);
        //    }
        //}

        private static void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);

                Console.WriteLine("Sent {0} bytes to the server. ", bytesSent);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static int Main(string[] args)
        {
            StartClient();
            return 0;
        }
    }
}
