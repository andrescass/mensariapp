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
    /// Lógica de interacción para newClientWindow.xaml
    /// </summary>
    public partial class newClientWindow : Window
    {
        // Database file name
        private String DATABASE_FILE_NAME = "." + Path.DirectorySeparatorChar + "mainDB.db";
        // Database connection
        SQLiteConnection dbConnection;

        public event EventHandler<EventArgs> AcceptEvent;

        public newClientWindow()
        {
            InitializeComponent();
        }

        private void newClientCanc_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void newClientAcc_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(newClientName.Text) ||
                String.IsNullOrEmpty(newClientContact.Text) ||
                String.IsNullOrEmpty(newClientAddress.Text) ||
                String.IsNullOrEmpty(newClientMail.Text) ||
                String.IsNullOrEmpty(newClientCuit.Text) ||
                String.IsNullOrEmpty(newClientTel.Text))
            {
                MessageBox.Show("Por favor complete todos los campos", "Campos incompletos");
                return;
            }
            else
            {
                dbConnection = new SQLiteConnection("Data Source=" + DATABASE_FILE_NAME + ";Version=3");

                String sqlQ = "INSERT INTO Clients (name, contact, address, email, cuit, tel) VALUES " +
                    "('" + newClientName.Text + "', '" + newClientContact.Text + "', '" + newClientAddress.Text + "', " +
                    "'" + newClientMail.Text + "', '" + newClientCuit.Text + "', '" + newClientTel.Text + "');";

                dbConnection.Open();
                SQLiteCommand scmd = new SQLiteCommand(sqlQ, dbConnection);
                scmd.ExecuteNonQuery();
                dbConnection.Close();

                this.AcceptEvent(newClientAcc, new EventArgs());

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
