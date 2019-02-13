using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPOCS.Protocol
{
  public class Message
  {
    public byte RL_MESSAGE;
    public string RXID_OBJECT;
    public List<Packet> packets = new List<Packet>();

    public static Message create(byte[] buffer)
    {
      var msg = new Message();
      msg.RL_MESSAGE = buffer[0];
      byte ll = 0;
      for (int i = 1; i < buffer.Length && buffer[ll] != 0x00; i++)
      {
        ll++;
      }
      msg.RXID_OBJECT = Encoding.UTF8.GetString(buffer, 1, ll - 1);
      ll++;

      var aBuf = new List<byte>(buffer.Skip(ll).ToArray());

      while (aBuf.Count != 0) 
      {
        Packet pkt;
        byte pktLen = Packet.create(out pkt, aBuf);
        aBuf = aBuf.Skip(pktLen).ToList();
        msg.packets.Add(pkt);
      }
      return msg;
    }

    public List<byte> serialize()
    {
      var pkts = new List<byte>();
      pkts.Add(this.RL_MESSAGE);
      pkts.AddRange(Encoding.UTF8.GetBytes(this.RXID_OBJECT));
      pkts.Add(0x00);
      
      foreach (Packet pkt in this.packets)
      {
        pkt.serialize(pkts);
      }
      pkts[0] = (byte)pkts.Count;
      return pkts;
    }

    void addPacket(Packet pkt)
    {
      this.packets.Add(pkt);
    }
  }
}
