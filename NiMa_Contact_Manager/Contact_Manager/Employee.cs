using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact_Manager
{
    internal class Employee : Person // Klasse für Mitarbeiter
    {
        public static int NaechsteMitarbeiterNummer { get; set; }
        public int MitarbeiterNummer { get; set; }
        public int Kaderstufe { get; set; }
        public string Funktion { get; set; }
        public string Abteilung { get; set; }
        public double Pensum { get; set; }
        public string AHVNummer { get; set; }
        public string AngestelltSeit { get; set; }
        public string Austrittsdatum { get; set; }

        public Employee() : base()
        {
            MitarbeiterNummer = NaechsteMitarbeiterNummer++; // Mitarbeiternr wird immer um eins erhöht
        }
    }
}
