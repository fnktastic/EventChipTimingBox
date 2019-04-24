using AtwService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Services.Server
{
    public static class ServerService
    {
        private static dynamic binding = null;
        private static dynamic endpoint = null;

        public static async void SendReadAsync(Guid readingId, DateTime capturedTime, string epc, string signal, string antennaNumber)
        {
            IService service = null;
            using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
            {
                channelFactory.Credentials.UserName.UserName = string.Empty;
                channelFactory.Credentials.UserName.Password = string.Empty;

                service = channelFactory.CreateChannel();
                try
                {
                    service = channelFactory.CreateChannel();
                    var read = await service.SetReadAsync(new Read()
                    {
                        Id = Guid.NewGuid(),
                        ReadingId = readingId,
                        EPC = epc,
                        Time = capturedTime,
                        Signal = signal,
                        AntennaNumber = antennaNumber
                    });
                }

                catch (Exception ex)
                {
                    (service as ICommunicationObject)?.Abort();
                    Logger.Log.Error(string.Format("{0}: {1}", nameof(SendReadAsync), ex.Message));
                }
            }
        }

        public static async Task<Reading> SendReadingAsync(int readerId, string ipAdress, string readerNumber, string timingPoint, string userId, string raceId)
        {
            IService service = null;
            using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
            {
                channelFactory.Credentials.UserName.UserName = string.Empty;
                channelFactory.Credentials.UserName.Password = string.Empty;

                service = channelFactory.CreateChannel();
                try
                {
                    service = channelFactory.CreateChannel();

                    var reading = await service.SetReadingAsync(new Reading()
                    {
                        Id = Guid.NewGuid(),
                        ReaderNumber = readerNumber,
                        TimingPoint = timingPoint,
                        ReaderId = readerId,
                        IPAddress = ipAdress,
                        UserName = userId,
                        RaceName = raceId,
                        StartedDateTime = DateTime.Now
                    });

                    return reading;
                }
                catch (Exception ex)
                {
                    (service as ICommunicationObject)?.Abort();
                    Logger.Log.Error(string.Format("{0}: {1}", nameof(SendReadAsync), ex.Message));

                    return null;
                }
            }
        }

        public static async Task<Reader> SendReaderAsync(string host, string port)
        {
            IService service = null;
            using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
            {
                channelFactory.Credentials.UserName.UserName = string.Empty;
                channelFactory.Credentials.UserName.Password = string.Empty;

                service = channelFactory.CreateChannel();
                try
                {
                    service = channelFactory.CreateChannel();

                    var reader = await service.SetReaderAsync(new Reader() { Host = host, Port = port });

                    return reader;
                }
                catch (Exception ex)
                {
                    (service as ICommunicationObject)?.Abort();
                    Logger.Log.Error(string.Format("{0}: {1}", nameof(SendReadAsync), ex.Message));

                    return null;
                }
            }
        }

        public static void InitProtocol(ProtocolEnum protocol)
        {
            try
            {
                if (protocol == ProtocolEnum.Http)
                {
                    binding = new WSHttpBinding(SecurityMode.None);
                    endpoint = new EndpointAddress(Consts.HttpUrl());
                }
                if (protocol == ProtocolEnum.Tcp)
                {
                    binding = new NetTcpBinding(SecurityMode.None);
                    endpoint = new EndpointAddress(Consts.TcpUrl());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("{0}: {1}", nameof(InitProtocol), ex.Message));
            }
        }
    }
}
