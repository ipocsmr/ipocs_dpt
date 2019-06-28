using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Xml;
using System.Xml.Serialization;

namespace IPOCS_Programmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            this.Title = this.Title + " - " + version;

            IPOCS.Networker.Instance.OnConnect += (client) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (MainWindow.Concentrators.ToList().FirstOrDefault((c) => c.UnitID == client.UnitID) == null)
                    {
                        client.Disconnect();
                        return;
                    }
                    client.Name = MainWindow.Concentrators.FirstOrDefault((c) => c.UnitID == client.UnitID).Name;
                    this.tcpLog.AppendText("(" + client.UnitID.ToString() + ") connected from " + client.RemoteEndpoint.ToString() + Environment.NewLine);
                    this.tcpLog.ScrollToEnd();
                    Clients.Add(new ClientTab(client));
                });
            };
            IPOCS.Networker.Instance.OnConnectionRequest += (client, request) =>
            {
                var concentrator = Concentrators.FirstOrDefault((c) => c.UnitID == client.UnitID);
                if (concentrator == null)
                {
                    return false;
                }
                List<byte> vector;
                try {
                    vector = concentrator.Serialize();
                } catch (NullReferenceException) {
                    return false;
                }

                ushort providedChecksum = ushort.Parse(request.RXID_SITE_DATA_VERSION);
                ushort computedChecksum = IPOCS.CRC16.Calculate(vector.ToArray());
                this.Dispatcher.Invoke(() =>
                {
                    this.tcpLog.AppendText("(" + client.UnitID.ToString() + ") R.CRC: " + request.RXID_SITE_DATA_VERSION + ", C.CRC: " + computedChecksum.ToString() + Environment.NewLine);
                    this.tcpLog.ScrollToEnd();
                });
                if (providedChecksum == 0 || computedChecksum != providedChecksum)
                {
                    var responseMsg = new IPOCS.Protocol.Message();
                    responseMsg.RXID_OBJECT = client.UnitID.ToString();
                    responseMsg.packets.Add(new IPOCS.Protocol.Packets.ApplicationData
                    {
                        RNID_XUSER = 0x0001,
                        PAYLOAD = vector.ToArray()
                    });
                    client.Send(responseMsg);
                    return false;
                }
                return true;
            };
            IPOCS.Networker.Instance.OnDisconnect += (client) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.tcpLog.AppendText(((client.UnitID == 0) ? "Unkown" : client.UnitID.ToString()) + " client disconnected" + Environment.NewLine);
                    this.tcpLog.ScrollToEnd();
                    var ct = Clients.FirstOrDefault((c) => c.Client == client);
                    if (ct != null)
                        Clients.Remove(ct);
                });
            };
            IPOCS.Networker.Instance.OnListening += (isListening) =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (isListening)
                            this.tcpLog.AppendText("Listening for connections..." + Environment.NewLine);
                        else
                            this.tcpLog.AppendText("No longer listening for connections." + Environment.NewLine);
                        this.tcpLog.ScrollToEnd();
                    });
                }
                catch (System.Threading.Tasks.TaskCanceledException) { }
            };
        }

        public ObservableCollection<ClientTab> Clients { get; } = new ObservableCollection<ClientTab>();
        
        private void AddObjectClick(object sender, RoutedEventArgs e)
        {
            var concentrator = new ObjectTypes.Concentrator();
            concentrator.UnitID = (byte)(Concentrators.Count + 1);
            Concentrators.Add(concentrator);
        }

        private void SerializeClick(object sender, RoutedEventArgs e)
        {
            if (this.objectlist.SelectedItem != null && this.objectlist.SelectedItem is ObjectTypes.Concentrator)
            {
                var concentrator = this.objectlist.SelectedItem as ObjectTypes.Concentrator;
                var vector = concentrator.Serialize();
                var output = BitConverter.ToString(vector.ToArray()).Replace("-", " ");
                System.Windows.Clipboard.SetText(output);
                Console.WriteLine(output);
            }
        }

        private void ArduinoConnect_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as System.Windows.Controls.Primitives.ToggleButton;
            IPOCS.Networker.Instance.isListening = !IPOCS.Networker.Instance.isListening;
            item.IsChecked = IPOCS.Networker.Instance.isListening;
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IPOCS.Networker.Instance.isListening = false;
        }

        public static ObservableCollection<ObjectTypes.Concentrator> Concentrators { get; } = new ObservableCollection<ObjectTypes.Concentrator>();
        private void Editor_LoadSiteData_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = "*.xml";
            var loaded = dialog.ShowDialog();
            if (loaded.HasValue && loaded.Value)
            {
                var types = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from lType in lAssembly.GetTypes()
                             where typeof(ObjectTypes.BasicObject).IsAssignableFrom(lType)
                             select lType).ToList();
                var types2 = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                              from lType in lAssembly.GetTypes()
                              where typeof(ObjectTypes.PointsMotor).IsAssignableFrom(lType)
                              select lType).ToList();
                types.AddRange(types2);
                types.Add(typeof(ObjectTypes.BasicObject));
                XmlSerializer xsSubmit = new XmlSerializer(Concentrators.GetType(), types.ToArray());
                using (var reader = XmlReader.Create(dialog.FileName))
                {
                    var objs = xsSubmit.Deserialize(reader) as ObservableCollection<ObjectTypes.Concentrator>;
                    Concentrators.Clear();
                    foreach (var concentrator in objs)
                        Concentrators.Add(concentrator);
                }
                saveFileName = dialog.FileName;
            }
        }

        string saveFileName { get; set; }

        private void Editor_SaveSiteData_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(saveFileName))
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.AddExtension = true;
                dialog.DefaultExt = "*.xml";
                var saved = dialog.ShowDialog();
                if (saved.HasValue && saved.Value)
                {
                    this.saveFileName = dialog.FileName;
                }
            }

            var types = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from lType in lAssembly.GetTypes()
                         where typeof(ObjectTypes.BasicObject).IsAssignableFrom(lType)
                         select lType).ToList();
            var types2 = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                          from lType in lAssembly.GetTypes()
                          where typeof(ObjectTypes.PointsMotor).IsAssignableFrom(lType)
                          select lType).ToList();
            types.AddRange(types2);
            types.Add(typeof(ObjectTypes.BasicObject));
            XmlSerializer xsSubmit = new XmlSerializer(Concentrators.GetType(), types.ToArray());
            using (XmlWriter writer = XmlWriter.Create(this.saveFileName))
            {
                xsSubmit.Serialize(writer, Concentrators);
            }
        }
    }
}
