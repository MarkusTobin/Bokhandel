using Labb2DatabasTest.Model;
using System.Diagnostics;
using System.Windows;

namespace Labb2DatabasTest.ViewModel
{

    public partial class LäggTillNyBok : Window
    {
        private Böcker nyBok;

        public LäggTillNyBok()
        {
            InitializeComponent();
            using var db = new BokhandelContext();
            lbFörfattare.ItemsSource = db.Författares.ToList();
        }

        private void LäggTillNyBok_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                using var db = new BokhandelContext();
                nyBok = CreateNewBook();

                var selectedFörfattare = lbFörfattare.SelectedItems.Cast<Författare>().ToList();
                foreach (var författare in selectedFörfattare)
                {
                    var existingFörfattare = db.Författares
                        .FirstOrDefault(f => f.Förnamn == författare.Förnamn && f.Efternamn == författare.Efternamn);

                    if (existingFörfattare != null)
                    {
                        nyBok.Författares.Add(existingFörfattare);
                    }
                }

                db.Böckers.Add(nyBok);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the book: " + ex.Message, "Error", MessageBoxButton.OK);
                }

                DialogResult = true;
                Close();
            }
        }

        private bool IsInputValid()
        {

            bool isDateValid = DateOnly.TryParseExact(tbUtgivardatum.Text, "yyyy-MM-dd", out _);

            return
                !string.IsNullOrWhiteSpace(tbTitle.Text) &&
                !string.IsNullOrWhiteSpace(tbIsbn13.Text) &&
                tbIsbn13.Text.Length == 13 &&
                !string.IsNullOrWhiteSpace(tbSpråk.Text) &&
                !string.IsNullOrWhiteSpace(tbPris.Text) &&
                isDateValid &&
                lbFörfattare.SelectedItems.Count > 0;
        }

        private Böcker CreateNewBook()
        {
            var newBook = new Böcker
            {
                Title = tbTitle.Text,
                Isbn13 = tbIsbn13.Text,
                Språk = tbSpråk.Text,
                Pris = decimal.Parse(tbPris.Text),
                Utgivardatum = DateOnly.Parse(tbUtgivardatum.Text),
            };
            return newBook;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateButtonState(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            btnLäggTillNyBok.IsEnabled = IsInputValid();
        }

        private void lbFörfattare_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateButtonState(sender, null);
            btnTaBortFörfattare.IsEnabled = lbFörfattare.SelectedItems.Count > 0;
        }

        private void LäggTillFörfattare_Click(object sender, RoutedEventArgs e)
        {
            var nyFörfattareFönster = new Författarhanterare();
            nyFörfattareFönster.ShowDialog();


            using var db = new BokhandelContext();
            lbFörfattare.ItemsSource = db.Författares.ToList();
        }


        private void TaBortFörfattare_Click(object sender, RoutedEventArgs e)
        {
            var selectedFörfattareList = lbFörfattare.SelectedItems.Cast<Författare>().ToList();
            if (selectedFörfattareList.Any())
            {
                using var db = new BokhandelContext();
                foreach (var selectedFörfattare in selectedFörfattareList)
                {
                    var hasBooks = db.Böckers.Any(b => b.Författares.Any(f => f.FörfattareId == selectedFörfattare.FörfattareId));
                    if (hasBooks)
                    {
                        MessageBox.Show($"Cannot remove author {selectedFörfattare.FullName} because they have books in the bookcollection.", "Error", MessageBoxButton.OK);
                        return;
                    }
                }

                var författareNames = string.Join(", ", selectedFörfattareList.Select(f => f.FullName));
                var result = MessageBox.Show($"Are you sure you want to remove the following authors: {författareNames}?", "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selectedFörfattare in selectedFörfattareList)
                    {
                        db.Författares.Remove(selectedFörfattare);
                    }

                    try
                    {
                        db.SaveChanges();
                        lbFörfattare.ItemsSource = db.Författares.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while removing the authors: " + ex.Message, "Error", MessageBoxButton.OK);
                    }
                    UpdateButtonState(sender, null);
                }
            }
        }
    }
}

