using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Contact_Manager
{
    public static class DatenTools
    {
        public static int HoechsteID(List<Person> contacts)//Holt die höchste ID und returnt diese
        {
            int hoechsteID = 0; 
            foreach(Person p in contacts)
            {
                if(hoechsteID < p.ID)
                {
                    hoechsteID = p.ID;
                }
            }
            return hoechsteID;
        }

        public static int HoechsteMitarbeiterNr(List<Person> contacts)//Holt die höchste Mitarbeiternummer und returnt diese
        {
            int hoechsteNr = 100000;
            foreach (Person p in contacts)
            {
                if(p is Employee e)
                {
                    if (hoechsteNr < e.MitarbeiterNummer)
                    {
                        hoechsteNr = e.MitarbeiterNummer;
                    }
                }

            }
            return hoechsteNr;
        }
        public static bool DatenPruefenSpeichern(List<Person> contacts, bool create, int updateIndex, string kontakt, string status, string anrede, string titel, string name,
            string vorname, char geschlecht, string geburtsdatum, string nationalitaet, string strasse, string plz, string ort, string telMobil,
            string telPrivat, string telGeschaeft, string email, string kaderstufe, string funktion = "", string pensum = "", string ahvNummer = "", string abteilung = "",
            string angestelltSeit = "", string austrittsdatum = "", string ausbildungsjahr = "", string ausbildungsdauer = "", string kundentyp = "n",
            string firmenname = "", string firmenkontakt = "", string firmaStr = "", string firmaPLZ = "", string firmaOrt = "", List<string> notizen = null)
        {  //Funktion prüft alle Eingaben -> wenn diese korrekt sind, speichert er sie in eiener neuen oder bestehenden Instanz 

            string fehler = null;

            if (string.IsNullOrWhiteSpace(kontakt)) //Eingaben werden geprüft, bei Falscheingaben wird der Strinf fehler mit Fehlermeldungen ergänzt
            {
                fehler += "Kontakt darf nicht leer sein." + "\n";
            }

            if (string.IsNullOrWhiteSpace(anrede))
            {
                fehler += "Anrede darf nicht leer sein." + "\n";
            }

            fehler = RequiredNameCheck(name, "Name", fehler, 2, 30);
            fehler = RequiredNameCheck(vorname, "Vorname", fehler, 2, 30);
            fehler = DateCheck(geburtsdatum, "Geburtsdatum", fehler);
            fehler = NumberCheck(plz, "Postleitzahl", fehler, 0, 100000);
            fehler = NumberCheck(telMobil, "Mobile Telefonnummer", fehler);
            fehler = NumberCheck(telPrivat, "Private Telefonnummer", fehler);
            fehler = NumberCheck(telGeschaeft, "Geschäftliche Telefonnummer", fehler);
            fehler = MailCheck(email, fehler);
            fehler = NumberCheck(pensum, "Prozentzahl", fehler, -1, 100);
            fehler = DateCheck(angestelltSeit, "Datum: Angestellt seit", fehler);
            fehler = DateCheck(austrittsdatum, "Datum: Austritt", fehler);
            fehler = NumberCheck(ausbildungsjahr, "Ausbildungsjahre", fehler);
            fehler = NumberCheck(ausbildungsdauer, "Ausbildungsdauer", fehler);

            if (!Regex.IsMatch(ahvNummer, @"^756\.\d{4}\.\d{4}\.\d{2}$") && ahvNummer != "") //AHV Nummer wird kontrolliert
            {
                fehler += "AHV Nummer ungültiges Format" + "\n";
            }

            if (fehler != null) // Wenn kein Fehler besteht ist fehler = null ansonsten wird eine Fehlermeldung aufpoppen und die Funktion vor dem Erstellen/Updaten beendet.
            {
                MessageBox.Show(fehler, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            Person p;
            if (create) //Wenn create = true ->Typ wird je nach Auswahl vom Kontakt-Feld erstellt
            {                       //=false -> Person welche geupdatet wird, wird geladen
                switch (kontakt)
                {
                    case "LERNENDER":
                        p = new Trainee();
                        break;
                    case "MITARBEITER":
                        p = new Employee();
                        break;
                    case "KUNDE":
                        p = new Customer();
                        break;
                    default:
                        p = new Person();
                        break;
                }
                contacts.Add(p);
            }
            else
            {
                p = contacts[updateIndex];
            }

            p.Status = status == "Aktiv";  //Alle Felder der Person P werden mit den aktuellen Daten gespeichert
            p.Anrede = anrede;
            p.Titel = titel;
            p.Name = name;
            p.Vorname = vorname;
            p.Geschlecht = geschlecht;
            p.Geburtsdatum = geburtsdatum;
            p.Nationalitaet = nationalitaet;
            p.Ort = ort;
            p.Strasse = strasse;
            if (plz != "")
            {
                p.Plz = Convert.ToInt32(plz);
            }
            else
            {
                p.Plz = 0;
            }
            p.TelMobil = telMobil;
            p.TelPrivat = telPrivat;
            p.TelGeschaeft = telGeschaeft;
            p.EMail = email;

            //Ab Hier wird kontrolliert was für ein Typ erstellt wurde. Es werden nur entsprechende Felder gespeichert
            if (p is Trainee t) //Lernender-Felder
            {
                if (ausbildungsdauer != "")
                {
                    t.AusbildungsDauer = Convert.ToInt32(ausbildungsdauer);
                }
                if (ausbildungsjahr != "")
                {
                    t.AktuellesAusbildungsJahr = Convert.ToInt32(ausbildungsjahr); 
                }
            }

            if (p is Employee e)//Employee-Felder
            {
                if (kaderstufe != "Nicht gesetzt")
                {
                    e.Kaderstufe = Convert.ToInt16(kaderstufe);
                }
                else
                {
                    e.Kaderstufe = -1;
                }

                
                e.Funktion = funktion;
                e.AHVNummer = ahvNummer;
                e.Abteilung = abteilung;
                e.AngestelltSeit = angestelltSeit;
                e.Austrittsdatum = austrittsdatum;

                // Pensum (double statt Int16)
                if (!string.IsNullOrWhiteSpace(pensum))
                {
                    if (double.TryParse(pensum, out var pensumVal))
                        e.Pensum = pensumVal;
                    else
                        e.Pensum = -1; // „nicht gesetzt“ oder ungültig
                }
                else
                {
                    e.Pensum = -1; // wenn leer
                }
            }

            if (p is Customer c)// Firmen-Felder
            {
                // Kundentyp sicher ermitteln:
                // - "Nicht gesetzt" => 'n'
                // - Sonst wird das erste Zeichen vom Input gespeichert (z. B. "A" -> 'a')
                if (!string.IsNullOrWhiteSpace(kundentyp) &&
                    !string.Equals(kundentyp, "Nicht gesetzt", StringComparison.OrdinalIgnoreCase))
                {
                    c.KundenTyp = char.ToLowerInvariant(kundentyp.Trim()[0]); // erwartet 'a'–'e'
                }
                else
                {
                    c.KundenTyp = 'n';
                }

                
                c.FirmenName = firmenname;
                c.FirmenKontakt = firmenkontakt;
                c.FirmaStrasse = firmaStr;
                c.FirmaPLZ = firmaPLZ;   
                c.FirmaOrt = firmaOrt;

                // Notizen-Liste absichern
                c.Notizen = notizen ?? new List<string>();
            }

            // --- PATCH 4: fehlendes Return ---
            return true;
        }

        private static string MailCheck(string input, string fehler) //Kontrolliert den Input ob es ein gültige E-Mailadresse ist. Eingabe ist Freiwillig.
        {
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (Regex.IsMatch(input, pattern) || input == "")
                {
                    return fehler;
                }
                else
                {
                    return fehler += "Ungültige E-Mail\n";
                }
            }
            catch
            {
                return fehler += "Ungültige E-Mail2\n";
            }
        }

        private static string DateCheck(string input, string inputName, string fehler) //Kontrolliert den Input ob es ein Datum ist. Eingabe ist Freiwillig.
        {
            if (!DateTime.TryParseExact(input, "dd.MM.yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateTime datum) && input != "")
            {
                fehler += "Ungültiges " + inputName + "\n";
            }
            return fehler;
        }

        private static string NumberCheck(string input, string inputName, string fehler, int min = 0, long max = 1000000000000000000)
        {

            try  //Kontrolliert den Input ob es eine Zahl ist. Eingabe ist Freiwillig.
            {
                if (input != "")
                {
                    long number = Convert.ToInt64(input);
                    if (number < min || number > max || input.Contains('+'))
                    {
                        return fehler += "Ungültige " + inputName + "\n";
                    }
                }
            }
            catch (Exception)
            {
                fehler += "Ungültige " + inputName + "\n";
            }
            return fehler;
        }
        private static string RequiredNameCheck(string input, string inputName, string fehler, int minLenght, int maxLenght)
        {
            if (string.IsNullOrWhiteSpace(input)) //Kontrolliert den Namen auf Gültigkeit. Eingabe ist Pflicht.
            {
                fehler += inputName + " darf nicht leer sein." + "\n";
            }

            if (input.Length < minLenght)
            {
                fehler += inputName + " ist zu kurz." + "\n";
            }

            if (input.Length > maxLenght)
            {
                fehler += inputName + " ist zu lang." + "\n";
            }
            // Nur Buchstaben, Leerzeichen oder Bindestrich erlaubt
            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[A-Za-zÄÖÜäöüß\- ]+$"))
            {
                fehler += inputName + " enthält ungültige Zeichen." + "\n";
            }
            return fehler;
        }

        public static List<Person> PersonLoeschen(Person toDelete, List<Person> list)
        {
            list.Remove(toDelete);
            return list;
        }

        public static string BirthdayCheck(List<Person> contacts) // Erstellt eine Liste mit Personen die heute Geburtstag haben
        {
            string BirthdayList = "Happy Birthday to:\n\n";
            DateTime today = DateTime.Today;
            bool found = false;

            //Durchgeht die ganze Liste und checkt ob heute jemand Geburtstag hat
            foreach (Person p in contacts)
            {
                if (DateTime.TryParse(p.Geburtsdatum, out DateTime geburi))
                {
                    geburi = new DateTime(today.Year, geburi.Month, geburi.Day);

                    if (geburi == today)
                    {
                        BirthdayList += p.Vorname + " " + p.Name + "\n";
                        found = true;
                    }
                }
            }
            // Falls niemand Geburtstag hat
            if (!found)
            {
                BirthdayList = "Heute hat niemand Geburtstag";
            }

            return BirthdayList;
        }

        public static List<string> NextBirthdayList(List<Person> contacts) // Gibt eine Liste mit den nächsten Geburtstagen zurück.
        {
            List<(Person Person, DateTime dateNächstes)> eintraege = new List<(Person, DateTime)>();
            string format = "dd.MM.yyyy";
            DateTime dateHeute = DateTime.Today;

            foreach (Person p in contacts)
            {
                if (DateTime.TryParseExact(p.Geburtsdatum, format,        
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime datum))
                // Prüft genau ob das Geburtsdatum im gewünschten Format vorliegt
                {
                    // Nächster Geburtstag wird im aktuellen Jahr erzeugt
                    DateTime dateNächstes = new DateTime(dateHeute.Year, datum.Month, datum.Day);

                    // Wenn der Geburtstag in diesem Jahr schon vorbei ist wird er auf nächstes Jahr verschoben
                    if (dateNächstes < dateHeute)
                        dateNächstes = dateNächstes.AddYears(1);

                    // heutiger Geburstag wird ausgeschlossen
                    if (dateNächstes != dateHeute)
                        eintraege.Add((p, dateNächstes));
                }
            }

            // Geburtstage nach datum sortieren
            var sortiert = eintraege.OrderBy(e => e.dateNächstes).ToList();

            List<string> angezeigteKontakte = new List<string>();
            foreach (var e in sortiert)
            {
                //Ausgabe in die liste
                angezeigteKontakte.Add($"{e.Person.Vorname} {e.Person.Name} - {e.dateNächstes:dd.MM}");
            }

            return angezeigteKontakte;
        }






    }
}
