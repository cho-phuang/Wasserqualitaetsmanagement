﻿using System;
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
using System.Data.SqlClient;

namespace Projekt_Schuler
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
  public partial class Login : Window
{
        private string connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
     

        private static bool databaseInitialized = false; // Prüft, ob die DB schon initialisiert wurde

        public Login()
        {
            if (!databaseInitialized)
            {
                InitializeDatabase();
                databaseInitialized = true; // Setzt die Variable auf "true", damit es nicht erneut passiert
            }
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
    }
        private void InitializeDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Schritt 1: Datenbank erstellen, falls sie nicht existiert
                    string createDatabaseScript = @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'wasser')
                BEGIN
                    CREATE DATABASE wasser;
                END
            ";

                    using (SqlCommand command = new SqlCommand(createDatabaseScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Schritt 2: Wechsel zur 'wasser' Datenbank
                    string useDatabaseScript = "USE wasser;";
                    using (SqlCommand command = new SqlCommand(useDatabaseScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Schritt 3: Restliches SQL-Skript für Tabellen und Daten
                    string sqlScript = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
                BEGIN
                    CREATE TABLE Users (
                        UserID INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(50) NOT NULL UNIQUE,
                        Password NVARCHAR(255) NOT NULL, 
                        LastLogin DATETIME NULL
                    );
                END
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'IsAdmin')
                BEGIN
                ALTER TABLE Users ADD IsAdmin BIT DEFAULT 0;
                END

                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Pools')
                BEGIN
                    CREATE TABLE Pools (
                        PoolID INT IDENTITY(1,1) PRIMARY KEY,
                        PoolName NVARCHAR(100) NOT NULL,
                        Location NVARCHAR(100) NULL,
                        UserID INT NULL,
                        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                    );
                END

                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WaterQualityData')
                BEGIN
                CREATE TABLE WaterQualityData (
                DataID INT IDENTITY(1,1) PRIMARY KEY,
                UserID INT NULL,
                PoolID INT NULL,
                PHValue DECIMAL(5,2) NULL,
                Temperature DECIMAL(5,2) NULL,
                ChlorineLevel DECIMAL(5,2) NULL,
                Turbidity DECIMAL(5,2) NULL,
                EntryDate DATETIME DEFAULT GETDATE(),
                FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE ON UPDATE CASCADE,
                FOREIGN KEY (PoolID) REFERENCES Pools(PoolID) ON DELETE NO ACTION ON UPDATE NO ACTION
);

                END

                IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
                BEGIN
                INSERT INTO Users (Username, Password, LastLogin, IsAdmin) 
                VALUES 
                ('admin', 'hashedpassword123', NULL, 1),  -- Admin mit IsAdmin = 1
                ('user1', 'hashedpassword456', NULL, 0),
                ('user2', 'hashedpassword789', NULL, 0);
                END


                IF NOT EXISTS (SELECT 1 FROM Pools WHERE PoolName = 'Stadtbad')
                BEGIN
                    INSERT INTO Pools (PoolName, Location, UserID) 
                    VALUES 
                        ('Stadtbad', 'Berlin', (SELECT UserID FROM Users WHERE Username = 'admin')),
                        ('Freibad West', 'München', (SELECT UserID FROM Users WHERE Username = 'user1')),
                        ('Therme Aqua', 'Hamburg', (SELECT UserID FROM Users WHERE Username = 'user2'));
                END

                IF NOT EXISTS (SELECT 1 FROM WaterQualityData WHERE PoolID = (SELECT PoolID FROM Pools WHERE PoolName = 'Stadtbad'))
                BEGIN
                    INSERT INTO WaterQualityData (UserID, PoolID, PHValue, Temperature, ChlorineLevel, Turbidity) 
                    VALUES 
                        ((SELECT UserID FROM Users WHERE Username = 'admin'), (SELECT PoolID FROM Pools WHERE PoolName = 'Stadtbad'), 7.2, 24.5, 0.8, 1.2),
                        ((SELECT UserID FROM Users WHERE Username = 'user1'), (SELECT PoolID FROM Pools WHERE PoolName = 'Freibad West'), 7.5, 22.3, 0.9, 1.0),
                        ((SELECT UserID FROM Users WHERE Username = 'user2'), (SELECT PoolID FROM Pools WHERE PoolName = 'Therme Aqua'), 7.1, 28.0, 1.0, 0.8);
                END
            ";

                    // Führe das restliche Skript aus
                    using (SqlCommand command = new SqlCommand(sqlScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Datenbank und Tabellen wurden erfolgreich erstellt oder sind bereits vorhanden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen oder Verbinden zur Datenbank: " + ex.Message);
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password; // In einer echten Anwendung sollte hier eine Hash-Überprüfung erfolgen

            using (SqlConnection conn = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=wasser;Integrated Security=SSPI"))
            {
                conn.Open();
                string query = "SELECT UserID FROM Users WHERE Username = @username AND Password = @password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // In einer echten Anwendung: Gehashtes Passwort überprüfen

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int userId = Convert.ToInt32(result);
                        MessageBox.Show("Login erfolgreich! Benutzer-ID: " + userId);

                        // Hier kannst du z. B. zur nächsten Seite der Anwendung navigieren
                        MainWindow mainWindow = new MainWindow(userId);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        loginErrorText.Text = "Falscher Benutzername oder Passwort.";
                    }
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordBox.Password.Trim(); // In einer echten Anwendung sollte das Passwort gehasht werden

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus!", "Fehlende Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=wasser;Integrated Security=SSPI"))
            {
                try
                {
                    conn.Open();

                    // Überprüfen, ob der Benutzername bereits existiert
                    string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        int userExists = (int)checkCmd.ExecuteScalar();

                        if (userExists > 0)
                        {
                            MessageBox.Show("Dieser Benutzername ist bereits vergeben. Bitte wählen Sie einen anderen.", "Benutzername vergeben", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Benutzer registrieren
                    string query = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password); // In einer echten Anwendung: Gehashtes Passwort speichern

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registrierung erfolgreich! Sie können sich nun einloggen.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                            usernameTextBox.Clear();
                            passwordBox.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Registrierung fehlgeschlagen. Bitte versuchen Sie es erneut.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler bei der Registrierung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }


}