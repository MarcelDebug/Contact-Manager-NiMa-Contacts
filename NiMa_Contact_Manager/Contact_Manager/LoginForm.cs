using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contact_Manager
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            txtbBenutzer.Text = "Patrick";
            txtbBenutzer.Enabled = false;
        }

        private void btnLogin_Click(object sender, EventArgs e) // Login Methode die ein passwort setzt und überprüft
        {
            MD5 md5= MD5.Create(); //hier wird der Input zu einer Zahl gehasht.
            byte[] inputBytes = Encoding.UTF8.GetBytes(txtbPasswort.Text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            string pwHash="";
            foreach (byte b in hashBytes)
            {
                pwHash += b;
            }
            if (txtbBenutzer.Text == "Patrick" && pwHash == "2092011692171101931786918915110548190107154179") //Das richtige PW ergibt diesen Hash-Wert... das PW ist Geheim
            {
                this.DialogResult = DialogResult.OK; //Wenn Login erfolgreich läuft der code weiter
                this.Close();
            }
            else
            {
                MessageBox.Show("Falscher Benutzername oder Passwort!");
            }
        }
        private void btnCancel_Click_1(object sender, EventArgs e) //Abbrechen des Login Vorgangs
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
