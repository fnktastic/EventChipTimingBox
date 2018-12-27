using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static ECTL.SetTimeDlg;

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
            try
            {
                return File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, fileName));
            }
            catch(IOException ex)
            {
                Debug.WriteLine("{0}",ex.Message);
                return new byte[0];
            }
        }
    }

    public class TcpDateTime
    {
        const int PORT = 5001;

        public static void Start()
        {
            IPAddress localAdd = IPAddress.Any;
            TcpListener listener = new TcpListener(localAdd, PORT);
            TcpClient tcpClient = new TcpClient();
            Console.WriteLine("DateTime Listening...");
            listener.Start();
            try
            {
                while (true)
                {
                    tcpClient = listener.AcceptTcpClient();

                    NetworkStream nwStream = tcpClient.GetStream();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                    int bytesRead = nwStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);

                    string action = Encoding.ASCII.GetString(buffer, 0, bytesRead); // action 1 - SendTime, action 2 - GetAndSetTime
                    Console.WriteLine("Received action: " + action);
                    if (action.StartsWith("t1@"))
                        SendTime(nwStream);
                    if (action.StartsWith("t2@"))
                        GetAndSetTime(nwStream, action.Remove(0,3));
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

        private static void SendTime(NetworkStream nwStream)
        {
            try
            {
                string timeNow = DateTime.Now.ToString("HH:mm:ss");
                var bytes = ASCIIEncoding.ASCII.GetBytes(timeNow);
                nwStream.Write(bytes, 0, bytes.Length);                
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private static void GetAndSetTime(NetworkStream nwStream, string timeString)
        {
            string format = "HH:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                DateTime result = DateTime.ParseExact(timeString, format, provider);

                SetSystemTime(result);

                Console.WriteLine("{0} converts to {1}.", timeString, result.ToString());

                var bytes = ASCIIEncoding.ASCII.GetBytes(result.ToString());
                nwStream.Write(bytes, 0, bytes.Length);
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} is not in the correct format.", timeString);
            }
        }

        private static void SetSystemTime(DateTime time)
        {
            var timeStringArray = time.ToString("HH:mm:ss").Split(':');
            SYSTEMTIME lpLocalTime = new SYSTEMTIME();
            lpLocalTime.FromDateTime(DateTime.Now);
            lpLocalTime.Hour = Convert.ToInt16(timeStringArray[0]);
            lpLocalTime.Minute = Convert.ToInt16(timeStringArray[1]);
            lpLocalTime.Second = Convert.ToInt16(timeStringArray[2]);
            SetLocalTime(ref lpLocalTime);
        }
    }
}

