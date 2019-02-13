using System;
using System.Collections.Generic;
using System.Linq;

namespace IPOCS.Protocol
{
  public abstract class Packet
  {
    public static byte RNID_PACKET { get { return 0; } }
    public byte RL_PACKET;
    public byte RM_ACK;

    protected abstract byte parseSpecific(List<byte> buffer);
    protected abstract byte serializeSpecific(List<byte> buffer);

    public static Packet create(byte RNID_PACKET)
    {
      var type = typeof(Packet);
      var types = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(s => s.GetTypes())
          .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);
      var packetType = types.SingleOrDefault((p) => {
        var prop = p.GetProperty("RNID_PACKET");
        var inst = Activator.CreateInstance(p);
        var val = (byte)prop.GetValue(inst);
        return val == RNID_PACKET;
        });
      if (packetType == null)
        throw new NotImplementedException("Unknown packet id");
      return (Packet)Activator.CreateInstance(packetType);
    }

    public static byte create(out Packet pkt, List<byte> buffer)
    {
      pkt = Packet.create(buffer[0]);
      var rnid = (byte)pkt.GetType().GetProperty("RNID_PACKET").GetValue(pkt);
      if (buffer[0] != rnid)
        throw new ArgumentOutOfRangeException("Unknown packet ID");
      pkt.RL_PACKET = buffer[1];
      pkt.RM_ACK = buffer[2];
      return (byte)(3 + pkt.parseSpecific(buffer.Skip(3).ToList()));
    }

    public byte serialize(List<byte> buffer)
    {
      var rnid = (byte)this.GetType().GetProperty("RNID_PACKET").GetValue(this);
      var b = new List<byte>();
      b.Add(rnid);
      b.Add(this.RL_PACKET);
      b.Add(this.RM_ACK);
      this.serializeSpecific(b);
      b[1] = (byte)b.Count;

      buffer.AddRange(b);
      return b[1];
    }

    protected abstract string stringify();

    public override string ToString()
    {
      return this.stringify();
    }
  }
}
