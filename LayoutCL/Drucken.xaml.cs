using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Drucken.xaml
    /// </summary>
    /// Test
    public partial class Drucken : Window
    {
        private string veranstaltung;
        private TextBox name = new TextBox();
        private string chip = "";
        public Drucken()
        {
            InitializeComponent();
            Uichipnr.Focus();
        }
        public void SetVeranstaltung(string Veranstaltung)
        {
            veranstaltung = Veranstaltung;
        }

        public static int Cm2Dip(double cm)
        {
            return (int)(cm / 2.54 * 96);
        }

        public FixedDocument GenerateFixedDocument()
        {
            Uichipnr.Focus();
            Size pagesize = new Size(Cm2Dip(21), Cm2Dip(29.7));

            FixedDocument document = new FixedDocument();

            document.DocumentPaginator.PageSize = pagesize;

            FixedPage page1 = new FixedPage
            {
                Width = document.DocumentPaginator.PageSize.Width,
                Height = document.DocumentPaginator.PageSize.Height
            };
            try
            {
                if (UimitLogo.IsChecked == true)
                {
                    Image image1 = new Image
                    {
                        Source = new BitmapImage(new Uri("../../Images/Edvschule Logo.png", UriKind.Relative)),
                        Margin = new Thickness(Cm2Dip(12), Cm2Dip(0), 0, 0),
                        // image1.Width = Cm2Dip(9);
                        Height = Cm2Dip(4.5)
                    };
                    page1.Children.Add(image1);

                    TextBlock adre = new TextBlock
                    {
                        Text = "EDV Schulen des Landkreises Deggendorf - Georg-Eckl-Straße 18 - 94447 Plattling",
                        Margin = new Thickness(Cm2Dip(1.3), Cm2Dip(5), 0, 0),
                        FontSize = 8,
                        Foreground = Brushes.DarkBlue
                    };
                    page1.Children.Add(adre);

                    TextBlock bold = new TextBlock
                    {
                        Text = "\n Berufsfachschule für IT-Berufe" +
                        "\n\n\n\n\n\n Fachschule für Wirtschatfsinformatik" +
                        "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                        "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                        "\n\n\n\n\n Adresse:" +
                        "\n\n\n\n\n\n\n\n\n\n\n E-Mail:" +
                        "\n\n\n Homepage:",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(new Color() { A = 255, R = 0, G = 27, B = 145 }),
                        FontFamily = new FontFamily("Century")
                    };
                    TextBlock rand = new TextBlock
                    {
                        Text = "\n\n              Fachinformatiker für " +
                                 "\n              Anwendungsentwicklung" +
                                 "\n              bzw. Systemintegration" +
                                 "\n\n\n\n              Staatlich geprüfter" +
                                 "\n              Wirtschaftsinformatiker" +
                                 "\n              (Bachelor Professional)" +
                                 "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                                 "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                                 "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                                 "\n              EDV-Schulen des" +
                                 "\n              Landkreises Deggendorf" +
                                 "\n              Georg-Eckl-Straße 18" +
                                 "\n              94447 Plattling" +
                                 "\n\n              Tel: 09931/95101" +
                                 "\n              Fax: 09931/951218" +
                                 "\n\n\n\n\n         verwaltung@edvschule-plattling.de" +
                                 "\n\n\n            http://www.edvschule-plattling.de",
                        Foreground = new SolidColorBrush(new Color() { A = 255, R = 2, G = 120, B = 156 }),
                        FontFamily = new FontFamily("Century")
                    };
                    bold.Background = Brushes.Transparent;
                    bold.Margin = new Thickness(Cm2Dip(16), Cm2Dip(5), 0, 0);
                    bold.FontSize = 8;
                    rand.Background = Brushes.AliceBlue;
                    rand.Margin = new Thickness(Cm2Dip(16), Cm2Dip(5), 0, 0);
                    rand.Height = Cm2Dip(23);
                    rand.Width = Cm2Dip(5);
                    rand.FontSize = 8;
                    page1.Children.Add(rand);
                    page1.Children.Add(bold);
                }

                name = new TextBox
                {
                    Text = "Herr" +
                         "\nMax Mustermann " +
                         "\nGeorg-Eckl-Straße 18 " +
                         "\n94447 Plattling",
                    IsReadOnly = false,
                    AcceptsReturn = true,
                    BorderBrush = Brushes.Transparent,
                    Margin = new Thickness(Cm2Dip(1.3), Cm2Dip(6), 0, 0),
                    FontSize = 12
                };
                page1.Children.Add(name);

                DbPostgres postgres = new DbPostgres("ux4.dvs-plattling.de", "db_alehner", "alehner", "alehner");

                Dictionary<string, DateTime> erg = new Dictionary<string, DateTime>();
                erg = postgres.GetTeilnehmervonAktion(chip, veranstaltung);//Uichipnr.Text);

                TextBlock datum = new TextBlock
                {
                    Text = "Datum: \n" + DateTime.Now.ToString("dd.MM.yyyy"),
                    Margin = new Thickness(Cm2Dip(13), Cm2Dip(8.5), 0, 0),
                    FontSize = 12
                };
                page1.Children.Add(datum);

                TextBlock ver = new TextBlock
                {
                    Text = "Besuchsbescheinigung " + postgres.GetVeranstaltungvonTeilnehmer(chip),
                    Margin = new Thickness(Cm2Dip(1.3), Cm2Dip(10), 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 17
                };
                page1.Children.Add(ver);
                TextBlock ak = new TextBlock
                {
                    FontSize = 15,
                    FontStretch = FontStretches.UltraCondensed
                };
                if (erg.Count > 15)
                {
                    ak.FontSize = 15;
                }
                ak.Text = "Du hast folgende Aktionen besucht:\n\n";

                const string unicode = "  • ";
                foreach (var item in erg)
                {
                    if (erg.Count > 15)
                    {
                        ak.Text += unicode + item.Key + "\n";
                    }
                    else if (erg.Count > 30)
                    {
                        ak.Text += unicode + item.Key + "\n";
                        ak.FontSize = 12;
                    }
                    else
                    {
                        ak.Text += unicode + item.Key + "\n\n";
                    }
                }
                ak.Text += "\nVielen Dank für deinen Besuch\n\n";
                ak.Text += "Dein RfidChase-Team";
                ak.Margin = new Thickness(Cm2Dip(1.3), Cm2Dip(11.5), 0, 0);

                page1.Children.Add(ak);

                PageContent page1Content = new PageContent();
                ((IAddChild)page1Content).AddChild(page1);
                document.Pages.Add(page1Content);
            }
            catch (Exception)
            {
                MessageBox.Show("Chipnr nicht vorhanden oder noch nicht in der Datenbank", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Uichipnr.Clear();
            }
            return document;
        }

        private void Uichipnr_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Uichipnr.Text.Length == 10)
            {
                chip = Uichipnr.Text;
                uiDocumentViewer.Document = GenerateFixedDocument();
                uiDocumentViewer.FitToMaxPagesAcross(1);
                Uichipnr.Clear();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!name.IsFocused)
            {
                Uichipnr.Focus();
            }
            if (e.Key == Key.Tab)
            {
                Uichipnr.Focus();
            }
            if(e.Key == Key.Escape && name.IsFocused)
            {
                Keyboard.ClearFocus();
                Uichipnr.Focus();
            }
        }

        private void UimitLogo_Checked(object sender, RoutedEventArgs e)
        {
            if (chip != "")
            {
                uiDocumentViewer.Document = GenerateFixedDocument();
            }
            Uichipnr.Focus();
            return;
        }

        private void UimitLogo_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chip != "")
            {
                uiDocumentViewer.Document = GenerateFixedDocument();
            }
            Uichipnr.Focus();
            return;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uiDocumentViewer.Document = null;
        }
    }
}
