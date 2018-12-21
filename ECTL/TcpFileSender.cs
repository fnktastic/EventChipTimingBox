using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ECTL
{
    public class TcpFileSender
    {
        const int PORT = 5000;

        public static void Start()
        {
            IPAddress localAdd = IPAddress.Any;
            TcpListener listener = new TcpListener(localAdd, PORT);
            TcpClient tcpClient = new TcpClient();
            Console.WriteLine("Listening...");
            listener.Start();
            try
            {
                while (true)
                {
                    tcpClient = listener.AcceptTcpClient();

                    NetworkStream nwStream = tcpClient.GetStream();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                    int bytesRead = nwStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);

                    string fileName = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received : " + fileName);

                    Console.WriteLine("Sending buffer size");
                    var bytes = ReadFileByName(fileName);
                    var bytesLenght = ASCIIEncoding.ASCII.GetBytes(bytes.Length.ToString());
                    nwStream.Write(bytesLenght, 0, bytesLenght.Length);

                    Console.WriteLine("Sending back Reads");
                    nwStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                tcpClient.Close();
                listener.Stop();
            }
        }

        public static byte[] ReadFileByName(string fileName)
        {
            return File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, fileName));
        }
    }
}

