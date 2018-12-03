using System;
using System.Collections.Generic;
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
using System.Data.SQLite;
using Path = System.IO.Path;
using System.Windows.Controls.Ribbon;
using System.Globalization;

namespace Mensajerapp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /**
         * Database variables
         */
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        /**
         * Model variables
         */
        // Clients
        List<Clients> clientList;
        // Cadets
        List<Cadets> cadetList;
        // Trips
        List<Trips> tripList;

        /**
         * Child windows
         */
        newClientWindow addClientWd;
        newCadetWindow addCadetWd;
        newTripWindow addTripWd;
        clientListWindow clientListWindow;
        cadetListWindow cadetListWindow;
        tripListWindow tripListWindow;
        cadetPay cadetPayWindow;

        public MainWindow()
        {
            InitializeComponent();

            // Initialice DB
            if(!File.Exists(path: DATABASE_FILE_NAME))
            {
                SQLiteConnection.CreateFile(DATABASE_FILE_NAME);
                dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            }

            string sqlQ = @"CREATE TABLE IF NOT EXISTS Cadets (
                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name VARCHAR(1024) NULL,
                                    address VARCHAR(256) NULL,
                                    email VARCHAR(64) NULL,
                                    cuit VARCHAR(32) NULL,
                                    tel VARCHAR(32) NULL)";

            string sqlQa = @"CREATE TABLE IF NOT EXISTS Clients (
                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name VARCHAR(1024) NULL,
                                    contact VARCHAR(1024) NULL,
                                    address VARCHAR(256) NULL,
                                    email VARCHAR(64) NULL,
                                    cuit VARCHAR(64) NULL,
                                    tel VARCHAR(32) NULL)";

            string sqlQt = @"CREATE TABLE IF NOT EXISTS Trips (
                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    clientID INTEGER NULL,
                                    price INTEGER NULL,
                                    initAddress VARCHAR(256) NULL,
                                    finalAddress VARCHAR(256) NULL,
                                    contactName VARCHAR(256) NULL,
                                    latening INTEGER NULL,
                                    initHour INTEGER NULL,
                                    isArqued INTEGER NULL,
                                    isFinished INTEGER NULL,
                                    cadetID INTEGER NULL,
                                    hasBox INTEGER NULL,
                                    initHourInt INTEGER NULL,
                                    isBilled INTEGER NULL)";

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");

            ExecuteQuery(sqlQ);
            ExecuteQuery(sqlQa);
            ExecuteQuery(sqlQt);

            clientList = new List<Clients>();
            cadetList = new List<Cadets>();
            tripList = new List<Trips>();

            //clientGrid.ItemsSource = LoadClients(dbConnection);
            //cadetGrid.ItemsSource = LoadCadets(dbConnection);
            tripGrid.ItemsSource = LoadTrips(dbConnection);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //dbConnection.Close();
            if(addClientWd != null)
            {
                addClientWd.Close();
            }
            if (addCadetWd != null)
            {
                addCadetWd.Close();
            }
            if (addTripWd != null)
            {
                addTripWd.Close();
            }
            if (clientListWindow != null)
            {
                clientListWindow.Close();
            }
            if (cadetListWindow != null)
            {
                cadetListWindow.Close();
            }
            if (tripListWindow != null)
            {
                tripListWindow.Close();
            }
            if (cadetPayWindow != null)
            {
                cadetPayWindow.Close();
            }
        }

        private void ExecuteQuery(string sqlQuery)
        {
            dbConnection.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQuery, dbConnection);
            scmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        private SQLiteDataReader ExecuteDBRead(string sqlQuery)
        {
            SQLiteDataReader sqlReader;
            dbConnection.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQuery, dbConnection);
            sqlReader = scmd.ExecuteReader();
            dbConnection.Close();

            return sqlReader;
        }

        private void MenuItemNuevo_Click(object sender, RoutedEventArgs e)
        {
            switch(mainTab.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    addClientWd = new newClientWindow();
                    addClientWd.AcceptEvent += AddClientWd_AcceptEvent;
                    addClientWd.Show();
                    break;
                case 2:
                    addCadetWd = new newCadetWindow();
                    addCadetWd.AcceptEvent += AddCadetWd_AcceptEvent;
                    addCadetWd.Show();
                    break;
                default:
                    break;
            }
        }

        private void clientNew_Click(object sender, RoutedEventArgs e)
        {
            switch (mainTab.SelectedIndex)
            {
                case 0:
                    addTripWd = new newTripWindow();
                    addTripWd.AcceptEvent += AddTripWd_AcceptEvent;
                    addTripWd.Show();
                    break;
                case 1:
                    addClientWd = new newClientWindow();
                    addClientWd.AcceptEvent += AddClientWd_AcceptEvent;
                    addClientWd.Show();
                    break;
                case 2:
                    addCadetWd = new newCadetWindow();
                    addCadetWd.AcceptEvent += AddCadetWd_AcceptEvent;
                    addCadetWd.Show();
                    break;
                default:
                    break;
            }
        }

        private void AddTripWd_AcceptEvent(object sender, EventArgs e)
        {
            tripGrid.ItemsSource = null;
            tripGrid.ItemsSource = LoadTrips(dbConnection);
        }

        private void AddCadetWd_AcceptEvent(object sender, EventArgs e)
        {
            //cadetGrid.ItemsSource = null;
            //cadetGrid.ItemsSource = LoadCadets(dbConnection);
        }

        private void AddClientWd_AcceptEvent(object sender, EventArgs e)
        {
            //clientGrid.ItemsSource = null;
            //clientGrid.ItemsSource = LoadClients(dbConnection);
        }

        private void clientDel_Click(object sender, RoutedEventArgs e)
        {
            switch (mainTab.SelectedIndex)
            {
                case 0:
                    Trips selTrip = (Trips)tripGrid.SelectedItem;
                    String sqlt = "DELETE FROM Trips WHERE ID = " + selTrip.ID + ";";
                    dbConnection.Open();
                    SQLiteCommand scmdt = new SQLiteCommand(sqlt, dbConnection);
                    scmdt.ExecuteNonQuery();
                    dbConnection.Close();

                    tripGrid.ItemsSource = null;
                    tripGrid.ItemsSource = LoadTrips(dbConnection);
                    break;
                case 1:
                    /*Clients selClient = (Clients)clientGrid.SelectedItem;
                    //String sqlQ = "DELETE FROM Clients WHERE ID = " + clientList[clientGrid.SelectedIndex].ID + ";";
                    String sqlQ = "DELETE FROM Clients WHERE ID = " + selClient.ID + ";";
                    dbConnection.Open();
                    SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                    scmd.ExecuteNonQuery();
                    dbConnection.Close();

                    clientGrid.ItemsSource = null;
                    clientGrid.ItemsSource = LoadClients(dbConnection);

                    if (clientGrid.Items.Count == 0)
                    {
                        clientDel.IsEnabled = false;
                    }*/
                    break;
                case 2:
                   /* Cadets selCadet = (Cadets)cadetGrid.SelectedItem;
                    String sqlQc = "DELETE FROM Cadets WHERE ID = " + selCadet.ID + ";";
                    dbConnection.Open();
                    SQLiteCommand scmdc = new SQLiteCommand(sqlQc, dbConnection);
                    scmdc.ExecuteNonQuery();
                    dbConnection.Close();

                    cadetGrid.ItemsSource = null;
                    cadetGrid.ItemsSource = LoadCadets(dbConnection);

                    if (cadetGrid.Items.Count == 0)
                    {
                        clientDel.IsEnabled = false;
                    }*/
                    break;
            }
        }

        private void clientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientDel.IsEnabled = true;
        }

        private void cadetGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientDel.IsEnabled = true;
        }

        private void tripGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientDel.IsEnabled = true;
            tripFinish.IsEnabled = true;
        }

        public List<Clients> LoadClients(SQLiteConnection _conn)
        {
            List<Clients> clients = new List<Clients>();
            String sqlQ = "SELECT * FROM Clients";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            _conn.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, _conn);
            SQLiteDataReader sqlReader = scmd.ExecuteReader();

            while (sqlReader.Read())
            {
                String lala = sqlReader.GetString(1);
                clients.Add(new Clients()
                {
                    ID = sqlReader.GetInt16(0),
                    Name = sqlReader.GetString(1),
                    Contact = sqlReader.GetString(2),
                    Address = sqlReader.GetString(3),
                    Email = sqlReader.GetString(4),
                    Cuit = sqlReader.GetString(5),
                    Tel = sqlReader.GetString(6)
                });
            }
            _conn.Close();
            return clients;
        }

        public List<Cadets> LoadCadets(SQLiteConnection _conn)
        {
            List<Cadets> cadets = new List<Cadets>();

            String sqlQ = "SELECT * FROM Cadets";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            _conn.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, _conn);
            SQLiteDataReader sqlReader = scmd.ExecuteReader();

            while (sqlReader.Read())
            {
                cadets.Add(new Cadets()
                {
                    ID = sqlReader.GetInt16(0),
                    Name = sqlReader.GetString(1),
                    Address = sqlReader.GetString(2),
                    Email = sqlReader.GetString(3),
                    Cuit = sqlReader.GetString(4),
                    Tel = sqlReader.GetString(5)
                });
            }
            _conn.Close();

            return cadets;
        }

        public List<Trips> LoadTrips(SQLiteConnection _conn)
        {
            List<Trips> trips = new List<Trips>();

            String sqlQ = "SELECT * FROM Trips";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            _conn.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, _conn);
            SQLiteDataReader sqlReader = scmd.ExecuteReader();

            while (sqlReader.Read())
            {
                sqlQ = "SELECT * FROM Clients WHERE ID = " + sqlReader.GetInt16(1);
                scmd = new SQLiteCommand(sqlQ, _conn);
                SQLiteDataReader sqlReader1 = scmd.ExecuteReader();
                sqlReader1.Read();

                sqlQ = "SELECT * FROM Cadets WHERE ID = " + sqlReader.GetInt16(10);
                scmd = new SQLiteCommand(sqlQ, _conn);
                SQLiteDataReader sqlReader2 = scmd.ExecuteReader();
                sqlReader2.Read();

                string dateStr = sqlReader.GetString(7);
                String dateDay = dateStr.Split(' ')[0];
                String dateHour = dateStr.Split(' ')[1];

                int dateYear = int.Parse(dateDay.Split('/')[2]);
                int dateMonth = int.Parse(dateDay.Split('/')[1]);
                int dateDayDay = int.Parse(dateDay.Split('/')[0]);
                int dateHourHour = int.Parse(dateHour.Split(':')[0]);
                int dateMin = int.Parse(dateHour.Split(':')[1]);
                int dateSec = int.Parse(dateHour.Split(':')[2]);
                string dateHourAux = dateHourHour + ":";
                if (dateMin > 0)
                {
                    dateHourAux += dateMin;
                }
                else
                {
                    dateHourAux += "00";
                }

                trips.Add(new Trips()
                {
                    ID = sqlReader.GetInt16(0),
                    ClientID = sqlReader.GetInt16(1),
                    ClientName = sqlReader1.GetString(1),
                    ClientContact = sqlReader1.GetString(2),
                    Tel = sqlReader1.GetString(6),
                    Price = sqlReader.GetInt16(2),
                    InitAddress = sqlReader.GetString(3),
                    EndAddress = sqlReader.GetString(4),
                    ContactName = sqlReader.GetString(5),
                    Latening = sqlReader.GetInt16(6),
                    InitHour = new DateTime(dateYear, dateMonth, dateDayDay, dateHourHour, dateMin, dateSec),
                    InitHourStr = dateHourAux, // dateHourHour + ":" + dateMin,
                    StringDate = dateDayDay + "/" + dateMonth + "/" + dateYear,
                    IsArqued = (sqlReader.GetInt16(8) == 1) ? true : false,
                    IsFinished = (sqlReader.GetInt16(9) == 1) ? true : false,
                    CadetID = sqlReader.GetInt16(10),
                    CadetName = sqlReader2.GetString(1),
                    HasBox = (sqlReader.GetInt16(11) == 1) ? "Si" : "No",
                    IsBilled = (sqlReader.GetInt16(13) == 1) ? true : false
                });

            }
            _conn.Close();

            return trips;
        }

        [ValueConversion(typeof(bool), typeof(String))]
        public class finishedToStateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if(value == null)
                {
                    return "";
                }
                bool isFinished = (bool)value;
                if(isFinished)
                {
                    return "Finalizado";
                }
                else
                {
                    return "En curso";
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void fileCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ribbNewTrip_Click(object sender, RoutedEventArgs e)
        {
            addTripWd = new newTripWindow();
            addTripWd.AcceptEvent += AddTripWd_AcceptEvent;
            addTripWd.Show();
        }

        private void ribbNewClient_Click(object sender, RoutedEventArgs e)
        {
            addClientWd = new newClientWindow();
            addClientWd.AcceptEvent += AddClientWd_AcceptEvent;
            addClientWd.Show();
        }

        private void ribbNewCaddet_Click(object sender, RoutedEventArgs e)
        {
            addCadetWd = new newCadetWindow();
            addCadetWd.AcceptEvent += AddCadetWd_AcceptEvent;
            addCadetWd.Show();
        }

        private void ribbHistClients_Click(object sender, RoutedEventArgs e)
        {
            if(!Application.Current.Windows.OfType<clientListWindow>().Any())
            {
                clientListWindow = new clientListWindow();

                clientListWindow.Show();
            }
        }

        private void ribbHistCadets_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<cadetListWindow>().Any())
            {
                cadetListWindow = new cadetListWindow();

                cadetListWindow.Show();
            }
        }

        private void ribbHistTrips_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<tripListWindow>().Any())
            {
                tripListWindow = new tripListWindow();

                tripListWindow.Show();
            }
        }

        private void ribbArqCadet_Click(object sender, RoutedEventArgs e)
        {
            cadetPayWindow = new cadetPay(-1);
            cadetPayWindow.Show();
        }

        private void tripFinish_Click(object sender, RoutedEventArgs e)
        {
            Trips selTrip = (Trips)tripGrid.SelectedItem;

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            dbConnection.Open();

            string sqlQ = "UPDATE trips SET isFinished = 1 WHERE ID = " + selTrip.ID;
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
            scmd.ExecuteNonQuery();

            dbConnection.Close();
            tripGrid.ItemsSource = null;
            tripGrid.ItemsSource = LoadTrips(dbConnection);
        }
    }
}
