using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IPOCS_Programmer
{
    /// <summary>
    /// Interaction logic for ClientTab.xaml
    /// </summary>
    public partial class ClientTab : UserControl
    {
        public enum EConnectionState
        {
            WAITING_FOR_REQUEST,
            CONNECTED
        }
        public static readonly DependencyProperty IdentityProperty =
        DependencyProperty.Register("Identity", typeof(string), typeof(ClientTab),
        new PropertyMetadata(string.Empty));

        public List<ObjectTypes.BasicObject> Objects { get { return this.Client.unit.Objects; } }

        public ClientTab(Client client)
        {
            this.Client = client;
            this.Identity = "Unknown client";
            InitializeComponent();
            BindingOperations.SetBinding(this, IdentityProperty, new Binding
            {
                FallbackValue = "Unknown Client",
                Source = this.Client.UnitID
            });
            client.OnMessage += (msg) =>
            {
                this.Dispatcher.Invoke(() =>
          {
                  foreach (var packet in msg.packets)
                  {
                      this.tcpLog.AppendText($"-<- {msg.RXID_OBJECT} : {packet.GetType().Name} : {packet.ToString()}" + Environment.NewLine);
                  }
                  this.tcpLog.ScrollToEnd();
              });
            };
        }

        public string Identity { get; private set; }

        public Client Client { get; private set; }

        private void TextBoxNet_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                sendItNet_Click(sender, null);
            }
        }

        private void sendItNet_Click(object sender, RoutedEventArgs e)
        {
            if (this.PacketToSend.SelectedObject != null)
            {
                var bo = this.comboBox.SelectedItem as ObjectTypes.BasicObject;

                var msg = new IPOCS.Message();
                msg.RXID_OBJECT = bo.Name;
                var packet = this.PacketToSend.SelectedObject as IPOCS.Packet;
                msg.packets.Add(packet);
                this.tcpLog.AppendText($"->- {msg.RXID_OBJECT} : {packet.GetType().Name} : {packet.ToString()}" + Environment.NewLine);
                Client.Send(msg);
            }
        }

        private void comboBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            var packetType = e.AddedItems[0] as Type;
            PacketToSend.SelectedObject = Activator.CreateInstance(packetType);
        }
    }
}
