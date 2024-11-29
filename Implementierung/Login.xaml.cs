using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekt_Schuler
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Beispiel: Benutzername = "admin", Passwort = "1234"
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            // Prüfen, ob der Benutzername und das Passwort korrekt sind
            if (username == "admin" && password == "1234")
            {
                // Bei erfolgreichem Login MainWindow öffnen und LoginWindow schließen
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                // Fehlermeldung anzeigen, wenn Login fehlschlägt
                loginErrorText.Text = "Ungültiger Benutzername oder Passwort!";
            }
        }
    }
}
