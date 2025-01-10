using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaktionslogik für Heraufstufen.xaml
    /// </summary>
    public partial class Heraufstufen : Window
    {
        public Heraufstufen()
        {
            InitializeComponent();
            Abbrechen.Focusable = false;
            Uebernehmen.Focusable = false;
            UITextbox.Focusable = false;
            CardInput.Focus();
        }


        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        private void Uebernehmen_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //password is edvschule => comment must be removed once the password is secured
                                               // hashed with sha256
            if (CardInput.Text.Length == 10 &&
               // GetHashString(UI_passwort.Password) == "2ecd03bc5c72436e8d304bd6c778f9f382a1a0c47eb3b38735a410377e712c54"  ==> Old hash
               UI_passwort.Password == "test"
                )
            {
                if (DbPostgres.Instance.CheckMAsertercard(CardInput.Text))
                {
                    if (DbPostgres.Instance.InsertMastercard(CardInput.Text))
                    {
                        MessageBox.Show("Mastercard erfolreich hinzugefügt", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim hinzufügen dieser Mastercard", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Mastercard bereits vorhanden", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Karte oder Passwort nicht korrekt", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetDarkMode()
        {
            Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            Edvschule.Background = Brushes.Black;
            Abbrechen.Background = Brushes.Gray;
            Abbrechen.Foreground = Brushes.White;
            Uebernehmen.Background = Brushes.Gray;
            Uebernehmen.Foreground = Brushes.White;
        }
        private void CardInput_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        private void Abbrechen_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
            }
            if (e.Key == Key.Enter)
            {
                UI_passwort.Focus();
            }
            else if (!UI_passwort.IsFocused)
            {
                CardInput.Focus();
            }
        }
    }
}
