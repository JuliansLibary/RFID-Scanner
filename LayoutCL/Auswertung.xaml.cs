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
using LiveCharts;
using LiveCharts.Wpf;
using OxyPlot.Wpf;
using RFID_Scanner.DatabaseCL;

namespace RFID_Scanner.LayoutCL
{
    /// <summary>
    /// Interaktionslogik für Auswertung.xaml
    /// </summary>
    public partial class Auswertung : Window
    {

        ColumnSeries c1 = new ColumnSeries();
        public void SetChart(string name) {
            List<DateTime> DatumsWerte = DbPostgres.Instance.GetChartContent(name);
            ChartValues<int> Werte = new ChartValues<int>();
            for (int i = 0; i < 24; i++)
            {
                Werte.Add(0);
            }
            foreach (var item in DatumsWerte)
            {
                Werte[item.Hour] += 1;
            }
            c1 = new ColumnSeries
            {
                Title = "Besucher",
                FontWeight = FontWeights.Light,
                FontSize = 14,
                Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Values = Werte
            };
            SeriesCollection = new SeriesCollection
            {
                c1
            };

            Labels = new[] { "00:00 Uhr", "01:00 Uhr", "02:00 Uhr", "03:00 Uhr", "04:00 Uhr", "05:00 Uhr", "06:00 Uhr", "07:00 Uhr", "08:00 Uhr", "09:00 Uhr", "10:00 Uhr", "11:00 Uhr", "12:00 Uhr", "13:00 Uhr", "14:00 Uhr", "15:00 Uhr", "16:00 Uhr", "17:00 Uhr", "18:00 Uhr", "19:00 Uhr", "20:00 Uhr", "21:00 Uhr", "22:00 Uhr", "23:00 Uhr" };
            Formatter = value => value.ToString("N");

            DataContext = this;

        }
        public Auswertung()
        {

            InitializeComponent();
        }

        public void SetDarkmode()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(32, 40, 49));
            chart.Foreground = Brushes.WhiteSmoke;
            X_Axis.Foreground = Brushes.DarkGray;
            Y_Axis.Foreground = Brushes.DarkGray;
            ToolTip.Foreground = Brushes.Gray;
            ToolTip.Background = Brushes.LightGray;
            c1.Fill = Brushes.WhiteSmoke;
            X_Axis_Stroke.Stroke = Brushes.WhiteSmoke;
            Y_Axis_Stroke.Stroke = Brushes.WhiteSmoke;
        }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<int, string> Formatter { get; set; }
    }
    }
