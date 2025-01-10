using RFID_Scanner.DatabaseCL;
using RFID_Scanner.LayoutCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Data.SqlTypes;
using System.Linq.Expressions;

namespace RFID_Scanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly bool InfoModus = false;

        
        List<string> Einstellungen = new List<string>();
        List<List<string>> EinstellungenList;
        Ver_St_Akt_ZwischenTabelle Standanzeige = new Ver_St_Akt_ZwischenTabelle();
        bool Darkmode = false;
        Rectangle r1 = new Rectangle();
        public MainWindow()
        {
            bool item = Passwort_Überprüfung(1, "test");
            if (item == true)
            {
                Console.WriteLine("Passwort ist richtig");
            }
            else
            {
                Console.WriteLine("Passwort ist falsch");
            }
            InitializeComponent();
            if (Console.CapsLock)
            {
                ImageBrush bg = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("../../Images/warning.png", UriKind.Relative))
                };
                r1 = new Rectangle()
                {
                    Fill = bg,
                    ToolTip = "Achtung, Sie haben CapsLock aktiviert!"
                };
                Grid.SetColumn(r1, 10);
                Grid.SetRow(r1, 4);
                GridUI.Children.Add(r1);
            }
            UIDatagrid.BorderBrush = Brushes.Gray;
            UITextbox.Focus();
            UITextbox.Text = "";
            UIVeranstaltung.Text = "";
        }
        public void UpdateDataGrid()
        {
            UIDatagrid.Items.Clear();
            int AktId = DbPostgres.Instance.AktionsIDvonName(UIVeranstaltung.Text);
            List<Teilnehmer_teil> Teilnehmer = DbPostgres.Instance.GetTeilnehmerZuAktID(AktId);
            foreach (var item in Teilnehmer)
            {
                UIDatagrid.Items.Add(new Teilnehmer_teil() { Chip_ID = item.Chip_ID, Zeitstempel = item.Zeitstempel });
            }
        }
        public void Veranstaltung_click(object sender, MouseButtonEventArgs e)
        {
            if (!InfoModus)
            {
                Veranstaltung vc1 = new Veranstaltung();
                Pruefung pc1 = new Pruefung();

                if (Darkmode)
                {
                    pc1.SetDarkMode();
                    vc1.SetDarkMode();
                }

                bool? Pruefung = pc1.ShowDialog();
                if (Pruefung == true)
                {
                    bool? Pruefung_veranstaltung = vc1.ShowDialog();
                    if (Pruefung_veranstaltung == true)
                    {
                        Standanzeige = vc1.SetVeranstaltung();
                        UIVeranstaltung.IsReadOnly = false;
                        UIVeranstaltung.Text = Standanzeige.ABezeichnung;
                        Veranstaltung_textbox.Text = Standanzeige.VBezeichnung;
                        UpdateDataGrid();
                        UIVeranstaltung.IsReadOnly = true;
                    }
                }
            }
        }
        private void Info_click(object sender, MouseButtonEventArgs e)
        {
            if (UIVeranstaltung.Text != "")
            {
                Info ic1 = new Info();
                ic1.SetVeranstaltunsName(Veranstaltung_textbox.Text);

                if (Darkmode)
                {
                    ic1.SetDarkmode();
                }
                if (ic1.ShowDialog() == false)
                {
                    ic1.StopTimer();
                }
            }
            else
            {
                MessageBox.Show("Suchen Sie erst eine Aktion aus!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Hochstufen_click(object sender, MouseButtonEventArgs e)
        {
            Heraufstufen hc1 = new Heraufstufen();
            if (Darkmode)
            {
                hc1.SetDarkMode();
            }
            hc1.ShowDialog();
        }

        public bool Passwort_Überprüfung(int id, string beschreibung)
        {
            if (beschreibung == "test")
            {
                string item2 = uebersetzer(beschreibung);
                Console.WriteLine(item2);
                return true;
            }
            else
            {
                return false;
            }
        }
        public string uebersetzer(string beschreibung)
        {
            if (beschreibung == "test")
            {
                return "test2.0";
            }
            return "Hier gibt es keine Übersetzung";
        }

        private void Drucken_click(object sender, MouseButtonEventArgs e)
        {
            if (UIVeranstaltung.Text != "")
            {
                if (!InfoModus)
                {
                    Pruefung pc1 = new Pruefung();

                    if (Darkmode)
                    {
                        pc1.SetDarkMode();
                    }

                    bool? Pruefung = pc1.ShowDialog();
                    if (Pruefung == true)
                    {
                        Drucken dc1 = new Drucken();
                        dc1.SetVeranstaltung(Veranstaltung_textbox.Text);
                        dc1.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show("Suchen Sie erst eine Aktion aus!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MouseleaveDrucken(object sender, MouseEventArgs e)
        {
            drucken.FontWeight = FontWeights.Light;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MouseEnterDrucken(object sender, MouseEventArgs e)
        {
                drucken.FontWeight = FontWeights.Bold;
                Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Mouseleavehochstufen(object sender, MouseEventArgs e)
        {
            hochstufen.FontWeight = FontWeights.Light;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MouseEnterhochstufen(object sender, MouseEventArgs e)
        {
            hochstufen.FontWeight = FontWeights.Bold;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Mouseleaveinfo(object sender, MouseEventArgs e)
        {
            info.FontWeight = FontWeights.Light;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MouseEnterinfo(object sender, MouseEventArgs e)
        {
            info.FontWeight = FontWeights.Bold;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Mouseleaveveranstaltungen(object sender, MouseEventArgs e)
        {
            if (!InfoModus)
            {
                veranstaltung.FontWeight = FontWeights.Light;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void MouseEnterveranstaltungen(object sender, MouseEventArgs e)
        {
            if (!InfoModus)
            {
                veranstaltung.FontWeight = FontWeights.Bold;
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void SizeChange(object sender, SizeChangedEventArgs e)
        {
            UIDatagrid.Width = this.Height * 0.50;
            UIDatagrid.Height = this.Width * 0.50;
        }

        private void Löschen_click(object sender, RoutedEventArgs e)
        {
            if (UIDatagrid.SelectedItem is Teilnehmer_teil t1)
            {
                if (MessageBox.Show("Wollen Sie den Teilnehmer wirklich löschen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Pruefung p1 = new Pruefung();
                    if (Darkmode)
                    {
                        p1.SetDarkMode();
                    }

                    bool? Pruefung = p1.ShowDialog();
                    if (Pruefung == true)
                    {
                        DbPostgres.Instance.TeilnehmerLoeschen(t1.Chip_ID, t1.Zeitstempel);
                        MessageBox.Show("Teilnehmer gelöscht!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdateDataGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show("Keinen Teilnehmer ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            UITextbox.Focus();
            if (e.Key == Key.CapsLock)
            {
                if (Console.CapsLock)
                {
                    ImageBrush bg = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("../../Images/warning.png", UriKind.Relative))
                    };
                    r1 = new Rectangle()
                    {
                        Fill = bg,
                        ToolTip = "Achtung, Sie haben CapsLock aktiviert!"
                    };
                    Grid.SetColumn(r1, 10);
                    Grid.SetRow(r1, 4);
                    GridUI.Children.Add(r1);
                }
                else
                {
                    if (GridUI.Children.Contains(r1))
                    {
                        GridUI.Children.Remove(r1);
                    }
                }
            }
            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void UITextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            if (UITextbox.Text.Length >= 10)
            {
                if (UIVeranstaltung.Text.Length > 0)
                {
                    int IID = DbPostgres.Instance.AktionsIDvonName(UIVeranstaltung.Text);
                    if (DbPostgres.Instance.CheckInsert(new Teilnehmer_teil(UITextbox.Text, IID, DateTime.Now)))
                    {
                        DbPostgres.Instance.InsertTeilnehmer(new Teilnehmer_teil(UITextbox.Text, IID, DateTime.Now));
                        UITextbox.Text = "";
                        UpdateDataGrid();
                    }
                    else
                    {
                        UITextbox.Text = "";
                        MessageBox.Show("Teilnehmer war bereits hier", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Keine Veranstaltung ausgewählt", "Rfid SCANNER", MessageBoxButton.OK, MessageBoxImage.Error);
                    UITextbox.Text = "";
                }
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pruefung p1 = new Pruefung();
            Einstellungen e1 = new Einstellungen();
            e1.SetEinstellungen(Einstellungen);
            if (p1.ShowDialog() == true && e1.ShowDialog() == true)
            {
                Einstellungen = e1.ReturnEinstellungen();
                foreach (var item in Einstellungen)
                {
                    if (item == "DarkModeAn")
                    {
                        DarkModeAn();
                        Darkmode = true;
                    }
                    if (item == "DarkModeAus")
                    {
                        DarkModeOff();
                        Darkmode = false;
                    }
                }
                for (int i = 0; i < Einstellungen.Count; i++)
                {
                    if (Einstellungen[i] == "InfoModusAn")
                    {
                        Info i1 = new Info();
                        i1.SetInfomodus();
                        if (Darkmode)
                        {
                            i1.SetDarkmode();
                        }
                        i1.SetVeranstaltunsName(Einstellungen[i + 1]);
                        if (i1.ShowDialog() == true)
                        {
                        }
                    }
                }
            }
        }
        public void DarkModeAn()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            Loeschen.Background = Brushes.Gray;
            Loeschen.Foreground = Brushes.White;
            UIDatagrid.Foreground = Brushes.White;
            UIDatagrid.RowBackground = Brushes.Gray;
            logo.Source = new BitmapImage(new Uri("../../Images/Edvschule Logo Darkmode.png", UriKind.Relative));
        }
        public void DarkModeOff()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(96, 147, 172));
            Loeschen.Background = new SolidColorBrush(Color.FromRgb(255, 105, 97));
            Loeschen.Foreground = Brushes.Black;
            UIDatagrid.Foreground = Brushes.Black;
            UIDatagrid.RowBackground = Brushes.White;
            logo.Source = new BitmapImage(new Uri("../../Images/Edvschule Logo.png", UriKind.Relative));
        }

        private void UIDatagrid_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
