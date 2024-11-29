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
        string username = usernameTextBox.Text;
        string password = passwordBox.Password;

        if (username == "admin" && password == "1234")
        {
            // Schwimmbadname und Standort holen
            string poolName = poolNameTextBox.Text;
            string poolLocation = poolLocationTextBox.Text;

            if (string.IsNullOrEmpty(poolName) || string.IsNullOrEmpty(poolLocation))
            {
                MessageBox.Show("Bitte geben Sie sowohl den Schwimmbadnamen als auch den Standort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Übergabe des Schwimmbadnamens und Standorts an MainWindow
            MainWindow mainWindow = new MainWindow(poolName, poolLocation);
            mainWindow.Show();
            this.Close();
        }
        else
        {
            loginErrorText.Text = "Ungültiger Benutzername oder Passwort!";
        }
    }

    //private void AddPoolButton_Click(object sender, RoutedEventArgs e)
    //{
    //    string poolName = poolNameTextBox.Text;
    //    string poolLocation = poolLocationTextBox.Text;

    //    if (string.IsNullOrEmpty(poolName) || string.IsNullOrEmpty(poolLocation))
    //    {
    //        MessageBox.Show("Bitte geben Sie sowohl den Schwimmbadnamen als auch den Standort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //    else
    //    {
    //        MessageBox.Show($"Schwimmbad '{poolName}' in '{poolLocation}' wurde erfolgreich hinzugefügt!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
    //        poolNameTextBox.Clear();
    //        poolLocationTextBox.Clear();
    //    }
    //}
}


}