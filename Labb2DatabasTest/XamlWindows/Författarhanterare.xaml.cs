using Labb2DatabasTest.Model;
using System.Windows;

namespace Labb2DatabasTest.XamlWindows
{

    public partial class Författarhanterare : Window
    {
        public Författare NewFörfattare { get; private set; }
        public Författarhanterare()
        {
            InitializeComponent();
        }

        private void LäggTillNyFörfattare_Click(object sender, RoutedEventArgs e)
        {
            if (NyFörfattareValid())
            {
                using var db = new BokhandelContext();
                NewFörfattare = CreateNewFörfattare();

                db.Författares.Add(NewFörfattare);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the author: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                DialogResult = true;
                Close();
            }
        }   


        private bool NyFörfattareValid()
        {
            bool isDateValid = DateOnly.TryParseExact(tbFödelsedatum.Text, "yyyy-MM-dd", out _);

            return 
                !string.IsNullOrWhiteSpace(tbFörnamn.Text) &&
                !string.IsNullOrWhiteSpace(tbEfternamn.Text) &&
                isDateValid; ;
        }
        private Författare CreateNewFörfattare()
        {
            var newFörfattare = new Författare
            {
                Förnamn = tbFörnamn.Text,
                Efternamn = tbEfternamn.Text,
                Födelsedatum = DateOnly.Parse(tbFödelsedatum.Text)
            };
            return newFörfattare;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateButtonState(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            btnLäggTill.IsEnabled = NyFörfattareValid();
        }
    }
}
