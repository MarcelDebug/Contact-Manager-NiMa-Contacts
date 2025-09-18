using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact_Manager
{
    public class Person // Klasse für Person - alle Kontakte erben von dieser Klasse
    {
        public static int NaechsteIdNummer { get; set; }
        public int ID { get; set; }
        public bool Status { get; set; } // Aktiv / Inaktiv
        public string Anrede { get; set; }
        public string Titel { get; set; }
        public string Name { get; set; }
        public string Vorname { get; set; }
        public char Geschlecht { get; set; } // m: Männlich w: Weiblich d: Divers n: Nicht gesetzt
        public string Geburtsdatum { get; set; }
        public string EMail { get; set; }
        public string Nationalitaet { get; set; }
        public string TelMobil { get; set; }
        public string TelPrivat { get; set; }
        public string TelGeschaeft { get; set; }

        public string Strasse { get; set; }
        public int Plz { get; set; }
        public string Ort { get; set; }


        public Person()
        {
            ID = NaechsteIdNummer++; // ID wird immer um 1 erhöht
        }
    }
}
