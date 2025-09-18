using Contact_Manager;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Contact_Manager
{
    public class SucheTools
    {
        public static List<Person> SearchByField(string searchText, string suchFeld, string type, List<Person> contacts) 
        //Methode um die Liste nach bestimmten Kriterien zu filtern
        {
            List<Person> result = new List<Person>();

            if (contacts == null) return result; // Falls keine Kontakte übergebn wurden geht eine leere Liste zurück um ein crash zu vermeiden

            string text = searchText.ToLower();

            foreach (Person p in contacts)
            {

                if (type == "Alle Kontakte")
                {
                    if (SearchByWhat(p, suchFeld, text))
                        result.Add(p);
                }
                else if (type == "Kunden" && p is Customer)
                {
                    if (SearchByWhat(p, suchFeld, text))
                        result.Add(p);
                }
                else if (type == "Mitarbeiter" && p is Employee && !(p is Trainee))
                {
                    if (SearchByWhat(p, suchFeld, text))
                        result.Add(p);
                }
                else if (type == "Lernende" && p is Trainee)
                {
                    if (SearchByWhat(p, suchFeld, text))
                        result.Add(p);
                }
            }
            return result;
        }


        private static bool SearchByWhat(Person p, string suchFeld, string text)
        //Methode um die bereits gefilteterte Liste nochmals nach neuen kriterien zu filtern, gibt ein bool zurück der zum abfragen
        // Gibt true zurück, wenn die Person das Suchkriterium erfüllt
        {
            switch (suchFeld)
            {
                case "vorname":
                    return p.Vorname != null && p.Vorname.ToLower().Contains(text);

                case "name":
                    return p.Name != null && p.Name.ToLower().Contains(text);

                case "geburtsdatum":
                    return p.Geburtsdatum != null && p.Geburtsdatum.ToLower().Contains(text);

                default:
                    return false;
            }
        }
    }
}
