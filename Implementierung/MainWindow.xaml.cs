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
        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=wasser;Trusted_Connection=True;";
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
            LoadPools();
        }
        // Methode, um die UserID des eingeloggten Benutzers zu erhalten
        private int GetLoggedInUserId()
        {
            return this.userId;
        }
        private bool IsAdminUser()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT IsAdmin FROM Users WHERE UserID = @userId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    object result = cmd.ExecuteScalar();
                    return result != null && Convert.ToBoolean(result);
                }
            }
        }


        // Pools für den Benutzer laden
        private void LoadPools()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Prüfen, ob der Benutzer ein Admin ist
                    bool isAdmin = IsAdminUser();
                    string query;

                    if (isAdmin)
                    {
                        // Admin sieht alle Pools
                        query = "SELECT PoolID, PoolName FROM Pools";
                    }
                    else
                    {
                        // Normale User sehen nur ihre eigenen Pools
                        query = "SELECT PoolID, PoolName FROM Pools WHERE UserID = @userId";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!isAdmin)
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            poolSelectionComboBox.Items.Clear();
                            while (reader.Read())
                            {
                                int poolId = reader.GetInt32(0);
                                string poolName = reader.GetString(1);
                                poolSelectionComboBox.Items.Add(new ComboBoxItem { Content = poolName, Tag = poolId });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der Schwimmbäder: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Pool-Daten aus der Datenbank laden
        private void LoadPoolData(int poolId)
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
                        WHERE p.PoolID = @poolId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@poolId", poolId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                poolNameDisplay.Text = reader["PoolName"].ToString();
                                poolLocationDisplay.Text = reader["Location"].ToString();
                                phValueTextBox.Text = reader["PHValue"]?.ToString() ?? "";
                                temperatureTextBox.Text = reader["Temperature"]?.ToString() ?? "";
                                chlorineTextBox.Text = reader["ChlorineLevel"]?.ToString() ?? "";
                                turbidityTextBox.Text = reader["Turbidity"]?.ToString() ?? "";
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

        // Neuer Pool hinzufügen
        private void AddPoolButton_Click(object sender, RoutedEventArgs e)
        {
            string newPoolName = newPoolNameTextBox.Text.Trim();
            string newPoolLocation = newPoolLocationTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newPoolName) || string.IsNullOrEmpty(newPoolLocation))
            {
                MessageBox.Show("Bitte geben Sie sowohl einen Namen als auch einen Standort für das Schwimmbad ein.",
                                "Fehlende Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Überprüfen, ob das Schwimmbad bereits existiert
                    string checkQuery = "SELECT COUNT(*) FROM Pools WHERE PoolName = @poolName AND Location = @location";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@poolName", newPoolName);
                        checkCmd.Parameters.AddWithValue("@location", newPoolLocation);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            MessageBox.Show("Dieses Schwimmbad existiert bereits mit dem gleichen Namen und Standort.",
                                            "Duplikat gefunden", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Schwimmbad hinzufügen
                    string query = "INSERT INTO Pools (PoolName, Location, UserID) VALUES (@poolName, @location, @userId)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@poolName", newPoolName);
                        cmd.Parameters.AddWithValue("@location", newPoolLocation);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        int rowsInserted = cmd.ExecuteNonQuery();
                        if (rowsInserted > 0)
                        {
                            MessageBox.Show("Schwimmbad erfolgreich hinzugefügt!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                            newPoolNameTextBox.Clear();
                            newPoolLocationTextBox.Clear();
                            LoadPools(); // Aktualisiert die Pool-Liste
                        }
                        else
                        {
                            MessageBox.Show("Fehler beim Hinzufügen des Schwimmbads.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Auswahl eines Pools im Dropdown-Menü
        private void PoolSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (poolSelectionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int selectedPoolId = (int)selectedItem.Tag;
                LoadPoolData(selectedPoolId);
            }
        }

        // Pool-Daten speichern
        private void SavePoolData()
        {
            if (poolSelectionComboBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                MessageBox.Show("Bitte wählen Sie zuerst ein Schwimmbad aus.", "Fehlende Auswahl", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Stelle sicher, dass die userId korrekt abgerufen wird
            int userId = GetLoggedInUserId();  // Hier wird die userId über die Methode abgerufen
            int selectedPoolId = (int)selectedItem.Tag;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    if (double.TryParse(phValueTextBox.Text, out double phValue) &&
                        double.TryParse(temperatureTextBox.Text, out double temperature) &&
                        double.TryParse(chlorineTextBox.Text, out double chlorineLevel) &&
                        double.TryParse(turbidityTextBox.Text, out double turbidity))
                    {
                        string query = @"
                        MERGE INTO WaterQualityData AS target
                        USING (SELECT @poolId AS PoolID) AS source
                        ON target.PoolID = source.PoolID
                        WHEN MATCHED THEN
                        UPDATE SET PHValue = @ph, Temperature = @temp, ChlorineLevel = @chlorine, Turbidity = @turbidity
                        WHEN NOT MATCHED THEN
                        INSERT (PoolID, PHValue, Temperature, ChlorineLevel, Turbidity, UserID)
                        VALUES (@poolId, @ph, @temp, @chlorine, @turbidity, @userId);";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Stelle sicher, dass alle Parameter korrekt gesetzt sind
                            cmd.Parameters.AddWithValue("@poolId", selectedPoolId);
                            cmd.Parameters.AddWithValue("@ph", phValue);
                            cmd.Parameters.AddWithValue("@temp", temperature);
                            cmd.Parameters.AddWithValue("@chlorine", chlorineLevel);
                            cmd.Parameters.AddWithValue("@turbidity", turbidity);
                            cmd.Parameters.AddWithValue("@userId", userId);  // Hier wird die userId als Parameter übergeben

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Daten erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
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



        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

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

            // Warnungen & Verbesserungsvorschläge
            string warnings = "Warnungen:\n";
            bool hasWarnings = false;

            // pH-Wert Prüfung
            if (phValue < StandardPHMin)
            {
                warnings += $"- pH-Wert ({phValue}) ist zu niedrig. Erhöhen Sie den pH-Wert durch Zugabe von Natriumcarbonat. Der empfohlene Bereich liegt zwischen {StandardPHMin} und {StandardPHMax}.\n";
                hasWarnings = true;
            }
            else if (phValue > StandardPHMax)
            {
                warnings += $"- pH-Wert ({phValue}) ist zu hoch. Senken Sie den pH-Wert mit pH-Senker (z.B. Schwefelsäure oder Natriumbisulfat). Der empfohlene Bereich liegt zwischen {StandardPHMin} und {StandardPHMax}.\n";
                hasWarnings = true;
            }

            // Temperatur Prüfung
            if (temperature < StandardTempMin)
            {
                warnings += $"- Die Wassertemperatur ({temperature}°C) ist zu niedrig. Erwägen Sie eine Poolheizung. Der empfohlene Bereich liegt zwischen {StandardTempMin}°C und {StandardTempMax}°C.\n";
                hasWarnings = true;
            }
            else if (temperature > StandardTempMax)
            {
                warnings += $"- Die Wassertemperatur ({temperature}°C) ist zu hoch. Lassen Sie den Pool abkühlen oder erhöhen Sie die Belüftung. Der empfohlene Bereich liegt zwischen {StandardTempMin}°C und {StandardTempMax}°C.\n";
                hasWarnings = true;
            }

            // Chlorlevel Prüfung
            if (chlorineLevel < StandardChlorMin)
            {
                warnings += $"- Chlorlevel ({chlorineLevel} mg/l) ist zu niedrig. Fügen Sie Chlor hinzu, um Bakterienwachstum zu verhindern. Der empfohlene Bereich liegt zwischen {StandardChlorMin} mg/l und {StandardChlorMax} mg/l.\n";
                hasWarnings = true;
            }
            else if (chlorineLevel > StandardChlorMax)
            {
                warnings += $"- Chlorlevel ({chlorineLevel} mg/l) ist zu hoch. Reduzieren Sie die Chlormenge oder belüften Sie das Wasser. Der empfohlene Bereich liegt zwischen {StandardChlorMin} mg/l und {StandardChlorMax} mg/l.\n";
                hasWarnings = true;
            }

            // Trübung Prüfung
            if (turbidity > StandardTurbidityMax)
            {
                warnings += $"- Trübung ({turbidity} NTU) ist zu hoch. Prüfen Sie das Filtersystem und fügen Sie Flockungsmittel hinzu. Der empfohlene Maximalwert liegt bei {StandardTurbidityMax} NTU.\n";
                hasWarnings = true;
            }

            // Anzeige der Warnungen oder Erfolgsmeldung
            warningText.Text = hasWarnings ? warnings : "Alle Werte sind innerhalb des empfohlenen Bereichs.";
        }


        private void DisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(phValueTextBox.Text, out double ph) &&
                double.TryParse(temperatureTextBox.Text, out double temperature) &&
                double.TryParse(chlorineTextBox.Text, out double chlorine) &&
                double.TryParse(turbidityTextBox.Text, out double turbidity))
            {
                // Falls alle Eingaben gültige Zahlen sind, Daten anzeigen
                phValueDisplay.Text = ph.ToString("0.00");
                temperatureDisplay.Text = temperature.ToString("0.0") + " °C";
                chlorineDisplay.Text = chlorine.ToString("0.00") + " mg/l";
                turbidityDisplay.Text = turbidity.ToString("0.00") + " NTU";
            }
            else
            {
                MessageBox.Show("Bitte geben Sie gültige numerische Werte ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            DisplayWarnings();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePoolData();
        }
        //Löschen von Schwimmbäder
        private void DeletePoolButton_Click(object sender, RoutedEventArgs e)
        {
            if (poolSelectionComboBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                MessageBox.Show("Bitte wählen Sie zuerst ein Schwimmbad aus.", "Fehlende Auswahl", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedPoolId = (int)selectedItem.Tag;

            // Bestätigung vom Benutzer einholen
            MessageBoxResult result = MessageBox.Show("Möchten Sie dieses Schwimmbad wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        // Beginne mit einer Transaktion, um sicherzustellen, dass beide Tabellen zusammen gelöscht werden
                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Zuerst Daten aus der WaterQualityData-Tabelle löschen
                                string deleteWaterQualityQuery = "DELETE FROM WaterQualityData WHERE PoolID = @poolId";
                                using (SqlCommand cmd = new SqlCommand(deleteWaterQualityQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@poolId", selectedPoolId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Dann das Schwimmbad aus der Pools-Tabelle löschen
                                string deletePoolQuery = "DELETE FROM Pools WHERE PoolID = @poolId";
                                using (SqlCommand cmd = new SqlCommand(deletePoolQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@poolId", selectedPoolId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Commit der Transaktion, wenn alles erfolgreich war
                                transaction.Commit();
                                MessageBox.Show("Schwimmbad erfolgreich gelöscht!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadPools(); // Liste der Pools neu laden
                            }
                            catch (Exception ex)
                            {
                                // Bei einem Fehler zurückrollen
                                transaction.Rollback();
                                MessageBox.Show($"Fehler beim Löschen des Schwimmbads: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler beim Verbinden mit der Datenbank: {ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

    }
}