using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using Path = System.IO.Path;

namespace Mensajerapp
{
    /// <summary>
    /// Lógica de interacción para tripListWindow.xaml
    /// </summary>
    public partial class tripListWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        /**
         * Model variables
         */
        // Clients
        List<Trips> tripList;
        private List<Clients> clientList;
        private List<Cadets> cadetList;
        private List<String> clientNames;
        private List<String> cadetNames;

        /**
         * Filter variables
         */
        DateTime? fromTime;
        DateTime? toTime;
        bool? finalized;
        bool? factured;
        int clientIdx;
        int cadetIdx;

        public tripListWindow()
        {
            InitializeComponent();

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            tripList = new List<Trips>();

            tripGrid.ItemsSource = LoadTrips(dbConnection, true, "");

            clientList = new List<Clients>();
            cadetList = new List<Cadets>();
            clientNames = new List<String>();
            cadetNames = new List<String>();

            FillCombos(dbConnection);
            finalized = null;
            factured = null;
        }

        public List<Trips> LoadTrips(SQLiteConnection _conn, bool isInit, string _sqlQ)
        {
            List<Trips> trips = new List<Trips>();
            String sqlQ;

            if (isInit)
            {
                sqlQ = "SELECT * FROM Trips ORDER BY initHourInt DESC";
            }
            else
            {
                sqlQ = _sqlQ;
            }

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
                    InitHourStr = dateHourAux, // dateHourHour + ":" + dateMin,dateHourHour + ":" + dateMin,
                    StringDate = dateDayDay + "/" + dateMonth + "/" + dateYear,
                    IsArqued = (sqlReader.GetInt16(8) == 1) ? true : false,
                    IsFinished = (sqlReader.GetInt16(9) == 1) ? true : false,
                    CadetID = sqlReader.GetInt16(10),
                    CadetName = sqlReader2.GetString(1),
                    HasBox = (sqlReader.GetInt16(11) == 1) ? "Si" : "No"
                });

            }
            _conn.Close();

            if (isInit)
            {
                tripFiltTo.SelectedDate = (DateTime?)trips[0].InitHour;
                tripFiltFrom.SelectedDate = (DateTime?)trips.Last().InitHour;
            }

            return trips;
        }

        void FillCombos(SQLiteConnection _conn)
        {
            String sqlQ = "SELECT * FROM Clients";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            _conn.Open();
            SQLiteCommand scmd = new SQLiteCommand(sqlQ, _conn);
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
                tripFiltClient.Items.Add(sqlReader.GetString(1));
            }
            dbConnection.Close();

            sqlQ = "SELECT * FROM Cadets";
            //SQLiteDataReader sqlReader = ExecuteDBRead(sqlQ);

            dbConnection.Open();
            scmd = new SQLiteCommand(sqlQ, _conn);
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
                tripFiltCadet.Items.Add(sqlReader.GetString(1));
            }
            dbConnection.Close();
        }

        private void tripFiltFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fromTime = tripFiltFrom.SelectedDate;
            fromTime = fromTime.Value.AddMinutes(fromTime.Value.Minute*(-1));
            fromTime = fromTime.Value.AddHours(fromTime.Value.Hour*(-1));
        }

        private void tripFiltTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            toTime = tripFiltTo.SelectedDate;
            toTime = toTime.Value.AddMinutes(59 - toTime.Value.Minute);
            toTime = toTime.Value.AddHours(23 - toTime.Value.Hour);
        }

        private void tripFiltFinish_Click(object sender, RoutedEventArgs e)
        {
            finalized = tripFiltFinish.IsChecked;
        }

        private void tripFiltBilled_Click(object sender, RoutedEventArgs e)
        {
            factured = tripFiltFinish.IsChecked;
        }

        private void tripFiltClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientIdx = tripFiltClient.SelectedIndex;
        }

        private void tripFiltCadet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cadetIdx = tripFiltCadet.SelectedIndex;
        }

        private void tripFiltBut_Click(object sender, RoutedEventArgs e)
        {
            string sqlQ = "SELECT * FROM Trips WHERE ";
            bool changes = false;

            if(fromTime.Value.Ticks <= toTime.Value.Ticks)
            {
                sqlQ += "initHourInt BETWEEN " + fromTime.Value.Ticks + " AND " + toTime.Value.Ticks;
                changes = true;
            }

            if(tripFiltFinish.IsChecked == true)
            {
                if(changes)
                {
                    sqlQ += " AND isFinished = 1 ";
                }
                else
                {
                    sqlQ += " isFinished = 1 ";
                    changes = true;
                }
            }
            else
            {
                if(finalized != null)
                {
                    //CheckBox check = (CheckBox)sender;
                    //if (check.Content.ToString() == "Finalizado")
                    {
                        if (changes)
                        {
                            sqlQ += " AND isFinished = 0 ";
                        }
                        else
                        {
                            sqlQ += " isFinished = 0 ";
                            changes = true;
                        }
                    }
                }
            }

            if (tripFiltBilled.IsChecked == true)
            {
                if (changes)
                {
                    sqlQ += " AND isBilled = 1 ";
                }
                else
                {
                    sqlQ += " isBilled = 1 ";
                    changes = true;
                }
            }
            else
            {
                if (factured != null)
                {
                    //CheckBox check = (CheckBox)sender;
                    //if (check.Content.ToString() == "Facturado")
                    {
                        if (changes)
                        {
                            sqlQ += " AND isBilled = 0 ";
                        }
                        else
                        {
                            sqlQ += " isBilled = 0 ";
                            changes = true;
                        }
                    }
                }
            }

            if(tripFiltCadet.SelectedIndex > -1)
            {
                if(changes)
                {
                    sqlQ += " AND cadetID = " + cadetList[tripFiltCadet.SelectedIndex].ID;
                }
                else
                {
                    sqlQ += " cadetID = " + cadetList[tripFiltCadet.SelectedIndex].ID;
                    changes = true;
                }
            }

            if (tripFiltClient.SelectedIndex > -1)
            {
                if (changes)
                {
                    sqlQ += " AND clientID = " + clientList[tripFiltClient.SelectedIndex].ID;
                }
                else
                {
                    sqlQ += " clientID = " + clientList[tripFiltClient.SelectedIndex].ID;
                    changes = true;
                }
            }

            if (changes)
            {
                sqlQ += " ORDER BY initHourInt DESC";
            }
            else
            {
                sqlQ = "SELECT * FROM Trips ORDER BY initHourInt DESC";
            }

            tripGrid.ItemsSource = LoadTrips(dbConnection, false, sqlQ);
        }

        private void tripFiltClearBut_Click(object sender, RoutedEventArgs e)
        {
            tripFiltClient.SelectedIndex = -1;
            tripFiltCadet.SelectedIndex = -1;
            tripFiltBilled.IsChecked = false;
            tripFiltFinish.IsChecked = false;

            tripGrid.ItemsSource = LoadTrips(dbConnection, true, "");
        }
    }
}
