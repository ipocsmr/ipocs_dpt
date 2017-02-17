﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public enum RQ_LEVEL_CROSSING_STATE
  {
    OPEN = 1,
    PREPARED = 2,
    ACTIVATED = 3,
    CLOSED = 4,
    OPENING = 5,
    OUT_OF_CONTROL = 6
  };
    
  public class LevelCrossing : Packet
  {
    public static new byte RNID_PACKET { get { return 19; } }
    public RQ_LEVEL_CROSSING_STATE RQ_LEVEL_CROSSING_STATE;
    public RQ_RELEASE_STATE RQ_RELEASE_STATE;
    public short RT_OPERATION;
    
    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_LEVEL_CROSSING_STATE = (RQ_LEVEL_CROSSING_STATE)buffer[0];
      this.RQ_RELEASE_STATE = (RQ_RELEASE_STATE)buffer[1];
      this.RT_OPERATION = (short)((buffer[2] << 8) + buffer[3]);
      return 5;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_LEVEL_CROSSING_STATE);
      buffer.Add((byte)this.RQ_RELEASE_STATE);
      buffer.Add((byte)(this.RT_OPERATION >> 8));
      buffer.Add((byte)(this.RT_OPERATION & 0xFF));
      return 5;
    }

    protected override string stringify()
    {
      return $"{RQ_LEVEL_CROSSING_STATE.ToString()} -> {RQ_RELEASE_STATE.ToString()} -> {RT_OPERATION}";
    }

  }
}