using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensajerapp
{
    class Models
    {
    }

    public class Clients
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Cuit { get; set; }
        public string   Tel { get; set; }

    }

    public class Cadets
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String Email { get; set; }
        public string Cuit { get; set; }
        public string   Tel { get; set; }
    }

    public class Trips
    {
        public int ID { get; set; }
        public int ClientID { get; set; }
        public String ClientName { get; set; }
        public String ClientContact { get; set; }
        public string Tel { get; set; }
        public int Price { get; set; }
        public String InitAddress { get; set; }
        public String EndAddress { get; set; }
        public string ContactName { get; set; }
        public int Latening { get; set; }
        public DateTime InitHour { get; set; }
        public String InitHourStr { get; set; }
        public String StringHour { get; set; }
        public string StringDate { get; set; }
        public bool IsArqued { get; set; }
        public bool IsFinished { get; set; }
        public String State { get; set; }
        public int CadetID { get; set; }
        public String CadetName { get; set; }
        public string HasBox { get; set; }
        public bool IsBilled { get; set; }

    }
}
