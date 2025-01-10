using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Veranstaltung.xaml
    /// </summary>
    public partial class Veranstaltung : Window
    {
        bool Darkmode;
        List<Veranstaltung_ver> Veranstaltungen = new List<Veranstaltung_ver>();
        List<Aktion_akt> Aktionen = new List<Aktion_akt>();
        List<Standort_st> Standort = new List<Standort_st>();
        List<Ver_St_Akt_ZwischenTabelle> DatagridEinträge = new List<Ver_St_Akt_ZwischenTabelle>();
        public Veranstaltung()
        {
            InitializeComponent();
            Fill_Site();
        }

        public void Fill_Site()
        {
            UIDatagrid.Items.Clear();
            Veranstalltung.Items.Clear();
            Kurs.Items.Clear();
            Standname.Items.Clear();
            Veranstaltungen = DbPostgres.Instance.GetVeranstaltungen();
            Aktionen = DbPostgres.Instance.GetAktion();
            Standort = DbPostgres.Instance.GetStandort();
            string st_bezeichnung = "";
            string ver_bezeichnung = "";
            Veranstalltung.Items.Add("Alle");
            foreach (var item in Veranstaltungen)
            {
                Veranstalltung.Items.Add(new ComboBoxItem { Content = item.SBezeichnung, Tag = item.IiD });
            }
            DatagridEinträge = new List<Ver_St_Akt_ZwischenTabelle>();
            foreach (Aktion_akt item in Aktionen)
            {
                foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung_ver Veranstaltung in Veranstaltungen
                                              where item.Ver_IiD == Veranstaltung.IiD
                                              select Veranstaltung)
                {
                    ver_bezeichnung = Veranstaltung.SBezeichnung;
                }

                foreach (Standort_st Standorte in from Standort_st Standorte in Standort
                                          where item.St_IiD == Standorte.IiD
                                          select Standorte)
                {
                    st_bezeichnung = Standorte.SBezeichnung;
                }
                long CountBesucher = DbPostgres.Instance.CountBesucher(item.SBezeichnung);
                Ver_St_Akt_ZwischenTabelle DatagridEintrag = new Ver_St_Akt_ZwischenTabelle(ver_bezeichnung, st_bezeichnung, item.SBezeichnung, CountBesucher);
                DatagridEinträge.Add(DatagridEintrag);
                UIDatagrid.Items.Add(DatagridEintrag);
            }
        }
        public void Datagridfill()
        {
            string st_bezeichnung = "";
            string ver_bezeichnung = "";
            if (UIDatagrid != null)
            {
                UIDatagrid.Items.Clear();
            }

            foreach (Aktion_akt item in Aktionen)
            {
                foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung_ver Veranstaltung in Veranstaltungen
                                                            where item.Ver_IiD == Veranstaltung.IiD
                                                            select Veranstaltung)
                {
                    ver_bezeichnung = Veranstaltung.SBezeichnung;
                }

                foreach (Standort_st Standorte in from Standorte in Standort
                                                  where item.St_IiD == Standorte.IiD
                                                  select Standorte)
                {
                    st_bezeichnung = Standorte.SBezeichnung;
                }

                if (Veranstalltung.SelectedItem != null && Standname.SelectedItem != null && Kurs.SelectedItem != null)
                {
                    if (int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD && int.Parse((Standname.SelectedItem as ComboBoxItem)?.Tag as string) == item.St_IiD && item.SBezeichnung == (Kurs.SelectedItem as string))
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                    }
                }
                else
                    if (Veranstalltung.SelectedItem != null && Standname.SelectedItem != null && Kurs.SelectedItem == null)
                {
                    if (int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD && int.Parse((Standname.SelectedItem as ComboBoxItem)?.Tag as string) == item.St_IiD)
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                    }
                }
                else
                    if (Veranstalltung.SelectedItem != null && Standname.SelectedItem == null && Kurs.SelectedItem == null)
                {
                    if (int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD)
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                    }
                }
                else
                {
                    UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                }
            }
        }
        public void SetDarkMode()
        {
            Darkmode = true;
            this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            UIDatagrid.Foreground = Brushes.White;
            UIDatagrid.RowBackground = Brushes.Gray;
            Abbrechen.Background = Brushes.Gray;
            Abbrechen.Foreground = Brushes.White;
            Uebernehmen.Background = Brushes.Gray;
            Uebernehmen.Foreground = Brushes.White;
            Veranstalltung.Foreground = Brushes.Gray;
            Standname.Foreground = Brushes.Gray;
            Kurs.Foreground = Brushes.Gray;
        }
        private void UIDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UIDatagrid.SelectedItem != null)
            {
                Ver_St_Akt_ZwischenTabelle DatagridItems = UIDatagrid.SelectedItem as Ver_St_Akt_ZwischenTabelle;
                Veranstalltung.SelectedItem = DatagridItems.VBezeichnung;
                Standname.SelectedItem = DatagridItems.SBezeichnung;
                Kurs.SelectedItem = DatagridItems.ABezeichnung;
            }
        }

        private void Veranstalltung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Veranstalltung.SelectedItem != null)
            {
                if (Veranstalltung.SelectedItem.ToString() != "Alle")
                {
                    Standname.Items.Clear();
                    Kurs.Items.Clear();
                    foreach (Standort_st item in DbPostgres.Instance.Standortliste(DbPostgres.Instance.StandortIDS(DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem.ToString()))))
                    {
                        Standname.Items.Add(new ComboBoxItem { Content = item.SBezeichnung, Tag = item.IiD });
                    }

                    string st_bezeichnung = "";
                    string ver_bezeichnung = "";
                    UIDatagrid.Items.Clear();
                    foreach (Aktion_akt item in Aktionen)
                    {
                        foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung_ver Veranstaltung in Veranstaltungen
                                                      where item.Ver_IiD == Veranstaltung.IiD
                                                      select Veranstaltung)
                        {
                            ver_bezeichnung = Veranstaltung.SBezeichnung;
                        }

                        foreach (Standort_st Standorte in from Standort_st Standorte in Standort
                                                  where item.St_IiD == Standorte.IiD
                                                  select Standorte)
                        {
                            st_bezeichnung = Standorte.SBezeichnung;
                        }

                        if (int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD)
                        {
                            UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                        }
                    }
                }
                else
                {
                    Fill_Site();
                }
            }
        }

        private void Standname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Kurs.Items.Clear();
            if (Standname.SelectedItem != null)
            {
                List<Aktion_akt> Aktionen = DbPostgres.Instance.GetAktion(Standname.SelectedItem.ToString());
                foreach (Aktion_akt item in Aktionen)
                {
                    Kurs.Items.Add(new ComboBoxItem { Content = item.SBezeichnung, Tag = item.IiD });
                }
                string st_bezeichnung = "";
                string ver_bezeichnung = "";
                UIDatagrid.Items.Clear();
                foreach (Aktion_akt item in Aktionen)
                {
                    foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung_ver Veranstaltung in Veranstaltungen
                                                  where item.Ver_IiD == Veranstaltung.IiD
                                                  select Veranstaltung)
                    {
                        ver_bezeichnung = Veranstaltung.SBezeichnung;
                    }

                    foreach (Standort_st Standorte in from Standort_st Standorte in Standort
                                              where item.St_IiD == Standorte.IiD
                                              select Standorte)
                    {
                        st_bezeichnung = Standorte.SBezeichnung;
                    }

                    if (int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD && int.Parse((Standname.SelectedItem as ComboBoxItem)?.Tag as string) == item.St_IiD)
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                    }
                }
            }
        }

        private void Kurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kurs.SelectedItem != null)
            {
                string st_bezeichnung = "";
                string ver_bezeichnung = "";
                UIDatagrid.Items.Clear();
                foreach (Aktion_akt item in Aktionen)
                {
                    foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung_ver Veranstaltung in Veranstaltungen
                                                  where item.Ver_IiD == Veranstaltung.IiD
                                                  select Veranstaltung)
                    {
                        ver_bezeichnung = Veranstaltung.SBezeichnung;
                    }

                    foreach (Standort_st Standorte in from Standort_st Standorte in Standort
                                              where item.St_IiD == Standorte.IiD
                                              select Standorte)
                    {
                        st_bezeichnung = Standorte.SBezeichnung;
                    }

                    if (Standname.SelectedItem != null && int.Parse((Veranstalltung.SelectedItem as ComboBoxItem)?.Tag as string) == item.Ver_IiD && int.Parse((Standname.SelectedItem as ComboBoxItem)?.Tag as string) == item.St_IiD && item.SBezeichnung == (Kurs.SelectedItem as string))
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung, Besucher = 999 });
                    }
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
            }
        }

        private void UIDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UIDatagrid.SelectedItem != null)
            {
                Auswertung a1 = new Auswertung();
                Ver_St_Akt_ZwischenTabelle Aktuelle = UIDatagrid.SelectedItem as Ver_St_Akt_ZwischenTabelle;
                a1.SetChart(Aktuelle.ABezeichnung);
                if (Darkmode)
                {
                    a1.SetDarkmode();
                }
                a1.ShowDialog();
            }
        }
        private void Uebernehmen_Click(object sender, RoutedEventArgs e)
        {
            if (Veranstalltung.Text.Trim()?.Length == 0 || Standname.Text.Trim()?.Length == 0 || Kurs.Text.Trim()?.Length == 0)
            {
                MessageBox.Show("Sie müssen eine Aktion auswählen!", "RFID Scanner", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                DialogResult = true;
            }
        }
        public Ver_St_Akt_ZwischenTabelle SetVeranstaltung() => new Ver_St_Akt_ZwischenTabelle(Veranstalltung.Text + "", Standname.Text + "", Kurs.Text + "");

        private void Abbrechen_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UebersichtAnzeigen_click(object sender, MouseButtonEventArgs e)
        {
            if (Darkmode)
            {
                new AddVeranstaltung().SetDarkMode();
            }
            new AddVeranstaltung().ShowDialog();

            Fill_Site();
        }

        private new void MouseLeave(object sender, MouseEventArgs e) => Verwalten.FontWeight = FontWeights.Light;

        private new void MouseEnter(object sender, MouseEventArgs e) => Verwalten.FontWeight = FontWeights.Bold;


    }
}
