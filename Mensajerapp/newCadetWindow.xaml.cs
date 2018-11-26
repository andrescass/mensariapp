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
    /// Lógica de interacción para newCadetWindow.xaml
    /// </summary>
    public partial class newCadetWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        public event EventHandler<EventArgs> AcceptEvent;

        public newCadetWindow()
        {
            InitializeComponent();
        }

        private void newCadetCanc_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void newCadetAcc_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(newCadetName.Text) ||
                String.IsNullOrEmpty(newCadetAddress.Text) ||
                String.IsNullOrEmpty(newCadetMail.Text) ||
                String.IsNullOrEmpty(newCadetCuit.Text) ||
                String.IsNullOrEmpty(newCadetTel.Text))
            {
                MessageBox.Show("Por favor complete todos los campos", "Campos incompletos");
                return;
            }
            else
            {
                dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");

                String sqlQ = "INSERT INTO Cadets (name, address, email, cuit, tel) VALUES " +
                    "('" + newCadetName.Text + "', '" + newCadetAddress.Text + "', " +
                    "'" + newCadetMail.Text + "', '" + newCadetCuit.Text + "', '" + newCadetTel.Text + "');";

                dbConnection.Open();
                SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                scmd.ExecuteNonQuery();
                dbConnection.Close();

                this.AcceptEvent(newCadetAcc, new EventArgs());

                this.Close();
            }


        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
