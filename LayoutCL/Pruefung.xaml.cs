using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Pruefung.xaml
    /// </summary>
    public partial class Pruefung : Window
    {


        public Pruefung()
        {
            InitializeComponent();
            Uichipnr.Focus();
        }
        public void SetDarkMode()
        {
            EdvSchule.Background = Brushes.Black;
            this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            Abbrechen.Background = Brushes.Gray;
            Abbrechen.Foreground = Brushes.White;
            Uebernehmen.Background = Brushes.Gray;
            Uebernehmen.Foreground = Brushes.White;
        }

        private void Uebernehmen_click(object sender, RoutedEventArgs e)
        {
            if (Uichipnr.Text.Length == 10)
            {
                if (DbPostgres.Instance.CheckMAsertercard(Uichipnr.Text))
                {
                    MessageBox.Show("Keine Berechtigung", "Rfid_scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    Uichipnr.Text = "";
                }
                else
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                MessageBox.Show("Ungültige Eingabe, Eingabe muss aus 10 Zahlen bestehen!", "Rfid_scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Abbrechen_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Uichipnr_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Uichipnr.Focus();

            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                }
            }
        }
    }
}
