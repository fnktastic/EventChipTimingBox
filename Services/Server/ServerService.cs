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

        public static async Task SendReadAsync()
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
                    var read = await service.SetReadAsync(new Read() { Id = Guid.NewGuid(), ReadingId = Guid.NewGuid(), EPC = "TAG 14", Time = DateTime.UtcNow });
                }

                catch (Exception ex)
                {
                    (service as ICommunicationObject)?.Abort();
                    Logger.Log.Error(string.Format("{0}: {1}", nameof(SendReadAsync), ex.Message));
                }
            }
        }

        public static async Task SendReadingAsync()
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
                    var reader = await service.SetReaderAsync(new Reader());
                    var reading = await service.SetReadingAsync(new Reading() { Id = Guid.NewGuid(), ReaderId = reader.Id, IPAddress = "192.168.15.125", StartedDateTime = DateTime.UtcNow });
                }
                catch (Exception ex)
                {
                    (service as ICommunicationObject)?.Abort();
                    Logger.Log.Error(string.Format("{0}: {1}", nameof(SendReadAsync), ex.Message));
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
