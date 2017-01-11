using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.ObjectTypes
{
  public class PointT1: BasicObject
  {
    public override byte objectTypeId { get { return 4; } }

    public byte servoPin { get; set; }

    public byte positionPin { get; set; }

    public override IList<Type> SupportedOrders
    {
      get
      {
        var list = base.SupportedOrders;
        list.Add(typeof(IPOCS.Packets.Orders.ThrowPoints));
        return list;
      }
    }
  }
}
