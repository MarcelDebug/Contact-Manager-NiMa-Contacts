using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contact_Manager
{
    internal class AnzeigeTools
    {
        public static void ShowContact(Form1 form, Person p, int x, int y, Font showFont) //Anzeigefeld wird generiert
        {
            string showName = p.Name + " " + p.Vorname;
            string showDate;
            if (p.Geburtsdatum != null && p.Geburtsdatum != "") //String für Geburtsdatum wird erstellt
            {
                showDate = p.Geburtsdatum;
            }
            else
            {
                showDate = "-";
            }

            string showKontakt; //Typ wird ermittelt
            switch (p)
            {
                case Trainee t:
                    showKontakt = "Lernender";
                    break;
                case Employee e:
                    showKontakt = "Mitarbeiter";
                    break;
                case Customer c:
                    showKontakt = "Kunde";
                    break;
                default:
                    showKontakt = "Person";
                    break;

            }


            Label labelKontakt = new Label(); //Label - Typ
            labelKontakt.Text = showKontakt;
            labelKontakt.Font = showFont;
            labelKontakt.Tag = p.ID.ToString();
            labelKontakt.Location = new Point(x, y);
            labelKontakt.Width = 240;
            form.Controls.Add(labelKontakt);
            labelKontakt.BringToFront();

            Label labelTitel = new Label();  //Label - Titel
            labelTitel.Text = p.Titel;
            labelTitel.Font = showFont;
            labelTitel.Tag = p.ID.ToString();
            labelTitel.Location = new Point(x + 110, y);
            labelTitel.Width = 240;
            form.Controls.Add(labelTitel);
            labelTitel.BringToFront();

            Label labelName = new Label(); //Label - Name
            labelName.Text = showName;
            labelName.Font = showFont;
            labelName.Tag = p.ID.ToString();
            labelName.Location = new Point(x + 210, y);
            labelName.Width = 240;
            form.Controls.Add(labelName);
            labelName.BringToFront();

            Label labelDate = new Label(); //Label - Geburtsdatum
            labelDate.Text = showDate;
            labelDate.Font = showFont;
            labelDate.Tag = p.ID.ToString();
            labelDate.Location = new Point(x + 500, y);
            form.Controls.Add(labelDate);
            labelDate.BringToFront();

            Button editContact = new Button(); //Edit-Button wird erstellt. Tag enthält die ID von der Person damit die Person wieder gefunden wird
            editContact.Text = "🔧";
            editContact.Font = showFont;
            editContact.Tag = p.ID.ToString();
            editContact.Width = 30;
            editContact.Height = 25;
            editContact.Location = new Point(x + 650, y - 5);
            form.Controls.Add(editContact);
            editContact.BringToFront();

            editContact.Click += new System.EventHandler(form.editContact_Click);


            Panel linie = new Panel(); // Optische Trennlinie
            linie.Height = 1;
            linie.Width = 680;
            linie.BackColor = Color.Gray;
            linie.Location = new Point(x, y + 20);
            form.Controls.Add(linie);
            linie.BringToFront();

        }
        public static void ShowTitles(Form1 form, int x, int y, Font showFont, Color titelColor) // Titelfelder werden erstellt. Gleiches Prinzip wie bei ShowContact
        {
            Label labelKontaktHead = new Label();
            labelKontaktHead.Text = "Kontakt:";
            labelKontaktHead.Font = showFont;
            labelKontaktHead.ForeColor = titelColor;
            labelKontaktHead.Location = new Point(x, y);
            labelKontaktHead.Width = 240;
            form.Controls.Add(labelKontaktHead);
            labelKontaktHead.BringToFront();

            Label labelTitelHead = new Label();
            labelTitelHead.Text = "Titel:";
            labelTitelHead.Font = showFont;
            labelTitelHead.ForeColor = titelColor;
            labelTitelHead.Location = new Point(x + 110, y);
            labelTitelHead.Width = 240;
            form.Controls.Add(labelTitelHead);
            labelTitelHead.BringToFront();

            Label labelNameHead = new Label();
            labelNameHead.Text = "Name:";
            labelNameHead.Font = showFont;
            labelNameHead.ForeColor = titelColor;
            labelNameHead.Location = new Point(x + 210, y);
            labelNameHead.Width = 240;
            form.Controls.Add(labelNameHead);
            labelNameHead.BringToFront();

            Label labelDateHead = new Label();
            labelDateHead.Text = "Datum:";
            labelDateHead.Font = showFont;
            labelDateHead.ForeColor = titelColor;
            labelDateHead.Location = new Point(x + 500, y);
            form.Controls.Add(labelDateHead);
            labelDateHead.BringToFront();

            Label labelEditHead = new Label();
            labelEditHead.Text = "Edit:";
            labelEditHead.Font = showFont;
            labelEditHead.ForeColor = titelColor;
            labelEditHead.Location = new Point(x + 645, y);
            form.Controls.Add(labelEditHead);
            labelEditHead.BringToFront();


            Panel linie = new Panel();
            linie.Height = 2;
            linie.Width = 680;
            linie.BackColor = titelColor; ;
            linie.Location = new Point(x, y + 20);
            form.Controls.Add(linie);
            linie.BringToFront();
        }
    }
}
