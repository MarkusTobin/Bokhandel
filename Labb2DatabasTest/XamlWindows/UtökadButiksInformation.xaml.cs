using Labb2DatabasTest.Model;
using System.Printing;
using System.Threading.Channels;
using System.Windows;

namespace Labb2DatabasTest.XamlWindows
{
    public partial class UtökadButiksInformation : Window
    {
        private Butiker _selectedButik;
        public UtökadButiksInformation(Butiker butik)
        {
            InitializeComponent();
            _selectedButik = butik;
            Loaded += UtökadButiksInformation_Loaded;
        }
        private void UtökadButiksInformation_Loaded(object sender, RoutedEventArgs e)
        {
            SetUtökadButikListBox();
        }

        private void SetUtökadButikListBox()
        {
            using var db = new BokhandelContext();
            var butiksInfo = db.InfoPerButiks
                .Where(info => info.Butiksnamn == _selectedButik.Butiksnamn)
                .ToList();

            var allaButiker = new InfoPerButik { Butiksnamn = "Alla butiker" };

            var butiker = db.InfoPerButiks.ToList();
            butiker.Insert(0, allaButiker);

            utökadButikListBox.ItemsSource = butiker;

            if (_selectedButik.Butiksnamn == "Alla butiker")
            {
                utökadButikListBox.SelectedIndex = 0;
            }
            else
            {
                utökadButikListBox.SelectedItem = butiker.FirstOrDefault(b => b.Butiksnamn == _selectedButik.Butiksnamn);
            }
            UtökadButikListBox_SelectionChanged(utökadButikListBox, null);
            utökadButikListBox.SelectionChanged += UtökadButikListBox_SelectionChanged;
        }

        private void UtökadButikListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (utökadButikListBox.SelectedItem is InfoPerButik selectedButik)
            {
                using var db = new BokhandelContext();

                _selectedButik = db.Butikers.FirstOrDefault(b => b.Butiksnamn == selectedButik.Butiksnamn) ?? new Butiker();

                if (selectedButik.Butiksnamn == "Alla butiker")
                {
                    LagerButton.IsEnabled = false;
                    utökadMyDataGrid.ItemsSource = db.InfoPerButiks.ToList();
                }
                else
                {
                    LagerButton.IsEnabled = true;
                    utökadMyDataGrid.ItemsSource = new[] { selectedButik };
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LagerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedButik == null || _selectedButik.Id == 0)
            {
                MessageBox.Show("Välj en specifik butik för att se Lager.", "Ingen butik vald", MessageBoxButton.OK);
                return;
            }

            var böckerHanterare = new BöckerHanterare(_selectedButik);

            böckerHanterare.ButikUpdated += SetUtökadButikListBox;
            böckerHanterare.ShowDialog();
        }
    }
}