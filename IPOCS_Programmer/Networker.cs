using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPOCS_Programmer
{
  public class Networker
  {
    private TcpListener listener { get; set; }
    private Thread listenerThread;

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
      this.listenerThread = new Thread(new ThreadStart(this.listenerThreadStart));
      this.listener = new TcpListener(IPAddress.Any, 9999);
    }

    private bool _isListening = false;
    public bool isListening {
      get
      {
        return this._isListening;
      }
      set
      {
        if (value != this.isListening)
        {
          this._isListening = value;
          if (value && !this.listenerThread.IsAlive)
          {
            this.listenerThread = new Thread(new ThreadStart(this.listenerThreadStart));
            this.listenerThread.Start();
          }
          if (!value && this.listenerThread.IsAlive)
          {
            Clients.ForEach((c) =>
            {
              c.Disconnect();
            });
          }
        }
      }
    }

    private void listenerThreadStart()
    {
      this.OnListening?.Invoke(true);
      this.listener.Start();
      while (this.isListening)
      {
        if (!listener.Pending())
        {
          Thread.Sleep(1000);
          continue;
        }
        var tcpClient = this.listener.AcceptTcpClient();
        var client = new Client(tcpClient);
        client.OnDisconnect += (c) => { Clients.Remove(c); OnDisconnect?.Invoke(c); };
        client.OnConnect += (c) => { Clients.Add(c); OnConnect?.Invoke(client); };
        Clients.Add(client);
      }
      this.OnListening?.Invoke(false);
    }

    private List<Client> Clients { get; } = new List<Client>();
  }
}
