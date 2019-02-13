using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Makaretu.Dns;

namespace IPOCS
{
    public class Networker
    {
        private TcpListener listener { get; set; }
        private Thread listenerThread;
        private bool _started { get; set; } = false;

        private MulticastService mdns { get; set; }
        private ServiceDiscovery mdnsServiceDiscovery { get; set; }

        private static Networker instance;

        public delegate void OnConnectDelegate(Client client);
        public event OnConnectDelegate OnConnect;
        public event OnConnectDelegate OnDisconnect;

        public delegate void OnListeningDelegate(bool isListening);
        public event OnListeningDelegate OnListening;

        public static Networker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Networker();
                }
                return instance;
            }
        }

        private Networker()
        {
            mdns = new MulticastService((sourceInterfaces) => { return sourceInterfaces; });
            mdnsServiceDiscovery = new ServiceDiscovery(mdns);
            mdnsServiceDiscovery.Advertise(new ServiceProfile("ipocs", "_ipocs._tcp", 10000));

            this.listenerThread = new Thread(new ThreadStart(this.listenerThreadStart));
            this.listener = new TcpListener(IPAddress.Any, 10000);
            this.stopToken.Token.Register(() => listener.Stop());
        }

        ~Networker()
        {
            if (_started)
            {
                this.isListening = false;
                this.listenerThread.Join();
            }
        }


        CancellationTokenSource stopToken = new CancellationTokenSource();
        public bool isListening
        {
            get
            {
                return this.listenerThread.IsAlive;
            }
            set
            {
                if (value != _started)
                {
                    if (value && !_started)
                    {
                        this.listenerThread = new Thread(new ThreadStart(this.listenerThreadStart));
                        this.listenerThread.Start();
                        mdns.Start();
                        _started = value;
                    }
                    if (!value && _started)
                    {
                        Clients.ForEach((c) =>
                        {
                            c.Disconnect();
                        });
                        this.stopToken.Cancel();
                        this.listener.Stop();
                        mdns.Stop();
                        _started = value;
                    }
                }
            }
        }

        private async void listenerThreadStart()
        {
            this.OnListening?.Invoke(true);
            this.listener.Start();
            while (!this.stopToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await Task.Run(() => listener.AcceptTcpClientAsync(), this.stopToken.Token);
                    var client = new Client(tcpClient);
                    client.OnDisconnect += (c) => { Clients.Remove(c); OnDisconnect?.Invoke(c); };
                    client.OnConnect += (c) => { Clients.Add(c); OnConnect?.Invoke(client); };
                    Clients.Add(client);
                } catch (Exception) { }
            }
            this.OnListening?.Invoke(false);
        }

        private List<Client> Clients { get; } = new List<Client>();
    }
}
