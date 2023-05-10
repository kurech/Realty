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
using System.Windows.Shapes;

namespace realty
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        public readonly realtygilyazovEntities db = new realtygilyazovEntities();
        public Client _client = new Client();
        public Client _deletionclient = new Client();
        public ClientsWindow()
        {
            InitializeComponent();
            ClientsToCmb();
            ClientsToDeletionCmb();
        }

        public void ClientsToCmb()
        {
            List<string> cmbtext = new List<string>();
            foreach (var a in db.Client)
            {
                cmbtext.Add(a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + ".");
            }
            clientCmb.ItemsSource = cmbtext;
        }

        public void ClientsToDeletionCmb()
        {
            List<string> cmbtext = new List<string>();
            foreach (var a in db.Client)
            {
                cmbtext.Add(a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + ".");
            }
            clientCmbForDelete.ItemsSource = cmbtext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void searchClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchClients.Text;

            var _clients = db.Client.ToList();

            IEnumerable<Client> results = FindClients(searchText, _clients);

            clientsListView.ItemsSource = results;
        }

        public int LevenshteinDistance(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1))
            {
                if (string.IsNullOrEmpty(s2))
                    return 0;
                else
                    return s2.Length;
            }

            if (string.IsNullOrEmpty(s2))
                return s1.Length;

            if (s1.Length > s2.Length)
            {
                var tmp = s1;
                s1 = s2;
                s2 = tmp;
            }

            int m = s2.Length;
            int n = s1.Length;
            int[,] d = new int[2, m + 1];
            int curRow = 0;
            for (int j = 0; j <= m; j++)
                d[curRow, j] = j;

            for (int i = 1; i <= n; ++i)
            {
                int prevRow = curRow;
                curRow = 1 - curRow;

                d[curRow, 0] = i;

                for (int j = 1; j <= m; ++j)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;

                    d[curRow, j] = Math.Min(
                        Math.Min(d[prevRow, j] + 1, d[curRow, j - 1] + 1),
                        d[prevRow, j - 1] + cost);
                }
            }

            return d[curRow, m];
        }

        public IEnumerable<Client> FindClients(string searchText, IEnumerable<Client> clients)
        {
            var results = clients.Where(client => LevenshteinDistance(searchText, client.Surname) <= 3)
                         .Select(client => new Client { Surname = client.Surname, Name = client.Name, Patronymic = client.Patronymic, Phone = client.Phone, Email = client.Email });

            return results;
        }

        private void btnCreateClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clientPhone.Text == string.Empty || clientEmail.Text == string.Empty)
                {
                    var newClient = new Client()
                    {
                        Surname = clientSurname.Text,
                        Name = clientName.Text,
                        Patronymic = clientPatronimyc.Text,
                        Phone = clientPhone.Text,
                        Email = clientEmail.Text
                    };

                    db.Client.Add(newClient);
                    db.SaveChanges();

                    MessageBox.Show("Успешно добавлен клиент!");
                }
                else
                {
                    MessageBox.Show("Добавьте контактную информацию");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            _client.Surname = clientUpdateSurname.Text;
            _client.Name = clientUpdateName.Text;
            _client.Patronymic = clientUpdatePatronimyc.Text;
            _client.Phone = clientUpdatePhone.Text;
            _client.Email = clientUpdateEmail.Text;

            db.SaveChanges();

            clientUpdateSurname.Clear();
            clientUpdateName.Clear();
            clientUpdatePatronimyc.Clear();
            clientUpdatePhone.Clear();
            clientUpdateEmail.Clear();

            clientCmb.ItemsSource = null;
            ClientsToCmb();

            MessageBox.Show($"{_client.Surname} - успешно обновлен");
        }

        private void clientCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var a in db.Client)
            {
                if (clientCmb.SelectedItem != null)
                {
                    if (a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + "." == clientCmb.SelectedItem.ToString())
                    {
                        clientUpdateSurname.Text = a.Surname;
                        clientUpdateName.Text = a.Name;
                        clientUpdatePatronimyc.Text = a.Patronymic;
                        clientUpdatePhone.Text = a.Phone;
                        clientUpdateEmail.Text = a.Email;
                        _client = a;
                    }
                }
            }  
        }

        private void clientCmbForDelete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var a in db.Client)
            {
                if (clientCmbForDelete.SelectedItem != null)
                {
                    if (a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + "." == clientCmbForDelete.SelectedItem.ToString())
                    {
                        _deletionclient = a;
                    }
                }
                    
            }
        }

        private void btnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            db.Client.Remove(_deletionclient);
            db.SaveChanges();

            ClientsToCmb();
            ClientsToDeletionCmb();
            MessageBox.Show($"{_deletionclient.Surname} - успешно удален");
        }
    }
}
