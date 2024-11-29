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

namespace Projekt_Schuler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private const double StandardPHMin = 7.2;
        private const double StandardPHMax = 7.8;
        private const double StandardTempMin = 24.0;
        private const double StandardTempMax = 28.0;
        private const double StandardChlorMin = 1.0;
        private const double StandardChlorMax = 3.0;
        private const double StandardTurbidityMax = 0.5;

        // Neuer Konstruktor, um den Schwimmbadnamen zu erhalten
        public MainWindow(string poolName,string poolLocation)
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            // Schwimmbadname in der Anzeige setzen
            poolNameDisplay.Text = poolName;
            poolLocationDisplay.Text = poolLocation;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void DisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Versuchen, die Eingabewerte zu lesen und in die entsprechenden Variablen zu konvertieren
            double phValue, temperature, chlorineLevel, turbidity;

            bool isPhValid = double.TryParse(phValueTextBox.Text, out phValue);
            bool isTempValid = double.TryParse(temperatureTextBox.Text, out temperature);
            bool isChlorineValid = double.TryParse(chlorineTextBox.Text, out chlorineLevel);
            bool isTurbidityValid = double.TryParse(turbidityTextBox.Text, out turbidity);

            // Warnmeldungen zurücksetzen
            warningText.Text = string.Empty;

            if (isPhValid && isTempValid && isChlorineValid && isTurbidityValid)
            {
                // Anzeige der eingegebenen Werte
                phValueDisplay.Text = phValue.ToString("F1");
                temperatureDisplay.Text = temperature.ToString("F1");
                chlorineDisplay.Text = chlorineLevel.ToString("F1");
                turbidityDisplay.Text = turbidity.ToString("F1");

                // Schwimmbadname anzeigen (wurde im Konstruktor bereits gesetzt)
                // poolNameDisplay.Text = poolName;   (Nicht nötig, da es bereits oben gesetzt wurde)

                // Vergleich der Werte mit den Standardwerten
                bool hasWarnings = false;
                string warnings = "Warnungen:\n";

                // pH-Wert prüfen
                if (phValue < StandardPHMin || phValue > StandardPHMax)
                {
                    warnings += $"- pH-Wert ({phValue}) ist außerhalb des empfohlenen Bereichs ({StandardPHMin} - {StandardPHMax})\n";
                    if (phValue < StandardPHMin)
                        warnings += "  Vorschlag: Erhöhen Sie den pH-Wert, z.B. durch Zugabe von pH-heben.\n";
                    else
                        warnings += "  Vorschlag: Senken Sie den pH-Wert, z.B. durch Zugabe von pH-senken.\n";
                    hasWarnings = true;
                }

                // Temperatur prüfen
                if (temperature < StandardTempMin || temperature > StandardTempMax)
                {
                    warnings += $"- Temperatur ({temperature}°C) ist außerhalb des empfohlenen Bereichs ({StandardTempMin} - {StandardTempMax})\n";
                    if (temperature < StandardTempMin)
                        warnings += "  Vorschlag: Erhöhen Sie die Wassertemperatur, z.B. durch Nutzung der Heizung.\n";
                    else
                        warnings += "  Vorschlag: Senken Sie die Wassertemperatur, z.B. durch Öffnen der Poolabdeckung.\n";
                    hasWarnings = true;
                }

                // Chlorlevel prüfen
                if (chlorineLevel < StandardChlorMin || chlorineLevel > StandardChlorMax)
                {
                    warnings += $"- Chlorlevel ({chlorineLevel} mg/l) ist außerhalb des empfohlenen Bereichs ({StandardChlorMin} - {StandardChlorMax})\n";
                    if (chlorineLevel < StandardChlorMin)
                        warnings += "  Vorschlag: Erhöhen Sie den Chlorwert, z.B. durch Zugabe von Chlor.\n";
                    else
                        warnings += "  Vorschlag: Senken Sie den Chlorwert, z.B. durch Verdünnung des Wassers.\n";
                    hasWarnings = true;
                }

                // Trübung prüfen
                if (turbidity > StandardTurbidityMax)
                {
                    warnings += $"- Trübung ({turbidity} NTU) ist über dem empfohlenen Maximalwert ({StandardTurbidityMax} NTU)\n";
                    warnings += "  Vorschlag: Klären Sie das Wasser mit einem Flockungsmittel.\n";
                    hasWarnings = true;
                }

                // Warnmeldungen anzeigen, wenn nötig
                if (hasWarnings)
                {
                    warningText.Text = warnings;
                }
                else
                {
                    warningText.Text = "Alle Werte sind innerhalb des empfohlenen Bereichs.";
                }
            }
            else
            {
                MessageBox.Show("Bitte geben Sie gültige Zahlen für alle Werte ein.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


}
