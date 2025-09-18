using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Contact_Manager
{
    public partial class Form1 : Form
    {
        int aktuelleSeite=1;
        string showType;
        const string showAll = "all"; // Konstante Werte für Kontaktmutationen
        const string customerOnly = "customer";
        const string employeeOnly = "employee";
        const bool create = true; 
        const bool update = false;
        bool sucheAktiv = false;


        Color BtnSelectetColor = Color.FromArgb(40, 100, 40); //Button-Farbe ausgewählt 
        Color BtnColor = Color.FromArgb(0,0,64);              //Button-Farbe standard
        Point pointTopLeft = new Point(100, 50);  //Start-Punkt für die Haupt-Panels
        JsonTools jsonTool = new JsonTools();
        List<Person> contacts;
        List<Person> suchErgebnisse = new List<Person>();

        Person loadedPerson;

        private SucheTools suche;

        public Form1()
        {
            InitializeComponent();

            this.MaximumSize = new Size(1000,800);// Der Bildschirm bleibt immer in der Grösse 1000;800 
            this.MinimumSize = new Size(1000, 800); 

            pnlDel.Location = new Point(0, 610);  // Locations der Panels werden gesetzt
            pnlCustomerFields.Location = new Point(0, 360);
            pnlEmployeeFields.Location = new Point(0, 360); 
            pnlTraineeFields.Location = new Point(0, 550);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Login wird aktiviert
           using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    this.Close();
                }
            }

            //Optische anpassungen
            PictureBox symbol = new PictureBox(); //Cooles Contact-Symbol oben links 
            symbol.Image = Image.FromFile(@"./SymbolCm.png");
            symbol.Location = new Point(33,8);
            symbol.Size = new Size(34, 34);
            symbol.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(symbol);
            symbol.BringToFront();

            btnBackground.SendToBack();

            pnlCreateEdit.Location = pointTopLeft;
            pnlKontakte.Location = pointTopLeft;
            pnlHome.Location = pointTopLeft;
            pnlSearch.Location = new Point(400, 50);

            contacts = jsonTool.Load(); 

            Employee.NaechsteMitarbeiterNummer = DatenTools.HoechsteMitarbeiterNr(contacts)+1; //HöchsteMitarbeiternummer wird geladen +1 für die nächste Person
            Person.NaechsteIdNummer = DatenTools.HoechsteID(contacts)+1;
            ShowHomescreen();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ShowHomescreen();
        }
        private void ShowHomescreen() //Der Homescreen mit den Geburtstags-Anzeigen + Anzahl Kontakte werden angezeigt
        {
            ClearScreen();
            ResetMenuBtnColor();
            btnHome.BackColor = BtnSelectetColor;
            string Birthdays = DatenTools.BirthdayCheck(contacts);
            lblHeuteGeburtstag.Text = Birthdays;

            listViewBirthday.Items.Clear();
            foreach (string line in DatenTools.NextBirthdayList(contacts))
            {
                listViewBirthday.Items.Add(line);
            }
            

            int anzahlKontakte = contacts.Count;

            lblKontakZähler.Text = "Aktuell sind " + anzahlKontakte + " Mitarbeiter und Kunden in der Datenbank hinterlegt.";

            pnlHome.Visible = true;
        }



        private void ResetMenuBtnColor() // Setzt alle Menubuttons wieder auf die standard-Farbe
        {
            foreach (Control c in this.Controls)
            {
                if (Convert.ToString(c.Tag) == "menubtn")
                {
                    c.BackColor = BtnColor;
                }
            }

        }


        private void ClearScreen() // Der Nutzbereich im Screen wird gecleart
        {
            
            foreach (Control c in this.Controls)
            { 
                if(c is Panel) //Wenn es Panels mit Child-Panels hat, werden diese auch wieder Visible = false gesetzt
                {
                    foreach(Control ct in c.Controls)
                    {

                        if (ct is Panel)
                        {
                            ct.Visible = false;
                        }
                    }
                }

                if (c.Location.X >= 100 && c.Location.Y >= 50)
                {
                    c.Visible = false;
                }
            }
            radM.Checked = false;
            radW.Checked = false;
            radD.Checked = false;

            btnErstellen.Visible = false;
            btnNextPage.Enabled = false;
            btnLastPage.Enabled = false;

            pnlDel.Visible = false;
        }


        private void btnKunden_Click(object sender, EventArgs e) // Bei allen 3 folgenden Buttons wird der ShowType gesetzt und dann die ContactButtonClicked Funktion aufgerufen
        {
            showType = customerOnly;
            ContactButtonClicked(btnKunden);
            lblAllContacts.Text = "Alle Kunden:";
        }

        private void btnMitarbeiter_Click(object sender, EventArgs e)
        {
            showType = employeeOnly;
            ContactButtonClicked(btnMitarbeiter);
            lblAllContacts.Text = "Alle Mitarbeiter:";
        }

        private void btnAlleKontakte_Click(object sender, EventArgs e)
        {
            showType = showAll;
            ContactButtonClicked(btnAlleKontakte);
            lblAllContacts.Text = "Alle Kontakte:";
        }

        private void ContactButtonClicked(Button b) // Ändert die Farbe des mitgegebenen Buttons grün und lässt die erste Seite mit Kontakten des Type showTypes anzeigen
        {
            sucheAktiv = false; 
            ClearScreen();
            ResetMenuBtnColor();
            ShowContacts(1, showType);
            b.BackColor = BtnSelectetColor;
        }



        private void ShowContacts(int seite, string type) // Lässt die Kontakte des gewünschten Typens anzeigen. Seitenanzahl wird auch angegeben
        {
            aktuelleSeite = seite;
            pnlKontakte.Visible = true;

            int anzKontakte=0;
            List<Person> contactsAnzeigen = new List<Person>();
            if (sucheAktiv) // Wenn gerade gesucht wird, ist sucheAktiv true. So wird nur in der Liste suchErgebnisse gesucht 
            {
                foreach (Person p in suchErgebnisse)   
                {
                    if (type == showAll || type == employeeOnly && p is Employee || type == customerOnly && p is Customer) //type-Werte sind: "all","customer","employee","trainee"
                    {
                        anzKontakte++;
                        contactsAnzeigen.Add(p); //Anzeigeliste wird erstellt
                    }
                }
            }
            else // Ansonsten werden alle Kontakte angezeigt (Nach Seitenzahl)
            {
                foreach (Person p in contacts)
                {
                    if (type == showAll || type == employeeOnly && p is Employee || type == customerOnly && p is Customer) //type-Werte sind: "all","customer","employee","trainee"
                    {
                        anzKontakte++;
                        contactsAnzeigen.Add(p); //Anzeigeliste wird erstellt
                    }
                }
            }

            int anzProSeite = 15; // Hier wird die Maximale Seitenanzahl ausgerechnet und der Start Index gesetzt (z.B bei Seite 2 ist der start Index 15 -> erste Person auf der 2. Seite)
            int startIndex = (seite - 1) * anzProSeite; 
            int anzVolleSeiten = anzKontakte / anzProSeite;
            int anzSeiten = anzVolleSeiten;
            if (anzVolleSeiten == 0 || Convert.ToDecimal(anzKontakte) / anzVolleSeiten > anzProSeite) //Wenn es keine Volle Seite hat ODER mehr Personen in der Liste als anzahl Personen pro Seite (z.B anzProSeite = 15 anzKontakte = 16 --> anzVolleSeite=1   --> 16/1 > 15 --> true
            {
                anzSeiten++;
            }

            lblSeitenZahl.Text = seite + "/" + anzSeiten;
            if(anzSeiten > seite) //Hier werden die Seite-Weiter/zurück Pfeile enabled/disabled
            {
                btnNextPage.Enabled = true;
            }
            else
            {
                btnNextPage.Enabled = false;
            }
            if (seite > 1)
            {
                btnLastPage.Enabled = true;
            }
            else
            {
                btnLastPage.Enabled = false;
            }

            Font titelFont = new Font("Microsoft Sans Serif", 11,FontStyle.Bold); //Fonts/Farben werden gesetzt für den Header und Body der Tabelle
            Color titelColor = Color.FromArgb(90, 90, 90);
            Font showFont = new Font("Microsoft Sans Serif", 11);
            int x = 200;
            int y = 120;
            AnzeigeTools.ShowTitles(this, x, y, titelFont, titelColor); //Header anzeigen

            int stopAtIndex = anzProSeite; // Stop-Index bestimmen für die nächste forschleife. Wenn eine Seite nicht voll ist, wird frühzeitig abgebrochen. z.B. letzt seite hat nur noch 3 Personen zum anzeigen.
            if(anzVolleSeiten+1 == seite)
            {
                stopAtIndex = anzKontakte - anzVolleSeiten * anzProSeite;
            }
            for (int c=startIndex; c<startIndex+stopAtIndex; c++) //Alle Anzeige-Personen anzeigen vom startindex an bis zum stopIndex
            {
                y += 30;
                AnzeigeTools.ShowContact(this, contactsAnzeigen[c], x, y, showFont);
            }

        }

        public void editContact_Click(object sender, EventArgs e) //Sucht person mit ID, welche im button-Tag gespeichert ist -> öffnet diese dann zum bearbeiten
        {
            Button btn = sender as Button;
            int idSender = Convert.ToInt16(btn.Tag);

            foreach (Person p in contacts)
            {
                if (p.ID == idSender)
                {
                    LoadPerson(p);
                    break;
                }
            }

        }

        private void LoadPerson(Person person) // Person wird geladen und alle Felder angezeigt
        {
            EmptyPanel(pnlCreateEdit);
            ClearScreen();
            SpecificFieldsVisible();
            cmbxKontakt.Enabled = false;
            btnLoeschen.Visible = true;
            pnlCreateEdit.Visible = true;
            btnBearbeiten.Visible = true;
            btnSpeichern.Visible = true;
            pnlDel.Visible = false;
            PanelEnableDisableTag(pnlCreateEdit, "edit", false);
            btnSpeichern.Enabled = false;


            if (!person.Status) //Standard ist immer Aktiv - wenn inaktiv gesetzt ist, wird Inaktiv angezeigt
            {
                cmbxStatus.SelectedItem = cmbxStatus.Items[1]; 
            }

            cmbxAnrede.Text = person.Anrede;
            txtbTitel.Text = person.Titel;
            txtbName.Text = person.Name;
            txtbVorname.Text = person.Vorname;
            txtbGeburtsdatum.Text = person.Geburtsdatum;
            txtbNationalitaet.Text = person.Nationalitaet;

            switch (person.Geschlecht) //Radio Button Geschlecht wird gecheckt 
            {
                case 'm':
                    radM.Checked = true;
                    break;

                case 'w':
                    radW.Checked = true;
                    break;

                case 'd':
                    radD.Checked = true;
                    break;
            }

            txtbStr.Text = person.Strasse;
            txtbOrt.Text = person.Ort;
            if(person.Plz != 0) 
            {
                txtbPLZ.Text = Convert.ToString(person.Plz);
            }
            txtbTelM.Text = person.TelMobil;
            txtbTelP.Text = person.TelPrivat;
            txtbTelG.Text = person.TelGeschaeft;
            txtBEMail.Text = person.EMail;


            if (person is Customer c) // Spezialfelder für Kunde
            {
                cmbxKontakt.SelectedItem = cmbxKontakt.Items[0];
                cmbxKundentyp.Text = c.KundenTyp.ToString();
                txtbFirmenname.Text = c.FirmenName;
                txtbFStrasse.Text = c.FirmaStrasse;
                txtbFPLZ.Text = c.FirmaPLZ;
                txtbFOrt.Text = c.FirmaOrt; 
                txtbFirmenKontakt.Text = c.FirmenKontakt;
                if (c.Notizen != null)
                {
                    foreach (string s in c.Notizen)
                    {
                        lstbxNotizen.Items.Add(s);
                    }
                }
            }
            if (person is Employee e)// Spezialfelder für Mitarbeiter
            {
                cmbxKontakt.SelectedItem = cmbxKontakt.Items[1];
                txtbMitarbeiterNr.Text = Convert.ToString(e.MitarbeiterNummer);
                if(e.Kaderstufe == -1)
                {
                    cmbxKader.SelectedItem = cmbxKader.Items[0];
                }
                else
                {
                    cmbxKader.Text = Convert.ToString(e.Kaderstufe);
                }
                txtbFunktion.Text = e.Funktion;
                txtbAbteilung.Text = e.Abteilung;
                if(e.Pensum != -1)
                {
                    txtbPensum.Text = Convert.ToString(e.Pensum);
                }
                txtbAHV.Text = e.AHVNummer;
                txtbAngestelltSeit.Text = e.AngestelltSeit;
                txtbAustrittsdatum.Text = e.Austrittsdatum;
            }
            if (person is Trainee t) // Spezialfelder für Lernende
            {
                cmbxKontakt.SelectedItem = cmbxKontakt.Items[2];
                if(t.AktuellesAusbildungsJahr != 0)
                {
                    txtbAusbildungsjahr.Text = Convert.ToString(t.AktuellesAusbildungsJahr);
                }
                if(t.AusbildungsDauer != 0)
                {
                    txtbAusbildungsdauer.Text = Convert.ToString(t.AusbildungsDauer);
                }
            }

            loadedPerson = person;

        }

        private void SpecificFieldsVisible()
            //Klassenspezifische Felder (Panels) werden eingeblendet.
        { 
            pnlCustomerFields.Visible = false;
            pnlTraineeFields.Visible = false;
            pnlEmployeeFields.Visible = false;

            if (cmbxKontakt.SelectedItem == cmbxKontakt.Items[0]) //Wenn in der Combobox Kunde ausgewählt
            {
                pnlCustomerFields.Visible = true;
                EmptyPanel(pnlEmployeeFields);
                EmptyPanel(pnlTraineeFields);
            }
            if (cmbxKontakt.SelectedItem == cmbxKontakt.Items[1]) //Wenn in der Combobox Mitarbeiter ausgewählt
            {
                pnlEmployeeFields.Visible = true;
                EmptyPanel(pnlCustomerFields);
                EmptyPanel(pnlTraineeFields);
            }
            if (cmbxKontakt.SelectedItem == cmbxKontakt.Items[2]) // //Wenn in der Combobox Lernender ausgewählt
            {
                pnlTraineeFields.Visible = true;
                pnlEmployeeFields.Visible = true;
                EmptyPanel(pnlCustomerFields);
            }
        }

        private void PanelEnableDisableTag(Panel panel, string tag, bool b)  
            // Alle Controlls vom gegebenen Panel die den mitgegebenen String enthalten Enable Wert setzen. Child-Panels werden auch berücksichtigt.
        {
            foreach (Control c in panel.Controls)
            {
                if (Convert.ToString(c.Tag).Contains(tag))
                {
                    if (c is Panel) 
                    {
                        PanelEnableDisableTag(c as Panel, tag, b); //Child-Panel mit den gleichen Parameter aufrufen.
                    }
                    else
                    {
                        c.Enabled = b;
                    }
                }
            }
        }

        private void btnBearbeiten_Click(object sender, EventArgs e)  //Bearbeiten Btn --> Felder können bearbeitet werden
        {
            btnSpeichern.Enabled = true;
            PanelEnableDisableTag(pnlCreateEdit, "edit", true); 
        }
        private void btnSpeichern_Click(object sender, EventArgs e) // Speicher Btn --> Daten werden geprüft, wenn diese i.O. sind wird gespeichert. Ansonsten return-Wert: false
        {
            if (DatenGeprueftGespeichert(update))
            {
                btnSpeichern.Enabled = false;
                PanelEnableDisableTag(pnlCreateEdit, "edit", false);
                jsonTool.Save(contacts);
            }
        }

        private void btnErstellen_Click(object sender, EventArgs e)// Speicher Btn --> Daten werden geprüft, wenn diese i.O. sind wird Kontakt erstellt. Ansonsten return-Wert: false
        {
            if (DatenGeprueftGespeichert(create))
            {
                btnErstellen.Visible = false;
                LoadPerson(contacts.Last());
                jsonTool.Save(contacts);
            }
        }

        private bool DatenGeprueftGespeichert(bool createOrUpdate) // Alle Inputs in den Formular-Felder werden in Variablen gespeichert und in die Funktion gegeben. Return wert bei erfolg - true, sonst - false
        {
            string kontakt = cmbxKontakt.Text;
            string status = cmbxStatus.Text;
            string anrede = cmbxAnrede.Text;
            string titel = txtbTitel.Text;
            string name = txtbName.Text;
            string vorname = txtbVorname.Text;
            string geburtsdatum = txtbGeburtsdatum.Text;
            string nationalitaet = txtbNationalitaet.Text;
            string ort = txtbOrt.Text;
            string strasse = txtbStr.Text;
            string plz = txtbPLZ.Text;
            string email = txtBEMail.Text;
            string telMobil = txtbTelM.Text;
            string telPrivat = txtbTelP.Text;
            string telGeschaeft = txtbTelG.Text;
            string kaderstufe = cmbxKader.Text;
            string funktion = txtbFunktion.Text;
            string abteilung = txtbAbteilung.Text;
            string ahvNummer = txtbAHV.Text;
            string angestelltSeit = txtbAngestelltSeit.Text;
            string austrittsdatum = txtbAustrittsdatum.Text;
            string ausbildungsjahr = txtbAusbildungsjahr.Text;
            string ausbildungsdauer = txtbAusbildungsdauer.Text;
            string kundentyp = cmbxKundentyp.Text;
            string firmenname = txtbFirmenname.Text;
            string firmenkontakt = txtbFirmenKontakt.Text;
            string fStrasse = txtbFStrasse.Text;
            string fPLZ = txtbFPLZ.Text;
            string fOrt = txtbFOrt.Text;
           
            List<string> notizen = new List<string>();

            foreach (var item in lstbxNotizen.Items)
            {
                notizen.Add(item.ToString());
            }

            char geschlecht = 'n';
            if (radD.Checked)
            {
                geschlecht = 'd';
            }
            if (radM.Checked)
            {
                geschlecht = 'm';
            }
            if (radW.Checked)
            {
                geschlecht = 'w';
            }

            string pensum = txtbPensum.Text;
            if(pensum == "")
            {
                pensum = "-1";
            }

            int updateIndex = 0;
            if (loadedPerson != null)
            {
                updateIndex = contacts.IndexOf(loadedPerson); // Wenn eine bestehende Person bearbeitet wird, wird hier der Index dieser Person ermittelt
            }


            bool DatenSindGespeichert = DatenTools.DatenPruefenSpeichern(contacts, createOrUpdate, updateIndex, kontakt, status, anrede,
            titel, name, vorname, geschlecht, geburtsdatum, nationalitaet, strasse, plz, ort, telMobil,
            telPrivat, telGeschaeft, email, kaderstufe, funktion, pensum, ahvNummer, abteilung, angestelltSeit, austrittsdatum,
            ausbildungsjahr, ausbildungsdauer, kundentyp, firmenname, firmenkontakt,fStrasse,fPLZ,fOrt, notizen);

            return DatenSindGespeichert;
        }

        private void EmptyPanel(Panel p)// Panel input-Felder werden geleert. Betrifft auch Panel in Panels
        {

            foreach(Control c in p.Controls)
            {
                if(c is Panel)
                {
                    EmptyPanel(c as Panel); //Rekursiv für Child Panel
                }
                if (Convert.ToString(c.Tag).Contains("edit") && c.GetType() == txtbVorname.GetType())
                {
                    c.Text = null;
                }
                if (Convert.ToString(c.Tag).Contains("edit") && c.GetType() == cmbxAnrede.GetType())
                {
                    ClearComboBox(c as ComboBox); 
                }

            }
            if (p == pnlCustomerFields) // pnlCustomerFields hat eine listbox, diese wird hier geleert 
            {
                lstbxNotizen.Items.Clear();
            }
        }
        private void ClearComboBox(ComboBox cb) //Hier wird die Combobox zurückgesetzt
        {
            cb.SelectedItem = cb.Items[0];
        }

        private void btnNeuerKontakt_Click(object sender, EventArgs e) //Leeres Kontaktformular wird geladen
        {
            ClearScreen();
            ResetMenuBtnColor();
            EmptyPanel(pnlCreateEdit);
           
            PanelEnableDisableTag(pnlCreateEdit, "edit", true);
            
            pnlCreateEdit.Visible = true;
            cmbxKontakt.Enabled = true;
            cmbxKontakt.Text = null;
            btnLoeschen.Visible = false;
            btnBearbeiten.Visible = false;
            btnSpeichern.Visible = false;
            btnErstellen.Visible = true;

            txtbMitarbeiterNr.Text = Employee.NaechsteMitarbeiterNummer.ToString(); // Die Mitarbeiternummer für den nächsten MA wird bereits eingetragen
            cmbxKontakt.SelectedIndex = -1; // Kein Kontakt-Typ ausgewählt
            SpecificFieldsVisible();
        }

        private void btnNextPage_Click(object sender, EventArgs e) //Nächste Seite wird aufgerufen 
        {
            ClearContactArea();
            ShowContacts(++aktuelleSeite,showType);
        }

        private void btnLastPage_Click(object sender, EventArgs e) //Letzte Seite wird aufgerufen 
        {

            ClearContactArea();
            ShowContacts(--aktuelleSeite,showType);
        }

        private void ClearContactArea() //Kontaktbereich wird gelöscht (alles ab x >= 200 und y >= 100)
        {
            foreach (Control c in this.Controls)
            {
                if (c.Location.X >= 200 && c.Location.Y >=100 )
                {
                    c.Visible = false ;
                }
            }
        }
        private void btnBeenden_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLoeschen_Click(object sender, EventArgs e) // Löschen-Panel wird angezeigt
        {
            pnlDel.BringToFront();
            pnlDel.Visible = true;
        }

        private void btnBestLoeschen_Click(object sender, EventArgs e) //Person wird aus contacts gelöscht -> Homescreen wird angezeigt
        {
            sucheAktiv = false;
            contacts = DatenTools.PersonLoeschen(loadedPerson, contacts);
            pnlDel.Visible = false;
            jsonTool.Save(contacts);
            ClearScreen();
            ShowHomescreen();
        }

        private void btnAbbrechen_Click(object sender, EventArgs e) 
        {
            pnlDel.Visible = false;
        }

        private void cmbxKontakt_SelectedIndexChanged(object sender, EventArgs e) //Wenn der Selected-Wert von cmbxKontakte ändert, werden andere Spezifischen Felder angezeîgt
        {
            SpecificFieldsVisible();
        }

        private void btnNotizSpeichern_Click(object sender, EventArgs e) //Fügt einer Notiz das Datum hinzu
        {
            if (txtbNotizen.Text != "")
            {
                lstbxNotizen.Items.Add(Convert.ToString(txtbNotizen.Text) + " " + DateTime.Today.ToShortDateString());
                txtbNotizen.Text = "";
            }
        }

        private void btnNotizloeschen_Click(object sender, EventArgs e)// ausgewählte Notiz wird gelöscht
        {
            lstbxNotizen.Items.Remove(lstbxNotizen.SelectedItem);
            lstbxNotizen.ClearSelected();
        }


        private void btnSuche_Click(object sender, EventArgs e) //Suche wird aktiviert - Such-Panel (Suchfilter) wird angezeigt
        {
            ClearScreen();
            ResetMenuBtnColor();
            btnSuche.BackColor = BtnSelectetColor;
            sucheAktiv = false;
            showType = showAll;
            lblAllContacts.Text = "Suchergebnisse:";

            pnlSearch.Visible = true;

            ShowContacts(1, showType);
            txtbSuche.Text = "";
            cmbxSearchField.SelectedIndex = 0;
            cmbxSearchType.SelectedIndex = 0;
        }


        private void btnSearch_Click(object sender, EventArgs e) //Suche wird mit ausgewählten Filter durchgeführt
        {
            if (cmbxSearchField.SelectedIndex == -1)
            {
                MessageBox.Show("Bitte wähle ein Suchkriterium aus");
                return;
            }
            else
            {
                string eingabe = txtbSuche.Text;
                string type = cmbxSearchType.SelectedItem.ToString();
                string suchFeld = cmbxSearchField.SelectedItem.ToString();

                if (suchFeld == "Vorname")
                {
                    suchFeld = "vorname";
                }
                else if (suchFeld == "Name")
                {
                    suchFeld = "name";
                }

                else if (suchFeld == "Geburtsdatum")
                {
                    suchFeld = "geburtsdatum";
                }
                suchErgebnisse = SucheTools.SearchByField(eingabe, suchFeld, type, contacts); //suchErgebnisse wird durch Funktion erstellt
                }

            sucheAktiv = true;
            showType = showAll;
            lblAllContacts.Text = "Suchergebnisse (" + suchErgebnisse.Count + "):";

            ClearContactArea();
            ShowContacts(1, showType); //Erste Seite vom suchErgebnisse wird angezeigt
        }
    }
}

