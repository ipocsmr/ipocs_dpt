using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace IPOCS_Programmer.ObjectTypes
{
    public class Points : BasicObject
    {
        public override byte objectTypeId { get { return 1; } }

        public byte frogOutput { get; set; }

        [Editor(typeof(PointsMotorEditor), typeof(CollectionEditor))]
        public List<PointsMotor> Motors { get; } = new List<PointsMotor>();

        public override IList<Type> SupportedOrders
        {
            get
            {
                var list = base.SupportedOrders;
                list.Add(typeof(IPOCS.Packets.Orders.ThrowPoints));
                return list;
            }
        }

        public override List<byte> Serialize()
        {
            var vector = new List<byte>();
            vector.Add(this.objectTypeId);
            int lengthPos = vector.Count;
            vector.Add(0); // Length;
            byte[] toBytes = Encoding.ASCII.GetBytes(this.Name);
            vector.AddRange(toBytes);
            vector.Add(0);
            vector.Add(this.frogOutput);

            foreach (var motor in Motors)
            {
                var motorVector = motor.Serialize();
                vector.AddRange(motorVector);
            }
            vector[lengthPos] = (byte)(vector.Count - lengthPos);
            return vector;
        }
    }

    public class PointsMotorEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.TypeEditor<CollectionControlButton>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = CollectionControlButton.ItemsSourceProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            var type = propertyItem.PropertyType;
            Editor.ItemsSourceType = type;
            // added

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ObjectTypes.PointsMotor).IsAssignableFrom(p) && !p.IsAbstract);
            Editor.NewItemTypes = types.ToList();

            base.ResolveValueBinding(propertyItem);
        }
    }
}
