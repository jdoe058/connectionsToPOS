using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace connectionsToPOS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string PATH = $"{Environment.CurrentDirectory}\\connectionsList.json";
        private readonly Connections _connections;

        public MainWindow()
        {
            InitializeComponent();
            _connections = (Connections)Resources["connections"];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
   
            try
            {
                if (!File.Exists(PATH))
                {
                    File.CreateText(PATH).Dispose();
                } else using (var reader = File.OpenText(PATH))
                {
                    Connections conn = JsonSerializer.Deserialize<Connections>(reader.ReadToEnd());
                    foreach (var c in conn)
                    {
                        _connections.Add(c);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter writer = File.CreateText(PATH))
                {
                    string output = JsonSerializer.Serialize(_connections, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                    writer.Write(output);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            DataGridRow row = sender as DataGridRow;
            ConnectionsModel model = row.DataContext as ConnectionsModel;
            System.Diagnostics.Process.Start($"{Environment.CurrentDirectory}\\AnyDesk.bat", $"{model.AnyDeskId} {model.Password}" );
        }
    }
    
    public class ConnectionsModel : IEditableObject
    {
        public string Company { get; set; }

        public string Name { get; set; }

        public string AnyDeskId { get; set; }

        public string Password { get; set; }

        private ConnectionsModel _connectionsModel = null;
        private bool _editing = false;

        public void BeginEdit()
        {
            if (!_editing) 
            {
                _connectionsModel = this.MemberwiseClone() as ConnectionsModel;
                _editing = true;
            }
        }

        public void CancelEdit()
        {
            if (_editing)
            {
                Company = _connectionsModel.Company;
                Name = _connectionsModel.Name;
                AnyDeskId = _connectionsModel.AnyDeskId;
                Password = _connectionsModel.Password;
                _editing = false;
            }
        }

        public void EndEdit()
        {
            if (_editing)
            {
                _connectionsModel = null;
                _editing = false;
            }
        }
    }

    public class Connections : ObservableCollection<ConnectionsModel> { }
}
