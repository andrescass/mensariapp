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
using System.Data.SQLite;
using Path = System.IO.Path;
using System.Text.RegularExpressions;

namespace Mensajerapp
{
    /// <summary>
    /// Lógica de interacción para newTripWindow.xaml
    /// </summary>
    public partial class newTripWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        public event EventHandler<EventArgs> AcceptEvent;

        private List<Clients> clientList;
        private List<Cadets> cadetList;

        private List<String> clientNames;
        private List<String> cadetNames;

        private DateTime currentDateTime = DateTime.Now;
        public DateTime CurrentDateTime
        {
            get
            {
                return DateTime.Now;
            }
            set
            {
                currentDateTime = value;
            }
        }

        public newTripWindow()
        {
            InitializeComponent();

            clientList = new List<Clients>();
            cadetList = new List<Cadets>();
            clientNames = new List<String>();
            cadetNames = new List<String>();

            CurrentDateTime = DateTime.Now;



            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");

            String sqlQ = "SELECT * FROM Clients";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            dbConnection.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
            SQLiteDataReader sqlReader = scmd.ExecuteReader();

            while (sqlReader.Read())
            {
                clientList.Add(new Clients()
                {
                    ID = sqlReader.GetInt16(0),
                    Name = sqlReader.GetString(1),
                    Contact = sqlReader.GetString(2),
                    Address = sqlReader.GetString(3),
                    Email = sqlReader.GetString(4),
                    Cuit = sqlReader.GetString(5),
                    Tel = sqlReader.GetString(6)
                });
                clientNames.Add(sqlReader.GetString(1));
                newTripClient.Items.Add(sqlReader.GetString(1));
            }
            dbConnection.Close();

            sqlQ = "SELECT * FROM Cadets";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            dbConnection.Open();
            scmd = new SQLiteCommand(sqlQ, dbConnection);
            sqlReader = scmd.ExecuteReader();

            while (sqlReader.Read())
            {
                cadetList.Add(new Cadets()
                {
                    ID = sqlReader.GetInt16(0),
                    Name = sqlReader.GetString(1),
                    Address = sqlReader.GetString(2),
                    Email = sqlReader.GetString(3),
                    Cuit = sqlReader.GetString(4),
                    Tel = sqlReader.GetString(5)
                });
                cadetNames.Add(sqlReader.GetString(1));
                newTripCadet.Items.Add(sqlReader.GetString(1));
            }
            dbConnection.Close();

            newTripHour.Value = CurrentDateTime;
        }

        private void newTripCanc_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void newTripAcc_Click(object sender, RoutedEventArgs e)
        {
            if ((newTripClient.SelectedIndex > -1) && (newTripCadet.SelectedIndex > 0) && !String.IsNullOrEmpty(newTripAddress.Text) && 
                !String.IsNullOrEmpty(newTripAddressEnd.Text) && !String.IsNullOrEmpty(newTripContact.Text) && !String.IsNullOrEmpty(newTripPrice.Text))
            {
                int tripHasBox = (newTripBox.IsChecked == true) ? 1 : 0;
                DateTime? tripHour = newTripHour.Value;

                dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");

                String sqlQ = "INSERT INTO Trips (clientID, price, initAddress, finalAddress, contactName, latening, initHour, isArqued, isFinished," +
                    " cadetID, hasBox, initHourInt, isBilled) VALUES " +
                    "('" + clientList[newTripClient.SelectedIndex].ID + "', '" + newTripPrice.Text + "', '" + newTripAddress.Text +
                    "', '" + newTripAddressEnd.Text + "', '" + newTripContact.Text + "', '" + "0" + "', '" + newTripHour.Value + "', '" + "0" + "', '" + "0" + "', '"
                    + cadetList[newTripCadet.SelectedIndex].ID + "', '" + tripHasBox + "', '" + tripHour.Value.Ticks + "', '" + 0 + "');";

                dbConnection.Open();
                SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                scmd.ExecuteNonQuery();
                dbConnection.Close();

                this.AcceptEvent(newTripAcc, new EventArgs());
            }

            this.Close();
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

        private void newTripClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            newTripAddress.Text = clientList[newTripClient.SelectedIndex].Address;
            newTripContact.Text = clientList[newTripClient.SelectedIndex].Contact;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}


