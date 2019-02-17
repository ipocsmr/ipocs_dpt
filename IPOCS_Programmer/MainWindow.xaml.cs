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

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            this.PortMenu.ItemsSource = ports;

            IPOCS.Networker.Instance.OnConnect += (client) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (MainWindow.Concentrators.FirstOrDefault((c) => c.UnitID == client.UnitID) == null)
                    {
                        client.Disconnect();
                        return;
                    }
                    this.tcpLog.AppendText("Client connected" + Environment.NewLine);
                    this.tcpLog.ScrollToEnd();
                    Clients.Add(new ClientTab(client));
                });
            };
            IPOCS.Networker.Instance.OnConnectionRequest += (client, request) =>
            {
                var concentrator = Concentrators.FirstOrDefault((c) => c.UnitID == client.UnitID);
                var vector = concentrator.Serialize();

                ushort providedChecksum = ushort.Parse(request.RXID_SITE_DATA_VERSION);
                ushort computedChecksum = IPOCS.CRC16.Calculate(vector.ToArray());
                this.Dispatcher.Invoke(() =>
                {
                    this.tcpLog.AppendText("Recieved CRC: " + request.RXID_SITE_DATA_VERSION + ", Calculated CRC: " + computedChecksum.ToString() + Environment.NewLine);
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

        private MenuItem selectedMenuItem = null;
        private void PortItemSelected(object sender, RoutedEventArgs e)
        {
            if (this.PortMenu == sender)
                return;
            if (this.selectedMenuItem != null && sender != this.selectedMenuItem)
                this.selectedMenuItem.IsChecked = false;

            var menuItem = (sender as MenuItem);
            menuItem.IsChecked = !menuItem.IsChecked;
            this.selectedMenuItem = menuItem;
        }

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
            if (this.selectedMenuItem == null)
            {
                item.IsChecked = false;
                return;
            }
            if (item.IsChecked.HasValue && item.IsChecked.Value)
            {
                this.port = new System.IO.Ports.SerialPort(this.selectedMenuItem.DataContext as String, 9600);
                new System.Threading.Thread(new System.Threading.ThreadStart(this.threadRunner)).Start();
            }
            else
            {
                this.port.Close();
                this.port = null;
            }
        }

        System.IO.Ports.SerialPort port;
        private void threadRunner()
        {
            if (this.port != null && !this.port.IsOpen)
                this.port.Open();
            try
            {
                while (this.port != null && this.port.IsOpen)
                {
                    int line = this.port.ReadChar();

                    this.Dispatcher.Invoke(() =>
                    {
                        this.incoming.AppendText("" + (char)line);
                        this.incoming.ScrollToEnd();
                    });
                }
            }
            catch
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.incoming.AppendText(Environment.NewLine + "*** Connection closed" + Environment.NewLine);
                    this.incoming.ScrollToEnd();
                });
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                sendIt_Click(sender, null);
            }
        }

        private void sendIt_Click(object sender, RoutedEventArgs e)
        {
            this.port.WriteLine(this.toSend.Text);
            this.toSend.Text = string.Empty;
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IPOCS.Networker.Instance.isListening = false;
            if (this.port != null && this.port.IsOpen)
            {
                this.port.Close();
                this.port = null;
            }
        }

        public static ObservableCollection<ObjectTypes.Concentrator> Concentrators { get; private set; } = new ObservableCollection<ObjectTypes.Concentrator>();
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
                restartServer();
            }
        }

        string saveFileName { get; set; }

        private void restartServer()
        {
            var bgw = new System.ComponentModel.BackgroundWorker();
            bgw.DoWork += (sender, e) =>
            {
                if (IPOCS.Networker.Instance.isListening)
                    IPOCS.Networker.Instance.isListening = false;
                while (IPOCS.Networker.Instance.isListening)
                {
                    System.Threading.Thread.Sleep(100);
                }
                IPOCS.Networker.Instance.isListening = true;
            };
            bgw.RunWorkerAsync();
        }

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
                    restartServer();
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
