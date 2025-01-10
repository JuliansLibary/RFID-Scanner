using RFID_Scanner.DatabaseCL;
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

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : Window
    {
        private ComboBox C1 = new ComboBox();
        private Label l1 = new Label();
        readonly List<Veranstaltung_ver> VeranstaltungsListe = new List<Veranstaltung_ver>();
        public Einstellungen()
        {
            InitializeComponent();
            VeranstaltungsListe = DbPostgres.Instance.GetVeranstaltungen();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
            }
        }
        private void Uebernehmen_click(object sender, RoutedEventArgs e)
        {
            if (InfomodusAn.IsChecked == true)
            {
                if (combobox.Child == C1 && C1.SelectedItem != null)
                {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Sie müssen schon eine Veranstaltung auswählen?", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                DialogResult = true;
            }
        }

        public void SetEinstellungen(List<string> Einstellungen)
        {
            if (Einstellungen.Count > 0)
            {
                foreach (var item in Einstellungen)
                {
                    if (item == "InfoModusAn")
                    {
                        InfomodusAn.IsChecked = true;
                    }
                    if (item == "InfoModusAus")
                    {
                        InfomodusAus.IsChecked = true;
                    }
                    if (item == "DarkModeAn")
                    {
                        DarkmodusAn.IsChecked = true;
                        DarkmodeOn();
                    }
                    if (item == "DarkModeAus")
                    {
                        DarkmodusAus.IsChecked = true;
                    }
                }
            }
            else
            {
                InfomodusAus.IsChecked = true;
                DarkmodusAus.IsChecked = true;
            }
        }

        public void DarkmodeOn()
        {
            Einstellungen_ui.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            Uebernehmen.Foreground = Brushes.White;
            Uebernehmen.Background = Brushes.Gray;
            Abbrechen.Foreground = Brushes.White;
            Abbrechen.Background = Brushes.Gray;
            C1.Background = Brushes.Gray;
        }
        public List<string> ReturnEinstellungen()
        {
            List<string> Einstellungen = new List<string>();
            if (InfomodusAn.IsChecked == true)
            {
                Einstellungen.Add("InfoModusAn");
                Einstellungen.Add(C1.SelectedItem as string);
            }
            else if (InfomodusAus.IsChecked == true)
            {
                Einstellungen.Add("InfoModusAus");
            }
            if (DarkmodusAn.IsChecked == true)
            {
                Einstellungen.Add("DarkModeAn");
            }
            else if (DarkmodusAus.IsChecked == true)
            {
                Einstellungen.Add("DarkModeAus");
            }
            return Einstellungen;
        }

        private void Abbrechen_click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void InfomodusAus_Checked(object sender, RoutedEventArgs e)
        {
            label.Child = null;
            combobox.Child = null;
        }

        private void InfomodusAn_Checked(object sender, RoutedEventArgs e)
        {
            C1 = new ComboBox()
            {
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                FontWeight = FontWeights.Light,
                FontSize = 14
            };
            foreach (var item in VeranstaltungsListe)
            {
                C1.Items.Add(item.SBezeichnung);
            }
            l1 = new Label()
            {
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                FontWeight = FontWeights.Light,
                FontSize = 20,
                Content = "Veranstaltung auswählen!"
            };

            Grid.SetColumn(l1, 2);
            Grid.SetColumnSpan(l1, 6);
            Grid.SetRow(l1, 4);

            Grid.SetColumn(C1, 2);
            Grid.SetColumnSpan(C1, 6);
            Grid.SetRow(C1, 5);
            label.Child = l1;
            combobox.Child = C1;
        }
    }
}
