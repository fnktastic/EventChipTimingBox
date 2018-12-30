namespace ECTL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal sealed class Server : Disposable
    {
        private List<TcpClient> _clients = new List<TcpClient>();
        private TcpListener _listener;
        private string currentReading = string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AsyncCallback _onAsyncReadComplete;
        public string RecoveryFile { private get; set; }

        public Server()
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
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Dispose), ex.Message, ex.StackTrace));
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
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Dispose), ex.Message, ex.StackTrace));
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
                catch (Exception ex)
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(client))
                        {
                            this._clients.Remove(client);
                        }
                    }
                    Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.FlushAll), ex.Message, ex.StackTrace));
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
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Listen), ex.Message, ex.StackTrace));
                }
            }
            try
            {
                this._listener = new TcpListener(any, nPort);
                this._listener.Start();
                this._listener.BeginAcceptTcpClient(new AsyncCallback(this.OnAsyncClientAccepted), this._listener);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Listen), ex.Message, ex.StackTrace));
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

                    var recoveryFile = Encoding.ASCII.GetBytes(RecoveryFile);
                    item.GetStream().Write(recoveryFile, 0, recoveryFile.Length);

                    item.GetStream().BeginRead(state.Buffer, 0, state.Buffer.Length, this._onAsyncReadComplete, state);
                }
                catch (Exception ex)
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(item))
                        {
                            this._clients.Remove(item);
                        }
                    }

                    Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.OnAsyncClientAccepted), ex.Message, ex.StackTrace));
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
            catch (Exception ex)
            {
                lock (this._clients)
                {
                    if (this._clients.Contains(asyncState.Client))
                    {
                        this._clients.Remove(asyncState.Client);
                    }
                }

                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.OnAsyncReadComplete), ex.Message, ex.StackTrace));
            }
        }

        public void OnTagRead(Read readToSend)
        {
            try
            {
                if (this._clients.Count >= 1)
                {
                    currentReading = readToSend.ToString();
                    SendAll(Encoding.ASCII.GetBytes(currentReading));
                }
            } 
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.OnTagRead), ex.Message, ex.StackTrace));
            }
        }

        public void SendAll(byte[] data)
        {
            foreach (TcpClient client in Utils.ToArray<TcpClient>((ICollection<TcpClient>)this._clients))
            {
                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    lock (this._clients)
                    {
                        if (this._clients.Contains(client))
                        {
                            this._clients.Remove(client);
                        }
                    }

                    Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.SendAll), ex.Message, ex.StackTrace));
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

