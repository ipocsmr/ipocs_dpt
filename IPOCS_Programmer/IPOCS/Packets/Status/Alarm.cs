using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public enum RQ_ALARM_STATE
  {
    ACTIVE = 1,
    NOT_PRESENT = 2,
    TRANSIENT = 3
  };

  public class Alarm : Packet
  {
    public static new byte RNID_PACKET { get { return 16; } }
    public short RQ_ALARM_CODE;
    public byte RN_ALARM_LEVEL;
    public RQ_ALARM_STATE RQ_ALARM_STATE;
    public uint RN_PAR_1;
    public uint RN_PAR_2;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_ALARM_CODE = (short)((buffer[0] << 8) + buffer[1]);
      this.RN_ALARM_LEVEL = buffer[2];
      this.RQ_ALARM_STATE = (RQ_ALARM_STATE)buffer[3];
      this.RN_PAR_1 = (uint)((buffer[4] << 24) + (buffer[5] << 16) + (buffer[6] << 8) + buffer[7]);
      this.RN_PAR_2 = (uint)((buffer[8] << 24) + (buffer[9] << 16) + (buffer[10] << 8) + buffer[11]);
      return 12;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)(this.RQ_ALARM_CODE >> 8));
      buffer.Add((byte)(this.RQ_ALARM_CODE & 0xFF));
      buffer.Add(this.RN_ALARM_LEVEL);
      buffer.Add((byte)this.RQ_ALARM_STATE);
      buffer.Add((byte)(this.RN_PAR_1 >> 24));
      buffer.Add((byte)(this.RN_PAR_1 >> 16));
      buffer.Add((byte)(this.RN_PAR_1 >> 8));
      buffer.Add((byte)(this.RN_PAR_1 & 0xFF));
      buffer.Add((byte)(this.RN_PAR_2 >> 24));
      buffer.Add((byte)(this.RN_PAR_2 >> 16));
      buffer.Add((byte)(this.RN_PAR_2 >> 8));
      buffer.Add((byte)(this.RN_PAR_2 & 0xFF));
      return 12;
    }

    protected override string stringify()
    {
      return $"Code: {RQ_ALARM_CODE.ToString("0:X4")} -> Level: {RN_ALARM_LEVEL.ToString("0:X2")} -> State: {RQ_ALARM_STATE.ToString()} -> Parameter 1: {RN_PAR_1.ToString("0:X8")} -> Parameter 2: {RN_PAR_2.ToString("0:X8")}";
    }
  }
}
