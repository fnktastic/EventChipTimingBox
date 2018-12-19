using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ECTL
{
    public class ReadingEmulator
    {
        private static string recoveryFileName;
        private static string saltString;
        private static readonly Random getrandom = new Random();
        private Read read;

        public void DisposeServer()
        {
            EmulatedServer.Dispose();
        }

        public void Start(CancellationTokenSource cancellationToken)
        {
            EmulatedServer.Init();
            EmulatedServer.StartReading();

            recoveryFileName = "Spotter";
            saltString = GetRandomInt(10, 10000).ToString();

            read = new Read()
            {
                ID = 0,
                AntennaNumber = "2",
                IpAddress = "127.0.0.1",
                ReaderNumber = "1",
                EPC = "TAG_12",
                UniqueReadingID = Guid.NewGuid().ToString(),
                PeakRssiInDbm = "-11dBm",
                TimingPoint = string.Format("{0}_{1}", recoveryFileName, saltString)
            };

           do
            {
                Thread.Sleep(3000);
                read.Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                read.ID++;
                read.UniqueReadingID = Guid.NewGuid().ToString();
                WriteReadingInFile(read);
                EmulatedServer.OnTagRead(read);
            } while (!cancellationToken.IsCancellationRequested);
        }

        private static bool WriteReadingInFile(Read read)
        {
            using (var fileStream = new FileStream(String.Format("{0}.txt", read.TimingPoint), FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(read.ToString());
            }

            return true;
        }

        private static int GetRandomInt(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }

        }
    }

    public static class EmulatedServer
    {
        private static Server _server;
        private const int DEFAULT_PORT = 10000;

        public static void Init()
        {
            try
            {
                _server = new Server();
            }
            catch
            {
                _server.Dispose();
            }

        }

        public static void StartReading()
        {
            try
            {
                _server.Listen(DEFAULT_PORT);
            }
            catch
            {
                _server.Dispose();
            }
        }

        public static void Dispose()
        {
            _server.Dispose();
        }

        public static void OnTagRead(Read read)
        {
            _server.OnTagRead(read);
        }
    }
}
