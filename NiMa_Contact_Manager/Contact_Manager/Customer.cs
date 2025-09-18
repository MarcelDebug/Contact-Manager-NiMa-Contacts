using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact_Manager
{
    internal class Customer : Person // Klasse für Kunden
    {
        public char KundenTyp { get; set; } // a-e Kundentyp n = ungesetzt
        public string FirmenName { get; set; }
        public string FirmenKontakt { get; set; }
        public string FirmaStrasse { get; set; }
        public string FirmaPLZ { get; set; }
        public string FirmaOrt{ get; set; }
        public List<string> Notizen { get; set; }

        public Customer() : base()
        {

        }
    }
}
