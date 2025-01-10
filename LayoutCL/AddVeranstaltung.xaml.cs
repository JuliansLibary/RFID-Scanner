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
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für AddVeranaltung.xaml
    /// </summary>
    public partial class AddVeranstaltung : Window
    {
        private List<Veranstaltung_ver> Veranstaltungen = new List<Veranstaltung_ver>();
        private List<Aktion_akt> Aktionen = new List<Aktion_akt>();
        private List<Standort_st> Standort = new List<Standort_st>();
        private string st_bezeichnung = "";
        private string ver_bezeichnung = "";
        private bool InsertKurs = false;
        private bool UpdateKurs = false;
        private bool InsertStandort = false;
        private bool UpdateStandort = false;
        private bool InsertVeranstaltung = false;
        private bool UpdateVeranstaltung;

        private double YInPro;
        private double XInPro;
        public double YInPro_Old;
        public double XInPro_Old;
        Rectangle r1 = new Rectangle();
        Rectangle Update = new Rectangle();
        public AddVeranstaltung()
        {
            InitializeComponent();
            Fill_Site();
            FIll_site_st();
            Fill_site_ve();
            Untergeschoss.IsChecked = true;
            ImageBrush bg = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_EG.png", UriKind.Relative))
            };
            ImageGrid.Background = bg;
        }

        private void InsertNew_click(object sender, RoutedEventArgs e)
        {
            Veranstalltung.Items.Clear();
            Standname.Items.Clear();
            Kurs.Items.Clear();
            UIDatagrid.Items.Clear();

            // Alle Daten werden aus dem UI entfernt damit man einen Neuen Ds anlegen kann

            //Alle Daten werden ohne Filter von allen Tabellen hinzugefügt damit der Benutzer aus allen Daten auswählen kann nicht nur aus 
            // denen der AKtionstabelle
            foreach (var item in Aktionen)
            {
                Kurs.Items.Add(item.SBezeichnung);
            }
            foreach (var item in Standort)
            {
                Standname.Items.Add(item.SBezeichnung);
            }
            foreach (Veranstaltung_ver item in Veranstaltungen)
            {
                Veranstalltung.Items.Add(item.SBezeichnung);
            }

            foreach (Aktion_akt item in Aktionen)
            {
                foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung in Veranstaltungen
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

                UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung });
            }
            // falls vorher ein Update versuch war wird dieser auch aufgräumt
            UpdateKurs = false;
            Standname_new.Items.Clear();
            Veranstaltung_textbox.Text = Veranstalltung.SelectedItem as string;
            Standort_textbox.Text = Standname.SelectedItem as string;
            Kurs_textbox.Text = Kurs.SelectedItem as string;
            Kurs_textbox.IsReadOnly = false;
            Kurs_textbox.Focus();
            InsertKurs = true;
        }

        private void DSandern_click(object sender, RoutedEventArgs e)
        {
            if (Standname.SelectedItem != null && Veranstalltung.SelectedItem != null && Kurs.SelectedItem != null)
            {
                if (DbPostgres.Instance.CheckIfGoodDs(new Aktion_akt(Kurs.SelectedItem as string, DbPostgres.Instance.GetStandortZuVeranstaltung(Standname.SelectedItem as string), DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string))))
                {
                    //prüfung ob gerade ein Insert geplant war, wenn ja jetzt Update und insert wird zurückgestellt
                    // prüfung ob ein Item im Datagrid ausgewählt wurde
                    //Seite wird resetet
                    InsertKurs = false;
                    UpdateKurs = true;
                    foreach (var item in Standort)
                    {
                        Standname_new.Items.Add(item.SBezeichnung);
                    }
                    Standname_new.SelectedItem = Standname.SelectedItem;
                    Kurs_textbox.IsReadOnly = false;
                    Kurs_textbox.Focus();
                    Veranstaltung_textbox.Text = Veranstalltung.SelectedItem as string;
                    Standort_textbox.Text = Standname.SelectedItem as string;
                    Kurs_textbox.Text = Kurs.SelectedItem as string;
                }
                else
                {
                    MessageBox.Show("Ungültiger Datensatz!", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nix ausgewählt", "Rfid_Scanner", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }
        public void FIll_site_st()
        {
            // Fill site methode für das Datagrid auf der Standort seite
            UIDatagrid_st.Items.Clear();
            Standort_st.Items.Clear();
            Standort = DbPostgres.Instance.GetStandort();
            foreach (var item in Standort)
            {
                Standort_st.Items.Add(item.SBezeichnung);
                UIDatagrid_st.Items.Add(new Standort_st() { SBezeichnung = item.SBezeichnung });
            }
        }
        public void Fill_Site()
        {
            // fill site methode für das Datagrid auf der aktions seite
            UIDatagrid.Items.Clear();
            Veranstalltung.Items.Clear();
            Standname.Items.Clear();
            Kurs.Items.Clear();
            Standname_new.Items.Clear();
            Veranstaltung_textbox.Text = "";
            Kurs_textbox.Text = "";
            Standort_textbox.Text = "";
            Veranstaltungen = DbPostgres.Instance.GetVeranstaltungen();
            Aktionen = DbPostgres.Instance.GetAktion();
            Standort = DbPostgres.Instance.GetStandort();
            st_bezeichnung = "";
            ver_bezeichnung = "";
            Veranstalltung.Items.Add("Alle");
            //alles wurde zurückgesetzt falls vorher was drinnen stand
            foreach (var item in Veranstaltungen)
            {
                Veranstalltung.Items.Add(item.SBezeichnung);
            }
            Veranstalltung.SelectedItem = null;

            foreach (var item in Aktionen)
            {
                foreach (var Veranstaltung in Veranstaltungen)
                {
                    if (item.Ver_IiD == Veranstaltung.IiD)
                    {
                        ver_bezeichnung = Veranstaltung.SBezeichnung;
                    }
                }
                foreach (var Standorte in Standort)
                {
                    if (item.St_IiD == Standorte.IiD)
                    {
                        st_bezeichnung = Standorte.SBezeichnung;
                    }
                }
                UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung });
                //^Items werden aus der db gelesen und ins datagrid geschrieben
            }
        }

        private void UIDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UIDatagrid.SelectedItem != null)
            {
                //Hier werden alle daten in die Comboboxen geschrieben damit die Bedienung flüssiger ist
                Ver_St_Akt_ZwischenTabelle DatagridItems = UIDatagrid.SelectedItem as Ver_St_Akt_ZwischenTabelle;
                Veranstalltung.SelectedItem = DatagridItems.VBezeichnung;
                Standname.SelectedItem = DatagridItems.SBezeichnung;
                Kurs.SelectedItem = DatagridItems.ABezeichnung;
            }
        }

        private void Ds_loeschen(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sind Sie sicher das Sie den Datensatz löschen wollen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
            if (Veranstalltung.SelectedItem != null && Standname.SelectedItem != null && Kurs.SelectedItem != null)//wird nur aufgeführt wenn ein Datensatz im Grid ausgewählt wurde
            {
                if (DbPostgres.Instance.CheckIfGoodDs(new Aktion_akt(Kurs.SelectedItem as string, DbPostgres.Instance.GetStandortZuVeranstaltung(Standname.SelectedItem as string), DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string))))
                {
                    int v_id = DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string);
                    int s_id = DbPostgres.Instance.GetStandortZuVeranstaltung(Standname.SelectedItem as string);
                    //^hier werden alle nötigen Daten für die eindeutige Identifikation des DS geholt um sicher zu sein das der richtige gelöscht wird
                    if (DbPostgres.Instance.DeleteKurs(new Aktion_akt(Kurs.SelectedItem as string, s_id, v_id)))//um die Methoder "DeleteKurs" ist ein try catch der einen Bool zurück gibt, somit kann man eine einfache Fehlerprüfung durchführen
                    {
                        InsertKurs = false;
                        UpdateKurs = false;
                        Kurs_textbox.Text = "";
                        Standort_textbox.Text = "";
                        Veranstaltung_textbox.Text = "";
                        Fill_Site();
                        MessageBox.Show("Aktion gelöscht!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim löschen!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Ungültiger Datensatz", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nix ausgewählt?", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            }
        }

        private void Uebernehmen_click(object sender, RoutedEventArgs e)
        {
            //Es wird geprüft welches Event gerade ausgewählt ist und für dieses aus
            if (InsertKurs && Kurs_textbox.Text.Length > 0 && Veranstalltung.SelectedItem != null && Standname.SelectedItem != null)
            {
                int v_id = DbPostgres.Instance.GetVeranstaltungsID(Veranstaltung_textbox.Text);
                int s_id = DbPostgres.Instance.GetStandortZuVeranstaltung(Standort_textbox.Text);
                if (DbPostgres.Instance.InsertKurs(new Aktion_akt(Kurs_textbox.Text, s_id, v_id)))
                {
                    MessageBox.Show("Kurs Erfolgreich hinzugefügt!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    InsertKurs = false;
                    Kurs_textbox.Text = "";
                    Standort_textbox.Text = "";
                    Veranstaltung_textbox.Text = "";
                    Fill_Site();
                }
                else
                {
                    MessageBox.Show("Fehler beim hinzufügen des DS", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    Kurs_textbox.Text = "";
                    Kurs_textbox.Focus();
                }
            }
            else if (InsertKurs)
            {
                MessageBox.Show("Noch nicht alle Daten eingetragen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (UpdateKurs&& Kurs_textbox.Text.Length > 0)
            {
                int newv_id = DbPostgres.Instance.GetVeranstaltungsID(Veranstaltung_textbox.Text);
                int news_id = DbPostgres.Instance.GetStandortZuVeranstaltung(Standort_textbox.Text);
                int oldv_id = DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string);
                int olds_id = DbPostgres.Instance.GetStandortZuVeranstaltung(Standname.SelectedItem as string);
                if (DbPostgres.Instance.UpdateAktion(new Aktion_akt(Kurs.SelectedItem as string, olds_id, oldv_id), new Aktion_akt(Kurs_textbox.Text.Trim(), news_id, newv_id)))
                {
                    MessageBox.Show("Aktion erfolgreich geändert!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    Veranstaltung_textbox.Text = "";
                    Standort_textbox.Text = "";
                    Kurs_textbox.Text = "";
                    Fill_Site();
                    UpdateKurs = false;
                }
                else
                {
                    MessageBox.Show("Fehler beim Update", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (UpdateKurs)
            {
                MessageBox.Show("Noch nicht alle Daten eingetragen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Keine Event ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Standname_SelectionChanged_new(object sender, SelectionChangedEventArgs e) => Standort_textbox.Text = Standname_new.SelectedItem as string;

        private void UIDatagrid_SelectionChanged_st(object sender, SelectionChangedEventArgs e)
        {
            if (UIDatagrid_st.SelectedItem != null)
            {
                Standort_st st = UIDatagrid_st.SelectedItem as Standort_st;
                Standort_st.SelectedItem = st.SBezeichnung;
            }
        }

        private void Standort_st_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Standort_st.SelectedItem != null)
            {
                if (ImageGrid.Children.Contains(Update))
                {
                    ImageGrid.Children.Remove(Update);
                }
                if (ImageGrid.Children.Contains(r1))
                {
                    ImageGrid.Children.Remove(r1);
                }
                Veranstaltung_textbox_st.Text = Standort_st.SelectedItem as string;
                Standort_st Standorte = DbPostgres.Instance.GetStandortmitKoords(Veranstaltung_textbox_st.Text);

                if (Standorte.UnterOber == 'u' || (Standorte.UnterOber == 'U' && Obergeschoss.IsChecked == true))
                {
                    Untergeschoss.IsChecked = true;
                }
                else if (Standorte.UnterOber == 'o' || (Standorte.UnterOber == 'O' && Untergeschoss.IsChecked == true))
                {
                    Obergeschoss.IsChecked = true;
                }

                ImageBrush b1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("../../Images/map-pin.png", UriKind.Relative))
                };
                r1 = new Rectangle()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = Standorte.UnterOber,
                    Height = 30,
                    Width = 30,
                    Fill = b1,
                    Margin = new Thickness(Standorte.X_Kooridinate / 100 * ImageGrid.ActualWidth - 15, Standorte.Y_Kooridinate / 100 * ImageGrid.ActualHeight - 30,0,0)
                };

                XInPro = Standorte.X_Kooridinate;
                YInPro = Standorte.Y_Kooridinate;
                XInPro_Old = Standorte.X_Kooridinate;
                YInPro_Old = Standorte.Y_Kooridinate;
                if (Standorte.UnterOber == 'U')
                {
                    r1.ToolTip = Standorte.SBezeichnung + " Untergeschoss";
                }
                else
                {
                    r1.ToolTip = Standorte.SBezeichnung + " Obergeschoss";
                }
                ImageGrid.Children.Add(r1);
            }
        }

        private void InsertNew_click_st(object sender, RoutedEventArgs e)
        {
            if (ImageGrid.Children.Contains(r1))
            {
                ImageGrid.Children.Remove(r1);
            }
            if (ImageGrid.Children.Contains(Update))
            {
                ImageGrid.Children.Remove(Update);
            }
            Veranstaltung_textbox_st.Focus();
            Veranstaltung_textbox_st.Text = "";
            Standort_st.SelectedItem = null;
            Veranstaltung_textbox_st.IsReadOnly = false;
            InsertStandort = true;
            UpdateStandort = false;
        }

        private void Uebernehmen_click_st(object sender, RoutedEventArgs e)
        {
            if (UpdateStandort && Veranstaltung_textbox_st.Text.Length > 0)
            {
                if (Untergeschoss.IsChecked == true)
                {
                    if (DbPostgres.Instance.UpdateStandort(Standort_st.SelectedItem as string, Veranstaltung_textbox_st.Text, XInPro, YInPro,  'U'))
                    {
                        MessageBox.Show("Standort Erfolgreich geändert!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                        FIll_site_st();
                        Fill_Site();
                        XInPro = 0;
                        YInPro = 0;
                        XInPro_Old = 0;
                        YInPro_Old = 0;
                        UpdateStandort = false;
                        if (ImageGrid.Children.Contains(Update))
                        {
                            ImageGrid.Children.Remove(Update);
                        }
                        if (ImageGrid.Children.Contains(r1))
                        {
                            ImageGrid.Children.Remove(r1);
                        }
                    }
                }
                else if (Obergeschoss.IsChecked == true)
                {
                    if (DbPostgres.Instance.UpdateStandort(Standort_st.SelectedItem as string, Veranstaltung_textbox_st.Text, XInPro, YInPro, 'O'))
                    {
                        MessageBox.Show("Standort Erfolgreich geändert!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                        FIll_site_st();
                        Fill_Site();
                        UpdateStandort = false;
                        XInPro = 0;
                        YInPro = 0;
                        XInPro_Old = 0;
                        YInPro_Old = 0;
                        if (ImageGrid.Children.Contains(Update))
                        {
                            ImageGrid.Children.Remove(Update);
                        }
                        if (ImageGrid.Children.Contains(r1))
                        {
                            ImageGrid.Children.Remove(r1);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Fehler beim ändern", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (UpdateStandort)
            {
                MessageBox.Show("Kein Standort ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (InsertStandort && Veranstaltung_textbox_st.Text.Length > 0)
            {
                if (Untergeschoss.IsChecked == true)
                {
                    if (XInPro > 0 && YInPro > 0 && DbPostgres.Instance.InsertStandort(Veranstaltung_textbox_st.Text, XInPro, YInPro, 'U'))
                    {
                        MessageBox.Show("Standort Erfolgreich hinzugefügt!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                        FIll_site_st();
                        Fill_Site();
                        XInPro = 0;
                        YInPro = 0;
                        XInPro_Old = 0;
                        YInPro_Old = 0;
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim hinzugefügen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (Obergeschoss.IsChecked == true)
                {
                    if (DbPostgres.Instance.InsertStandort(Veranstaltung_textbox_st.Text, XInPro, YInPro, 'O'))
                    {
                        MessageBox.Show("Standort Erfolgreich hinzugefügt!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                        FIll_site_st();
                        Fill_Site();
                        XInPro = 0;
                        YInPro = 0;
                        XInPro_Old = 0;
                        YInPro_Old = 0;
                    }
                }
            }
            else if (InsertStandort)
            {
                MessageBox.Show("Keine Eingabe gemacht", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Kein Event ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DSandern_click_st(object sender, RoutedEventArgs e)
        {
            if (Standort_st.SelectedItem != null)
            {
                InsertStandort = false;
                UpdateStandort = true;
                Veranstaltung_textbox_st.Focus();
                Veranstaltung_textbox_st.IsReadOnly = false;
            }
            else
            {
                MessageBox.Show("Kein Event ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ds_loeschen_st(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sind Sie sicher das Sie den Datensatz löschen wollen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Veranstaltung_textbox_st.IsReadOnly = true;
                if (Standort_st.SelectedItem != null)
                {
                    if (DbPostgres.Instance.DeleteStandort(Standort_st.SelectedItem as string))
                    {
                        //Test
                        FIll_site_st();
                        Veranstaltung_textbox_ve.Clear();
                        ImageGrid.Children.Remove(r1);
                        MessageBox.Show("Standort Erfolgreich gelöscht!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Löschen.\nMöglicherweiße bestehen noch beziehungen\nzu Aktionen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Veranstalltung_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (Veranstalltung.SelectedItem != null && !InsertKurs)
            {
                Kurs_textbox.Text = "";
                Kurs_textbox.IsReadOnly = true;
                UpdateKurs = false;
                if (Veranstalltung.SelectedItem as string == "Alle")
                {
                    Fill_Site();
                }
                else
                {
                    UIDatagrid.Items.Clear();
                    Standname.Items.Clear();
                    Kurs.Items.Clear();
                    Veranstaltung_textbox.Text = Veranstalltung.SelectedItem as string;
                    Standort_textbox.Text = "";
                    foreach (Standort_st item in DbPostgres.Instance.Standortliste(DbPostgres.Instance.StandortIDS(DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string))))
                    {
                        Standname.Items.Add(item.SBezeichnung);
                    }

                    foreach (Aktion_akt item in Aktionen)
                    {
                        foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung in Veranstaltungen
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

                        if (item.Ver_IiD == DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string))
                        {
                            UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung });
                        }
                    }
                }
            }
            else if (InsertKurs)
            {
                Veranstaltung_textbox.Text = Veranstalltung.SelectedItem as string;
            }
        }

        private void Standname_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (Standname.SelectedItem != null && !InsertKurs)
            {
                Kurs_textbox.Text = "";
                Kurs_textbox.IsReadOnly = true;
                UpdateKurs = false;
                UIDatagrid.Items.Clear();
                Standort_textbox.Text = Standname.SelectedItem as string;

                Kurs.Items.Clear();
                if (Standname.SelectedItem != null)
                {
                    string Sname = Standname.SelectedItem as string;
                    List<Aktion_akt> Aktionen = DbPostgres.Instance.GetAktion(Sname);
                    foreach (var item in Aktionen)
                    {
                        Kurs.Items.Add(item.SBezeichnung);
                    }
                    foreach (Aktion_akt item in Aktionen)
                    {
                        foreach (Veranstaltung_ver Veranstaltung in from Veranstaltung in Veranstaltungen
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

                        if (Standname.SelectedItem != null && DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string) == item.Ver_IiD && DbPostgres.Instance.GetAktionZuStandort(Standname.SelectedItem as string) == item.St_IiD)
                        {
                            UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung });
                        }
                    }
                }
            }
            else if (InsertKurs)
            {
                Standort_textbox.Text = Standname.SelectedItem as string;
            }
        }

        private void Kurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kurs.SelectedItem != null && !InsertKurs)
            {
                Kurs_textbox.Text = "";
                Kurs_textbox.IsReadOnly = true;
                UpdateKurs = false;
                UIDatagrid.Items.Clear();
                string st_bezeichnung = "";
                string ver_bezeichnung = "";
                Kurs_textbox.Text = Kurs.SelectedItem as string;
                UIDatagrid.Items.Clear();
                foreach (var item in Aktionen)
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

                    if (Standname.SelectedItem != null && DbPostgres.Instance.GetVeranstaltungsID(Veranstalltung.SelectedItem as string) == item.Ver_IiD && DbPostgres.Instance.GetAktionZuStandort(Standname.SelectedItem as string) == item.St_IiD && item.SBezeichnung == Kurs.SelectedItem as string)
                    {
                        UIDatagrid.Items.Add(new Ver_St_Akt_ZwischenTabelle() { VBezeichnung = ver_bezeichnung, SBezeichnung = st_bezeichnung, ABezeichnung = item.SBezeichnung });
                    }
                }
            }
            else if (InsertKurs)
            {
                Kurs_textbox.Text = Kurs.SelectedItem as string;
            }
        }

        private void UIDatagrid_SelectionChanged_ve(object sender, SelectionChangedEventArgs e)
        {
            if (UIDatagrid_ve.SelectedItem != null)
            {
                Veranstaltung_ver Veranstaltung = UIDatagrid_ve.SelectedItem as Veranstaltung_ver;
                Standort_ve.SelectedItem = Veranstaltung.SBezeichnung;
                Veranstaltung_textbox_ve.Text = Veranstaltung.SBezeichnung;
            }
        }
        public void Fill_site_ve()
        {
            UIDatagrid_ve.Items.Clear();
            Standort_ve.Items.Clear();
            Veranstaltungen = DbPostgres.Instance.GetVeranstaltungen();
            foreach (Veranstaltung_ver item in Veranstaltungen)
            {
                Standort_ve.Items.Add(item.SBezeichnung);
                UIDatagrid_ve.Items.Add(new Veranstaltung_ver() { SBezeichnung = item.SBezeichnung });
            }
        }
        private void Uebernehmen_click_ve(object sender, RoutedEventArgs e)
        {
            if (UpdateVeranstaltung && Veranstaltung_textbox_ve.Text.Length > 0)
            {
                if (DbPostgres.Instance.UpdateVeranstaltung(Standort_ve.SelectedItem as string, Veranstaltung_textbox_ve.Text))
                {
                    MessageBox.Show("Standort Erfolgreich geändert!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    FIll_site_st();
                    Fill_Site();
                    Fill_site_ve();
                    UpdateVeranstaltung = false;
                }
                else
                {
                    MessageBox.Show("Fehler beim ämdern", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (UpdateVeranstaltung)
            {
                MessageBox.Show("Kein Veranstaltung ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (InsertVeranstaltung && Veranstaltung_textbox_ve.Text.Length > 0)
            {
                if (DbPostgres.Instance.InsertVeranstaltung(Veranstaltung_textbox_ve.Text))
                {
                   MessageBox.Show("Veranstaltung Erfolgreich hinzugefügt!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    FIll_site_st();
                    Fill_Site();
                    Fill_site_ve();
                    InsertVeranstaltung = false;
                }
                else
                {
                    MessageBox.Show("Fehler beim hinzugefügen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (InsertVeranstaltung)
            {
                MessageBox.Show("Keine Eingabe gemacht", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Kein Event ausgewählt", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertNew_click_ve(object sender, RoutedEventArgs e)
        {
            Veranstaltung_textbox_ve.Focus();
            Veranstaltung_textbox_ve.Text = "";
            Standort_ve.SelectedItem = null;
            Veranstaltung_textbox_ve.IsReadOnly = false;
            InsertVeranstaltung = true;
            UpdateVeranstaltung = false;
        }

        private void DSandern_click_ve(object sender, RoutedEventArgs e)
        {
            if (Standort_ve.SelectedItem != null)
            {
                InsertVeranstaltung = false;
                UpdateVeranstaltung = true;
                Veranstaltung_textbox_ve.Focus();
                Veranstaltung_textbox_ve.IsReadOnly = false;
            }
            else
            {
                MessageBox.Show("Keine Veranstaltung ausgewählt");
            }
        }

        private void Ds_loeschen_ve(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sind Sie sicher das Sie den Datensatz löschen wollen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Veranstaltung_textbox_ve.IsReadOnly = true;
                if (Standort_ve.SelectedItem != null)
                {
                    if (DbPostgres.Instance.DeleteVeranstaltung(Standort_ve.SelectedItem as string))
                    {
                        Fill_site_ve();
                        MessageBox.Show("Standort Erfolgreich gelöscht!", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Löschen.\nMöglicherweiße bestehen noch beziehungen\nzu Aktionen", "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Veranstaltung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Standort_ve.SelectedItem != null)
            {
                Veranstaltung_textbox_ve.Text = Standort_ve.SelectedItem as string;
            }
        }

        private void Untergeschoss_Checked(object sender, RoutedEventArgs e)
        {
            ImageGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_EG.png", UriKind.Relative))
            };

            if (UpdateStandort && (char)r1.Tag == 'U')
            {
                ImageGrid.Children.Add(r1);
            }
            else if (ImageGrid.Children.Contains(r1))
            {
                ImageGrid.Children.Remove(r1);
            }
        }

        private void Obergeschoss_Checked(object sender, RoutedEventArgs e)
        {
            ImageGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_OG.png", UriKind.Relative))
            };

            if (UpdateStandort && (char)r1.Tag == 'O')
            {
                ImageGrid.Children.Add(r1);
            }
            else if (ImageGrid.Children.Contains(r1))
            {
                ImageGrid.Children.Remove(r1);
            }
        }
        private void ImageGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UpdateStandort)
            {
                Point p = e.GetPosition((IInputElement)sender);
                XInPro = p.X / ImageGrid.ActualWidth * 100;
                YInPro = p.Y / ImageGrid.ActualHeight * 100;

                if (ImageGrid.Children.Contains(Update))
                {
                    ImageGrid.Children.Remove(Update);
                }
                ImageBrush b1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("../../Images/map-pin.png", UriKind.Relative))
                };
                Update = new Rectangle()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ToolTip = "Dies ist der neue Standort",
                    Height = 30,
                    Width = 30,
                    Fill = b1,
                    Margin = new Thickness(p.X - 15, p.Y - 30, r1.Margin.Right, r1.Margin.Bottom)
                };
                ImageGrid.Children.Add(Update);
            }
            else
            {
                if (ImageGrid.Children.Contains(r1))
                {
                    ImageGrid.Children.Remove(r1);
                }
                Point p = e.GetPosition((IInputElement)sender);
                r1 = new Rectangle();
                XInPro = p.X / ImageGrid.ActualWidth * 100;
                YInPro = p.Y / ImageGrid.ActualHeight * 100;
                XInPro_Old = p.X / ImageGrid.ActualWidth * 100;
                YInPro_Old = p.Y / ImageGrid.ActualHeight * 100;
                r1.Margin = new Thickness(p.X - 15, p.Y - 30, 0, 0);
                r1.VerticalAlignment = VerticalAlignment.Top;
                r1.HorizontalAlignment = HorizontalAlignment.Left;
                if (Untergeschoss.IsChecked == true)
                {
                    r1.Tag = "U";
                }
                else
                {
                    r1.Tag = "O";
                }
                r1.ToolTip = "Neuer Punkt";
                r1.Height = 30;
                r1.Width = 30;
                ImageBrush b1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("../../Images/map-pin.png", UriKind.Relative))
                };
                r1.Fill = b1;
                ImageGrid.Children.Add(r1);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ImageGrid.Children.Contains(r1))
            {
                r1.Margin = new Thickness(XInPro / 100 * ImageGrid.ActualWidth - 15, YInPro / 100 * ImageGrid.ActualHeight - 30, r1.Margin.Right, r1.Margin.Bottom);
            }
        }
        public void SetDarkMode()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            UIDatagrid_ve.RowBackground = Brushes.Gray;
            UIDatagrid_st.RowBackground = Brushes.Gray;
            UIDatagrid.RowBackground = Brushes.Gray;
            UIDatagrid_ve.Foreground = Brushes.White;
            UIDatagrid_st.Foreground = Brushes.White;
            UIDatagrid.Foreground = Brushes.White;
            VeranstaltungTabItem.Foreground = Brushes.Gray;
            Standort_ui.Foreground = Brushes.Gray;
            Aktions_ui.Foreground = Brushes.Gray;

            Standort_st.Foreground = Brushes.Gray;
            Standort_ve.Foreground = Brushes.Gray;
            Veranstalltung.Foreground = Brushes.Gray;
            Standname.Foreground = Brushes.Gray;
            Kurs.Foreground = Brushes.Gray;
            Standname_new.Foreground = Brushes.Gray;

            löschen_ve.Foreground = Brushes.White;
            löschen_ve.Background = Brushes.Gray;
            löschen.Foreground = Brushes.White;
            löschen.Background = Brushes.Gray;
            löschen_st.Background = Brushes.Gray;
            löschen_st.Foreground = Brushes.White;

            ChangeDS.Foreground = Brushes.White;
            ChangeDS.Background = Brushes.Gray;
            ChangeDS_ve.Background = Brushes.Gray;
            ChangeDS_ve.Foreground = Brushes.White;
            ChangeDS_st.Foreground = Brushes.White;
            ChangeDS_st.Background = Brushes.Gray;

            Uebernehmen.Foreground = Brushes.White;
            Uebernehmen.Background = Brushes.Gray;
            Uebernehmen_ve.Foreground = Brushes.White;
            Uebernehmen_ve.Background = Brushes.Gray;
            Uebernehmen_st.Background = Brushes.Gray;
            Uebernehmen_st.Foreground = Brushes.White;

            InsertNew_st.Foreground = Brushes.White;
            InsertNew_st.Background = Brushes.Gray;
            InsertNew_ve.Foreground = Brushes.White;
            InsertNew_ve.Background = Brushes.Gray;
            InsertNew.Foreground = Brushes.White;
            InsertNew.Background = Brushes.Gray;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
            }
        }
    }
}
