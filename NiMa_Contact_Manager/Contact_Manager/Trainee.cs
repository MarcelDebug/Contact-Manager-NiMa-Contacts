using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact_Manager
{
    internal class Trainee : Employee // Klasse für Lernende
    {
        public int AusbildungsDauer { get; set; }
        public int AktuellesAusbildungsJahr { get; set; }

        public Trainee() : base()
        {

        }
    }
}
