using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPOCS_Programmer
{
    public class Client
    {
        private TcpClient tcpClient { get; set; }
        private Thread clientReadThread { get; set; }
        private Timer staleTimer { get; set; }
        public byte UnitID { get; private set; } = 0;

        public Client(TcpClient client)
        {
            this.tcpClient = client;
            this.clientReadThread = new Thread(new ThreadStart(this.clientReader));
            this.clientReadThread.Start();
            this.staleTimer = new Timer(new TimerCallback(StaleTimer), null, 1000, Timeout.Infinite);
        }

        public delegate void OnDisconnectDelegate(Client client);
        public event OnDisconnectDelegate OnDisconnect;

        public OnDisconnectDelegate OnConnect;

        public delegate void OnMessageDelegate(IPOCS.Message msg);
        public event OnMessageDelegate OnMessage;
        public ObjectTypes.Concentrator unit { get; private set; } = null;

        void StaleTimer(object state)
        {
            this.tcpClient.Close();
        }

        private void clientReader()
        {
            while (this.tcpClient.Connected)
            {
                var buffer = new byte[255];
                int recievedCount = 0;
                try
                {
                    recievedCount = this.tcpClient.GetStream().Read(buffer, 0, 1);
                }
                catch { this.tcpClient.Close(); continue; }
                if (0 == recievedCount)
                    continue;

                try
                {
                    recievedCount += this.tcpClient.GetStream().Read(buffer, 1, buffer[0] - 1);
                }
                catch { this.tcpClient.Close(); continue; }
                if (0 == recievedCount)
                    continue;

                // Message received. Parse it.
                var message = IPOCS.Message.create(buffer.Take(recievedCount).ToArray());

                // If unit has not yet sent a ConnectionRequest
                if (this.UnitID == 0)
                {
                    var pkt = message.packets.FirstOrDefault((p) => p is IPOCS.Packets.ConnectionRequest) as IPOCS.Packets.ConnectionRequest;
                    if (pkt == null)
                    {
                        // First message must be a Connection Request
                        this.Disconnect();
                        return;
                    }

                    this.UnitID = Encoding.UTF8.GetBytes(message.RXID_OBJECT)[0];


                    // TODO: Validate the site data version.
                    // TODO: Check protocol version.

                    this.staleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    this.unit = MainWindow.Concentrators.FirstOrDefault((c) => c.UnitID == this.UnitID);
                    if (unit == null)
                    {
                        this.Disconnect();
                        break;
                    }

                    var responseMsg = new IPOCS.Message();
                    responseMsg.RXID_OBJECT = Encoding.UTF8.GetString(new byte[] { this.UnitID });
                    responseMsg.packets.Add(new IPOCS.Packets.ConnectionResponse
                    {
                        RM_PROTOCOL_VERSION = pkt.RM_PROTOCOL_VERSION
                    });
                    this.Send(responseMsg);

                    List<byte> data = this.unit.Serialize();
                    var crc = new ccit_crc16(InitialCrcValue.NonZero1);
                    ushort computedChecksum = crc.ComputeChecksum(data.ToArray());
                    ushort providedChecksum = (ushort)Convert.ToInt32(pkt.RXID_SITE_DATA_VERSION, 16);
                    
                    if (providedChecksum == 0 || computedChecksum != providedChecksum)
                    {
                        responseMsg = new IPOCS.Message();
                        responseMsg.RXID_OBJECT = Encoding.UTF8.GetString(new byte[] { this.UnitID });
                        responseMsg.packets.Add(new IPOCS.Packets.ApplicationData
                        {
                            RNID_XUSER = 0x0001,
                            PAYLOAD = data.ToArray()
                        });
                        this.Send(responseMsg);
                        this.Disconnect();
                        break;
                    } else
                        OnConnect?.Invoke(this);
                }
                else
                    // And if not, hand it to listeners
                    OnMessage?.Invoke(message);
            }
            OnDisconnect?.Invoke(this);
        }

        public void Disconnect()
        {
            this.tcpClient.Close();
        }

        public void Send(IPOCS.Message msg)
        {
            var buffer = msg.serialize().ToArray();
            this.tcpClient.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}
