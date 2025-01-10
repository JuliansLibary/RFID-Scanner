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
using System.Windows.Threading;
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>

    public partial class Info : Window
    {
        DispatcherTimer dp1 = new DispatcherTimer();

        string LastSelectedItem = "";
        string LastSelectedItemStandorteUnbesucht = "";
        List<Rectangle> Staende = new List<Rectangle>();
        List<Standort_st> Aktionen = new List<Standort_st>();
        List<Aktion_akt> SortierteAktionen = new List<Aktion_akt>();
        List<Standort_st> UnbesuchteStandorte = new List<Standort_st>();
        bool IsInfoMode = false;
        int counter = 0;
        public Info()
        {
            InitializeComponent();
            dp1.Interval = TimeSpan.FromSeconds(1);
            Untergeschoss.IsChecked = true;
            KartenNummer.Focus();
            Grid g1 = new Grid();
            ImageBrush bg = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_EG.png", UriKind.Relative))
            };
            ImageGrid.Background = bg;
        }

        public void StopTimer()
        {
            dp1.Stop();
        }

        public void SetDarkmode()
        {
            Datagrid.Foreground = Brushes.White;
            UnbesuchteStandorteUIGRID.Foreground = Brushes.White;
            Datagrid.RowBackground = Brushes.Gray;
            UnbesuchteStandorteUIGRID.RowBackground = Brushes.Gray;
            this.Background = this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            logo.Source = new BitmapImage(new Uri("../../Images/Edvschule Logo Darkmode.png", UriKind.Relative));
        }
        public void SetVeranstaltunsName(string Veranstaltung)
        {
            Veranstaltung_UI.Content = Veranstaltung;
        }
        public void SetInfomodus()
        {
            IsInfoMode = true;
            Window.ResizeMode = ResizeMode.NoResize;
            Window.WindowStyle = WindowStyle.None;
            Window.WindowState = WindowState.Maximized;

        }
        public void GetandSetAllStandorte()
        {
            List<Aktion_akt> AlleAktionen = new List<Aktion_akt>();


            ImageBrush b1 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/map-pin.png", UriKind.Relative))
            };
            ImageBrush UnbesuchtPin = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/unbesucht-pin.png", UriKind.Relative))
            };
            Aktionen = DbPostgres.Instance.GetTeilnehmervonAktionMitKoordinaten(KartenNummer.Text, (string)Veranstaltung_UI.Content);

            AlleAktionen = DbPostgres.Instance.GetAktion();
            foreach (var item in AlleAktionen)
            {
                bool isin = false;
                foreach (var aktionen in Aktionen)
                {
                    if (aktionen.SBezeichnung == item.SBezeichnung)
                    {
                        isin = true;
                    }
                }
                if (isin == false)
                {
                    SortierteAktionen.Add(item);
                }
            }

            if (Aktionen.Count() == 0)
            {
                MessageBox.Show("Noch keinen Standort besucht! Beeil dich es ist schon " + DateTime.Now.ToString("HH:mm"), "Rfid Scanner", MessageBoxButton.OK, MessageBoxImage.Information);
                UnbesuchteStandorte = DbPostgres.Instance.AlleAktionenFürVeranstaltung((string)Veranstaltung_UI.Content);
            }
            else
            {
                UnbesuchteStandorte = DbPostgres.Instance.AlleStandorteZuAktion(SortierteAktionen, (string)Veranstaltung_UI.Content);
            }
            foreach (var item in Aktionen)
            {
                Datagrid.Items.Add(new Standort_st() { SBezeichnung = item.SBezeichnung, Zeitstempel = item.Zeitstempel, Bezeichnung = item.Bezeichnung });
                Rectangle Stand = new Rectangle()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ToolTip = item.SBezeichnung,
                    Tag = item.UnterOber,
                    Height = 40,
                    Width = 40,
                    Fill = b1,
                    Margin = new Thickness(item.X_Kooridinate / 100.00 * ImageGrid.ActualWidth - 20, item.Y_Kooridinate / 100.00 * ImageGrid.ActualHeight - 40, 0, 0)
                };
                Stand.MouseDown += Stand_MouseDown;
                Staende.Add(Stand);
            }
            foreach (var item in UnbesuchteStandorte)
            {
                UnbesuchteStandorteUIGRID.Items.Add(new Standort_st() { SBezeichnung = item.SBezeichnung, Bezeichnung = item.Bezeichnung });
                Rectangle Stand = new Rectangle()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ToolTip = item.SBezeichnung,
                    Tag = item.UnterOber,
                    Height = 40,
                    Width = 40,
                    Fill = UnbesuchtPin,
                    Margin = new Thickness(item.X_Kooridinate / 100.00 * ImageGrid.ActualWidth - 20, item.Y_Kooridinate / 100.00 * ImageGrid.ActualHeight - 40, 0, 0)
                };
                Stand.MouseDown += Stand_MouseDown;
                Staende.Add(Stand);
            }

            foreach (var item in Staende)
            {
                if (Untergeschoss.IsChecked == true && (char)item.Tag == 'U')
                {
                    ImageGrid.Children.Add(item);
                }
                else if (Obergeschoss.IsChecked == true && (char)item.Tag == 'O')
                {
                    ImageGrid.Children.Add(item);
                }
            }   

        }
        private void Stand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (Rectangle item in from Rectangle item in Staende
                                 where sender == item
                                 select item)
            {
                MessageBox.Show((string)item.ToolTip);
            }
        }

        private void Untergeschoss_Checked(object sender, RoutedEventArgs e)
        {

            ImageGrid.Children.Clear();
            foreach (Rectangle item in from item in Staende
                                 where (char)item.Tag == 'U'
                                 select item)
            {
                ImageGrid.Children.Add(item);
            }

            ImageBrush bg = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_EG.png", UriKind.Relative))
            };
            ImageGrid.Background = bg;
        }

        private void Obergeschoss_Checked(object sender, RoutedEventArgs e)
        {
            ImageGrid.Children.Clear();
            foreach (Rectangle item in from item in Staende
                                 where (char)item.Tag == 'O'
                                 select item)
            {
                ImageGrid.Children.Add(item);
            }

            ImageBrush bg = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("../../Images/grundriss_OG.png", UriKind.Relative))
            };
            ImageGrid.Background = bg;
        }

        private void Abbrechen_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Input_card(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            if (KartenNummer.Text.Length >= 10)
            {
                ClearAll();
                GetandSetAllStandorte();
                dp1.Stop();
                dp1 = new DispatcherTimer();
                dp1.Interval = TimeSpan.FromSeconds(1);
                dp1.Tick += Dp1_Tick1;
                dp1.Start();

                KartenNummer.Text = "";
            }
        }

        private void Dp1_Tick1(object sender, EventArgs e)
        {
            counter++;
            if (counter == 10)
            {
                if (IsInfoMode)
                {
                    ClearAll();
                    dp1.Stop();
                }
                else
                {
                    this.DialogResult = false;
                }
            }
        }

        public void ClearAll()
        {
            SortierteAktionen = new List<Aktion_akt>();
            Aktionen = new List<Standort_st>();
            Staende = new List<Rectangle>();
            LastSelectedItem = "";
            LastSelectedItemStandorteUnbesucht = "";
            ImageGrid.Children.Clear();
            Datagrid.Items.Clear();
            UnbesuchteStandorteUIGRID.Items.Clear();
            Untergeschoss.IsChecked = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach ((Standort_st aktionen, Rectangle staende) in from aktionen in Aktionen
                                                from staende in Staende
                                                where (string)staende.ToolTip == aktionen.SBezeichnung
                                                select (aktionen, staende))
            {
                staende.Margin = new Thickness(aktionen.X_Kooridinate / 100 * ImageGrid.ActualWidth - staende.Width / 2, aktionen.Y_Kooridinate / 100 * ImageGrid.ActualHeight - staende.Height, 0, 0);
            }

            foreach ((Standort_st item, Rectangle staende) in from item in UnbesuchteStandorte
                                            from staende in
                                                from Rectangle staende in Staende
                                                where (string)staende.ToolTip == item.SBezeichnung
                                                select staende
                                            select (item, staende))
            {
                staende.Margin = new Thickness(item.X_Kooridinate / 100 * ImageGrid.ActualWidth - staende.Width / 2, item.Y_Kooridinate / 100 * ImageGrid.ActualHeight - staende.Height, 0, 0);
            }
        }
        private void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Datagrid.SelectedItem != null)
            {
                Standort_st DatagridItems = Datagrid.SelectedItem as Standort_st;
                foreach (Rectangle item in from Rectangle item in Staende
                                     where (string)item.ToolTip == DatagridItems.SBezeichnung
                                     select item)
                {
                    if ((char)item.Tag == 'U')
                    {
                        Untergeschoss.IsChecked = true;
                    }
                    else
                    {
                        Obergeschoss.IsChecked = true;
                    }

                    if (LastSelectedItem != "")
                    {
                        foreach (var LastSelection in from Rectangle LastSelection in Staende
                                                      where (string)LastSelection.ToolTip == LastSelectedItem
                                                      select LastSelection)
                        {
                            LastSelection.Width -= 40;
                            LastSelection.Height -= 40;
                            LastSelection.Margin = new Thickness(LastSelection.Margin.Left + 20, LastSelection.Margin.Top + 40, LastSelection.Margin.Right, LastSelection.Margin.Bottom);
                        }
                    }

                    item.Width += 40;
                    item.Height += 40;
                    item.Margin = new Thickness(item.Margin.Left - 20, item.Margin.Top - 40, item.Margin.Right, item.Margin.Bottom);
                    LastSelectedItem = (string)item.ToolTip;
                }
            }
        }

        private void UnbesuchteStandorteUIGRID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnbesuchteStandorteUIGRID.SelectedItem != null)
            {
                Standort_st DatagridItems = UnbesuchteStandorteUIGRID.SelectedItem as Standort_st;
                foreach (Rectangle item in from Rectangle item in Staende
                                     where (string)item.ToolTip == DatagridItems.SBezeichnung
                                     select item)
                {
                    if ((char)item.Tag == 'U')
                    {
                        Untergeschoss.IsChecked = true;
                    }
                    else
                    {
                        Obergeschoss.IsChecked = true;
                    }

                    if (LastSelectedItemStandorteUnbesucht != "")
                    {
                        foreach (Rectangle LastSelection in from Rectangle LastSelection in Staende
                                                      where (string)LastSelection.ToolTip == LastSelectedItemStandorteUnbesucht
                                                      select LastSelection)
                        {
                            LastSelection.Width -= 40;
                            LastSelection.Height -= 40;
                            LastSelection.Margin = new Thickness(LastSelection.Margin.Left + 20, LastSelection.Margin.Top + 40, LastSelection.Margin.Right, LastSelection.Margin.Bottom);
                        }
                    }

                    item.Width += 40;
                    item.Height += 40;
                    item.Margin = new Thickness(item.Margin.Left - 20, item.Margin.Top - 40, item.Margin.Right, item.Margin.Bottom);
                    LastSelectedItemStandorteUnbesucht = (string)item.ToolTip;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            KartenNummer.Focus();
            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("Wollen sie das Fenster sicher schließen?", "Rfid Scanner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            counter = 0;
        }

        private void Datagrid_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void UnbesuchteStandorteUIGRID_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
