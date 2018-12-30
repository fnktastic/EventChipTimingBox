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
        private static string recoveryFileName = string.Format("{0}$_{1}.txt", "Spotter", DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss"));
        private Read read;

        public void DisposeServer()
        {
            EmulatedServer.Dispose();
        }

        public void StopServer()
        {
            EmulatedServer.Stop();
        }

        public void Start(CancellationTokenSource cancellationToken)
        {
            try
            {
                EmulatedServer.Init(recoveryFileName);
                EmulatedServer.StartReading();

                read = new Read()
                {
                    ID = 0,
                    AntennaNumber = "2",
                    IpAddress = "127.0.0.1",
                    ReaderNumber = "1",
                    EPC = "TAG_12",
                    UniqueReadingID = Guid.NewGuid().ToString(),
                    PeakRssiInDbm = "-11dBm",
                    TimingPoint = "Spotter"
                };

                do
                {
                    Thread.Sleep(1500);
                    read.Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    read.ID++;
                    read.UniqueReadingID = Guid.NewGuid().ToString();
                    WriteReadingInFile(read);
                    EmulatedServer.OnTagRead(read);
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Start), ex.Message, ex.StackTrace));
            }
        }

        private static bool WriteReadingInFile(Read read)
        {
            try
            {
                using (var fileStream = new FileStream(recoveryFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(read.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(WriteReadingInFile), ex.Message, ex.StackTrace));
                return false;
            }
        }
    }

    public static class EmulatedServer
    {
        private static Server _server;
        private const int DEFAULT_PORT = 10000;

        public static void Init(string recoveryFile)
        {
            try
            {
                _server = new Server();
                _server.RecoveryFile = recoveryFile;
            }
            catch
            {
                _server.Dispose();
            }

        }

        public static void Stop()
        {
            _server.SendAll(Encoding.ASCII.GetBytes("!STOP!"));
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
