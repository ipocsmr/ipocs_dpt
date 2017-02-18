using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.ObjectTypes
{
    public abstract class BasicObject
    {
        public string Name { get; set; }

        public abstract byte objectTypeId { get; }

        public virtual IList<Type> SupportedOrders
        {
            get
            {
                var list = new List<Type>();
                list.Add(typeof(IPOCS.Packets.Orders.RequestStatus));
                return list;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public abstract List<byte> Serialize();
    }
}
