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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Projekt_Schuler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private string connectionString = "Server=DESKTOP-JJAKV1E;Database=wasser;Trusted_Connection=True;";
        private int userId;

        // Standardwerte für Wasserqualität
        private const double StandardPHMin = 7.2;
        private const double StandardPHMax = 7.8;
        private const double StandardTempMin = 24.0;
        private const double StandardTempMax = 28.0;
        private const double StandardChlorMin = 1.0;
        private const double StandardChlorMax = 3.0;
        private const double StandardTurbidityMax = 0.5;

        // Konstruktor
        public MainWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            this.WindowState = WindowState.Maximized;

            LoadPoolData();
        }

        // Pool-Daten aus der Datenbank laden
        private void LoadPoolData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT p.PoolName, p.Location, w.PHValue, w.Temperature, w.ChlorineLevel, w.Turbidity
                        FROM Pools p
                        LEFT JOIN WaterQualityData w ON p.PoolID = w.PoolID
                        WHERE p.UserID = @userId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                poolNameDisplay.Text = reader["PoolName"].ToString();
                                poolLocationDisplay.Text = reader["Location"].ToString();
                                phValueTextBox.Text = reader["PHValue"].ToString();
                                temperatureTextBox.Text = reader["Temperature"].ToString();
                                chlorineTextBox.Text = reader["ChlorineLevel"].ToString();
                                turbidityTextBox.Text = reader["Turbidity"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Keine Pool-Daten gefunden!", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Änderungen speichern
        private void SavePoolData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Werte aus den TextBoxen holen und konvertieren
                    if (double.TryParse(phValueTextBox.Text, out double phValue) &&
                        double.TryParse(temperatureTextBox.Text, out double temperature) &&
                        double.TryParse(chlorineTextBox.Text, out double chlorineLevel) &&
                        double.TryParse(turbidityTextBox.Text, out double turbidity))
                    {
                        string query = @"
                    UPDATE WaterQualityData
                    SET PHValue = @ph, Temperature = @temp, ChlorineLevel = @chlorine, Turbidity = @turbidity
                    WHERE PoolID = (SELECT PoolID FROM Pools WHERE UserID = @userId)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ph", phValue);
                            cmd.Parameters.AddWithValue("@temp", temperature);
                            cmd.Parameters.AddWithValue("@chlorine", chlorineLevel);
                            cmd.Parameters.AddWithValue("@turbidity", turbidity);
                            cmd.Parameters.AddWithValue("@userId", userId);

                            int rowsUpdated = cmd.ExecuteNonQuery();
                            if (rowsUpdated > 0)
                            {
                                MessageBox.Show("Daten erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Fehler beim Speichern der Daten. Möglicherweise existiert kein passender Pool.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bitte geben Sie gültige Zahlenwerte ein.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Warnungen basierend auf den eingegebenen Werten anzeigen
        private void DisplayWarnings()
        {
            double phValue, temperature, chlorineLevel, turbidity;

            // Werte aus den Textboxen lesen und umwandeln
            bool isPhValid = double.TryParse(phValueTextBox.Text, out phValue);
            bool isTempValid = double.TryParse(temperatureTextBox.Text, out temperature);
            bool isChlorineValid = double.TryParse(chlorineTextBox.Text, out chlorineLevel);
            bool isTurbidityValid = double.TryParse(turbidityTextBox.Text, out turbidity);

            // Fehlerprüfung
            if (!isPhValid || !isTempValid || !isChlorineValid || !isTurbidityValid)
            {
                MessageBox.Show("Bitte geben Sie gültige Zahlenwerte ein.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Warnungen generieren
            string warnings = "Warnungen:\n";
            bool hasWarnings = false;

            if (phValue < StandardPHMin || phValue > StandardPHMax)
            {
                warnings += $"- pH-Wert ({phValue}) ist außerhalb des empfohlenen Bereichs ({StandardPHMin} - {StandardPHMax}).\n";
                hasWarnings = true;
            }

            if (temperature < StandardTempMin || temperature > StandardTempMax)
            {
                warnings += $"- Temperatur ({temperature}°C) ist außerhalb des empfohlenen Bereichs ({StandardTempMin} - {StandardTempMax}).\n";
                hasWarnings = true;
            }

            if (chlorineLevel < StandardChlorMin || chlorineLevel > StandardChlorMax)
            {
                warnings += $"- Chlorlevel ({chlorineLevel} mg/l) ist außerhalb des empfohlenen Bereichs ({StandardChlorMin} - {StandardChlorMax}).\n";
                hasWarnings = true;
            }

            if (turbidity > StandardTurbidityMax)
            {
                warnings += $"- Trübung ({turbidity} NTU) überschreitet den Maximalwert ({StandardTurbidityMax} NTU).\n";
                hasWarnings = true;
            }

            // Anzeige der Warnungen oder Erfolgsmeldung
            warningText.Text = hasWarnings ? warnings : "Alle Werte sind innerhalb des empfohlenen Bereichs.";
        }

        // Event-Handler für den Speichern-Button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePoolData();
        }

        // Event-Handler für den Warnungs-Button
        private void DisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayWarnings();
        }

        // Logout-Funktion
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}

