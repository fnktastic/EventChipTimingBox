namespace ECTL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal sealed class MultiServer : Disposable
    {
        private List<TcpClient> _clients = new List<TcpClient>();
        private Dictionary<string, List<string>> _lostClients = new Dictionary<string, List<string>>();
        private TcpListener _listener;
        private const string ALL_CLIENTS = "unknown";
        private string currentReading = string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AsyncCallback _onAsyncReadComplete;

        public MultiServer()
        {
            this._onAsyncReadComplete = new AsyncCallback(this.OnAsyncReadComplete);
        }

        protected override void Dispose(bool bDisposing)
        {
            lock (this)
            {
                Utils.ToArray<TcpClient>((ICollection<TcpClient>)this._clients);
                this._clients.Clear();
            }
            foreach (TcpClient client in this._clients)
            {
                try
                {
                    client.Close();
                }
                catch
                {
                }
            }
            lock (this)
            {
                if (this._listener != null)
                {
                    try
                    {
                        this._listener.Stop();
                    }
                    catch
                    {
                    }
                    this._listener = null;
                }
            }
            base.Dispose(bDisposing);
        }

        private void FlushAll()
        {
            foreach (TcpClient client in Utils.ToArray<TcpClient>((ICollection<TcpClient>)this._clients))
            {
                try
                {
                    client.GetStream().Flush();
                }
                catch
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(client))
                        {
                            this._clients.Remove(client);
                        }
                    }
                }
            }
        }

        private static long GetUnixTime(DateTime dt)
        {
            DateTime time = new DateTime(0x7bc, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = (TimeSpan)(dt - time);
            return Convert.ToInt64(span.TotalSeconds);
        }

        public bool Listen(int nPort)
        {
            return this.Listen(nPort, null);
        }

        public bool Listen(int nPort, string strHost)
        {
            IPAddress any = IPAddress.Any;
            if (strHost.Validate() && (string.Compare("localhost", strHost, true) != 0))
            {
                try
                {
                    any = IPAddress.Parse(strHost);
                }
                catch
                {
                }
            }
            try
            {
                this._listener = new TcpListener(any, nPort);
                this._listener.Start();
                this._listener.BeginAcceptTcpClient(new AsyncCallback(this.OnAsyncClientAccepted), this._listener);
                return true;
            }
            catch
            {
            }
            return false;
        }

        private void OnAsyncClientAccepted(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient item = (asyncResult.AsyncState as TcpListener).EndAcceptTcpClient(asyncResult);
                this._listener.BeginAcceptTcpClient(new AsyncCallback(this.OnAsyncClientAccepted), this._listener);
                this._clients.Add(item);
                try
                {
                    ReadContext state = new ReadContext(item);
                    item.GetStream().BeginRead(state.Buffer, 0, state.Buffer.Length, this._onAsyncReadComplete, state);
                }
                catch
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(item))
                        {
                            this._clients.Remove(item);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void OnAsyncReadComplete(IAsyncResult asyncResult)
        {
            ReadContext asyncState = asyncResult.AsyncState as ReadContext;
            NetworkStream stream = null;
            try
            {
                stream = asyncState.Client.GetStream();
                stream.EndRead(asyncResult);
                stream.BeginRead(asyncState.Buffer, 0, asyncState.Buffer.Length, this._onAsyncReadComplete, asyncState);
            }
            catch
            {
                lock (this._clients)
                {
                    if (this._clients.Contains(asyncState.Client) && _lostClients.Keys.Contains(asyncState.Client.GetIP()) == false)
                    {
                        _lostClients.Add(asyncState.Client.GetIP(), new List<string> { currentReading });
                        this._clients.Remove(asyncState.Client);
                    }
                }
            }
        }

        public void OnTagRead(Read readToSend)
        {
            currentReading = readToSend.ToString();

            if (_clients.Count == 0)
                foreach (var lostClient in _lostClients)
                    lostClient.Value.Add(currentReading);

            if (this._clients.Count > 0)
            {
                if (_lostClients.Count > 0)
                {
                    foreach (var lostClient in _lostClients)
                    {
                        if (lostClient.Key == ALL_CLIENTS)
                        {
                            lostClient.Value.ForEach(x =>
                            {
                                SendAll(Encoding.ASCII.GetBytes(x));
                            });
                        }

                        else if (_clients.FirstOrDefault(x => x.GetIP() == lostClient.Key) != null)
                        {
                            var client = _clients.FirstOrDefault(x => x.GetIP() == lostClient.Key);
                            // send to one client
                            lostClient.Value.ForEach(x =>
                            {
                                SendAll(Encoding.ASCII.GetBytes(x));
                            });
                        }
                    }

                    _lostClients.Clear();
                }

                SendAll(Encoding.ASCII.GetBytes(currentReading));
                Console.WriteLine(currentReading);
            }

        }

        private void SendToSelected(byte[] data)
        {

        }

        private void SendAll(byte[] data)
        {
            foreach (TcpClient client in Utils.ToArray<TcpClient>((ICollection<TcpClient>)this._clients))
            {
                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                }
                catch
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(client))
                        {
                            this._clients.Remove(client);
                        }
                    }
                }
            }
        }

        private sealed class ReadContext
        {
            public readonly byte[] Buffer = new byte[0x100];
            public readonly TcpClient Client;

            public ReadContext(TcpClient client)
            {
                this.Client = client;
            }
        }
    }
}

