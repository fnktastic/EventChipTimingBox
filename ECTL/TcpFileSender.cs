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
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";

        public static void Start()
        {
                //---listen at the specified IP and port no.---
                IPAddress localAdd = IPAddress.Parse(SERVER_IP);
                TcpListener listener = new TcpListener(localAdd, PORT_NO);
                TcpClient tcpClient = new TcpClient();
                Console.WriteLine("Listening...");
                listener.Start();
            try
            {
                while (true)
                {
                    //---incoming client connected---
                    tcpClient = listener.AcceptTcpClient();

                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = tcpClient.GetStream();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = nwStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string fileName = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received : " + fileName);

                    //---write back the text to the client---
                    Console.WriteLine("Sending buffer size");
                    var bytes = ReadFileByName(fileName);
                    var bytesLenght = ASCIIEncoding.ASCII.GetBytes(bytes.Length.ToString());
                    nwStream.Write(bytesLenght, 0, bytesLenght.Length);

                    //---write back the text to the client---
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

