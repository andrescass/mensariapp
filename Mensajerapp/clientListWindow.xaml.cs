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
    /// Lógica de interacción para clientListWindow.xaml
    /// </summary>
    public partial class clientListWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        /**
         * Model variables
         */
        // Clients
        List<Clients> clientList;

        public clientListWindow()
        {
            InitializeComponent();

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            clientList = new List<Clients>();

            clientGrid.ItemsSource = LoadClients(dbConnection);

        }

        public List<Clients> LoadClients(SQLiteConnection _conn)
        {
            List<Clients> clients = new List<Clients>();
            string sqlQ = "SELECT * FROM Clients ORDER BY name ASC";
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
    }
}
