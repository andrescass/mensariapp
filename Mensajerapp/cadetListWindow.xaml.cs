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
    /// Lógica de interacción para cadetListWindow.xaml
    /// </summary>
    public partial class cadetListWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        /**
         * Model variables
         */
        // Cadets
        List<Cadets> cadetList;

        public cadetListWindow()
        {
            InitializeComponent();

            dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");
            cadetList = new List<Cadets>();

            cadetGrid.ItemsSource = LoadCadets(dbConnection);
        }

        public List<Cadets> LoadCadets(SQLiteConnection _conn)
        {
            List<Cadets> cadets = new List<Cadets>();

            String sqlQ = "SELECT * FROM Cadets ORDER BY name ASC";
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
    }
}
