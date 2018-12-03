using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para cadetPay.xaml
    /// </summary>
    public partial class cadetPay : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        /**
         * Model variables
         */
        // Clients
        private List<Cadets> cadetList;
        List<int> tripIdList;

        DateTime? fromTime;
        DateTime? toTime;
        int cadetPerc;
        bool toConfirm;

        public cadetPay(int cadetId)
        {
            InitializeComponent();

            fromTime = DateTime.Today;
            fromTime = fromTime.Value.AddDays(-7);
            toTime = DateTime.Today;
            toTime = toTime.Value.AddHours(23);
            toTime = toTime.Value.AddMinutes(59);

            cadetPayFrom.SelectedDate = fromTime;
            cadetPayTo.SelectedDate = toTime;
            cadetPerc = 10;

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            cadetList = new List<Cadets>();

            cadetList = LoadCadets(dbConnection, cadetId);
            toConfirm = false;
            cadetPayConfirm.IsEnabled = false;
        }

        public List<Cadets> LoadCadets(SQLiteConnection _conn, int cadetID)
        {
            List<Cadets> cadets = new List<Cadets>();
            int loopi = 0;

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
                cadetPayList.Items.Add(sqlReader.GetString(1));
                if(cadetID == sqlReader.GetInt16(0))
                {
                    cadetPayList.SelectedIndex = loopi;
                }
                loopi++;
            }
            _conn.Close();

            return cadets;
        }

        private void cadetPayCalBut_Click(object sender, RoutedEventArgs e)
        {
            tripIdList = new List<int>();
            long total = 0;
            float cadetTotal = 0;

            fromTime = cadetPayFrom.SelectedDate;
            fromTime = fromTime.Value.AddMinutes(fromTime.Value.Minute * (-1));
            fromTime = fromTime.Value.AddHours(fromTime.Value.Hour * (-1));
            toTime = cadetPayTo.SelectedDate;
            toTime = toTime.Value.AddMinutes(59 - toTime.Value.Minute);
            toTime = toTime.Value.AddHours(23 - toTime.Value.Hour);

            if ((cadetPayList.SelectedIndex > -1) && (fromTime.Value.Ticks <= toTime.Value.Ticks) && (!String.IsNullOrEmpty(cadetPayPerc.Text)) && (!String.IsNullOrWhiteSpace(cadetPayPerc.Text)))
            {
                String sqlQ = "SELECT * FROM trips WHERE ";
                sqlQ += "cadetID = " + cadetList[cadetPayList.SelectedIndex].ID;
                sqlQ += " AND initHourInt BETWEEN " + fromTime.Value.Ticks + " AND " + toTime.Value.Ticks;
                sqlQ += " AND isArqued = 0";

                dbConnection.Open();
                SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                SQLiteDataReader sqlReader = scmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    tripIdList.Add(sqlReader.GetInt16(0));
                    total += sqlReader.GetInt16(2);
                }
                dbConnection.Close();

                cadetPerc = Convert.ToInt16(cadetPayPerc.Text);
                cadetTotal = total * cadetPerc / 100;

                cadetPayTotal.Content = "Total = " + total;
                cadetPayCadetTotal.Content = cadetTotal;

                toConfirm = true;
                cadetPayConfirm.IsEnabled = true;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cadetPayConfirm_Click(object sender, RoutedEventArgs e)
        {
            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            dbConnection.Open();

            foreach (int cid in tripIdList)
            {
                string sqlQ = "UPDATE trips SET isArqued = 1 WHERE ID = " + cid;
                SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                scmd.ExecuteNonQuery();
            }

            dbConnection.Close();

            toConfirm = false;
            cadetPayConfirm.IsEnabled = false;
        }
    }
}
