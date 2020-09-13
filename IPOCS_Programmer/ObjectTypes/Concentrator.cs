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
    public class Concentrator
    {
        public string Name { get; set; }

        [Editor(typeof(ConcentratorEditor), typeof(CollectionEditor))]
        public List<BasicObject> Objects { get; set; } = new List<BasicObject>();

        public List<byte> Serialize()
        {
            var vector = new List<byte>();

            foreach (var basicObject in this.Objects)
            {
                var objectVector = basicObject.Serialize();
                vector.AddRange(objectVector);
            }

            return vector;
        }
    }

    public class ConcentratorEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.TypeEditor<CollectionControlButton>
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
                .Where(p => typeof(ObjectTypes.BasicObject).IsAssignableFrom(p) && !p.IsAbstract);
            Editor.NewItemTypes = types.ToList();

            base.ResolveValueBinding(propertyItem);
        }
    }
}
