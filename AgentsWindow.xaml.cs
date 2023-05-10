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
    /// Логика взаимодействия для AgentsWindow.xaml
    /// </summary>
    public partial class AgentsWindow : Window
    {
        public readonly realtygilyazovEntities db = new realtygilyazovEntities();
        public Agent _updateagent = new Agent();
        public Agent _deletionagent = new Agent();
        public AgentsWindow()
        {
            InitializeComponent();
            AgentsToUpdateCmb();
            AgentsToDeletionCmb();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void searchAgents_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchAgents.Text;

            var _agents = db.Agent.ToList();

            IEnumerable<Agent> results = FindClients(searchText, _agents);

            agentsListView.ItemsSource = results;
        }

        public IEnumerable<Agent> FindClients(string searchText, IEnumerable<Agent> agents)
        {
            var results = agents.Where(agent => LevenshteinDistance(searchText, agent.Surname) <= 3)
                         .Select(agent => new Agent { Surname = agent.Surname, Name = agent.Name, Patronymic = agent.Patronymic, DealShare = agent.DealShare });

            return results;
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

        private void btnCreateAgent_Click(object sender, RoutedEventArgs e)
        {
            if (agentSurname.Text != string.Empty && agentName.Text != string.Empty && agentPatronimyc.Text != string.Empty)
            {
                if (agentDealShare.Text != string.Empty)
                {
                    var newAgent = new Agent()
                    {
                        Surname = agentSurname.Text,
                        Name = agentName.Text,
                        Patronymic = agentPatronimyc.Text,
                        DealShare = int.Parse(agentDealShare.Text)
                    };

                    db.Agent.Add(newAgent);
                    db.SaveChanges();
                }
                else
                {
                    var newAgent = new Agent()
                    {
                        Surname = agentSurname.Text,
                        Name = agentName.Text,
                        Patronymic = agentPatronimyc.Text,
                        DealShare = null
                    };

                    db.Agent.Add(newAgent);
                    db.SaveChanges();
                }
                
                MessageBox.Show("Успешно добавлен риэлтор!");
            }
            else
            {
                MessageBox.Show("ФИО вводить обязательно");
            }
        }

        private void btnUpdateAgent_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(agentUpdateDealShare.Text) >= 0 && int.Parse(agentUpdateDealShare.Text) <= 100)
            {
                _updateagent.Surname = agentUpdateSurname.Text;
                _updateagent.Name = agentUpdateName.Text;
                _updateagent.Patronymic = agentUpdatePatronimyc.Text;
                _updateagent.DealShare = int.Parse(agentUpdateDealShare.Text);

                db.SaveChanges();

                agentUpdateSurname.Clear();
                agentUpdateName.Clear();
                agentUpdatePatronimyc.Clear();
                agentUpdateDealShare.Clear();

                agentCmbForUpdate.ItemsSource = null;
                AgentsToUpdateCmb();
                MessageBox.Show($"{_updateagent.Surname} - успешно обновлен");
            }
            else
            {
                MessageBox.Show("Невалидня доля от комиссии");
            }
            
        }

        private void btnDeleteAgent_Click(object sender, RoutedEventArgs e)
        {
            db.Agent.Remove(_deletionagent);
            db.SaveChanges();

            AgentsToUpdateCmb();
            AgentsToDeletionCmb();
            MessageBox.Show($"{_deletionagent.Surname} - успешно удален");
        }

        private void agentCmbForDelete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var a in db.Agent)
            {
                if (agentCmbForDelete.SelectedItem != null)
                {
                    if (a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + "." == agentCmbForDelete.SelectedItem.ToString())
                    {
                        _deletionagent = a;
                    }
                }
            }
        }

        private void agentCmbForUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var a in db.Agent)
            {
                if (agentCmbForUpdate.SelectedItem != null)
                {
                    if (a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + "." == agentCmbForUpdate.SelectedItem.ToString())
                    {
                        agentUpdateSurname.Text = a.Surname;
                        agentUpdateName.Text = a.Name;
                        agentUpdatePatronimyc.Text = a.Patronymic;
                        agentUpdateDealShare.Text = Convert.ToString(a.DealShare);
                        _updateagent = a;
                    }
                }
            }
        }

        public void AgentsToUpdateCmb()
        {
            List<string> cmbtext = new List<string>();
            foreach (var a in db.Agent)
            {
                cmbtext.Add(a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + ".");
            }
            agentCmbForUpdate.ItemsSource = cmbtext;
        }

        public void AgentsToDeletionCmb()
        {
            List<string> cmbtext = new List<string>();
            foreach (var a in db.Agent)
            {
                cmbtext.Add(a.Surname + " " + a.Name.Substring(0, 1) + ". " + a.Patronymic.Substring(0, 1) + ".");
            }
            agentCmbForDelete.ItemsSource = cmbtext;
        }
    }
}
