using Labb2DatabasTest.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Labb2DatabasTest.ViewModel
{

    public partial class BöckerHanterare : Window
    {
        private Butiker _selectedButik;

        public event Action ButikUpdated;
        public BöckerHanterare(Butiker butik)
        {
            InitializeComponent();
            _selectedButik = butik;
            Loaded += BöckerHanterare_Loaded;
        }

        private void NotifyDataUpdated()
        {
            ButikUpdated?.Invoke();
        }
        private void BöckerHanterare_Loaded(object sender, RoutedEventArgs e)
        {
            SetBöckerListBox();
        }

        private void SetBöckerListBox()
        {
            using var db = new BokhandelContext();

            var böcker = db.Böckers.ToList();
            var läggTillBok = new Böcker { Title = "Lägg till ny bok" };
            böcker.Insert(0, läggTillBok);
            var böckerIButik = db.LagerSaldos
        .Where(l => l.ButikId == _selectedButik.Id && l.AntalBöckerKvar > 0)
        .Select(l => new
        {
            l.Böckers.Isbn13,
            l.Böckers.Title, 
            l.Böckers.Språk,
            l.Böckers.Pris, 
            l.Böckers.Utgivardatum, 
            AntalBöckerKvar = l.AntalBöckerKvar
        })
        .ToList();

            //Ändrar bakgrundsfärg på första boken i listan 
            Dispatcher.InvokeAsync(() =>
            {
                if (böckerListBox.ItemContainerGenerator.ContainerFromIndex(0) is ListBoxItem item)
                {
                    item.Background = new SolidColorBrush(Colors.LightGreen);
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);


            böckerListBox.ItemsSource = böcker;
            böckerListBox.DisplayMemberPath = "Title";

            böckerMyDataGrid.ItemsSource = böckerIButik;
        }

        private void BöckerListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Check if the clicked item is the first item ("Lägg till ny bok")
            if (böckerListBox.SelectedIndex == 0)
            {
                var läggTillNyBokWindow = new LäggTillNyBok();
                if (läggTillNyBokWindow.ShowDialog() == true)
                {
                    SetBöckerListBox();
                }
            }

        }

        private void Ta_bort_från_Sortimentet_Click(object sender, RoutedEventArgs e)
        {
            if (böckerListBox.SelectedItem is Böcker selectedBook)
            {
                using var db = new BokhandelContext();

                var book = db.Böckers
                            .Include(b => b.Författares)
                            .FirstOrDefault(b => b.Isbn13 == selectedBook.Isbn13);
                var existerarIButik = db.LagerSaldos.Any(l => l.Isbn13 == book.Isbn13);

                if (existerarIButik)
                {
                    MessageBox.Show("Du kan inte ta bort en bok som existerar i en butiks lager.", "Error", MessageBoxButton.OK);
                    return;
                }
                var result = MessageBox.Show(
                $"Are you sure you want to REMOVE {selectedBook.Title}?",
                "Confirm Save",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes && book != null)
                {
                    foreach (var författare in book.Författares.ToList()) 
                    {
                        book.Författares.Remove(författare);
                    }

                    db.Böckers.Remove(book);
                    db.SaveChanges();
                    SetBöckerListBox();
                }
            }
        }

        private void EnableRemoveBookButton(object sender, SelectionChangedEventArgs e)
        {
            if (böckerListBox.SelectedItem is Böcker selectedBok)
            {
                taBortFrånSortimentet.IsEnabled = selectedBok != null && böckerListBox.SelectedIndex != 0;
            }
        }

        private void LäggTillBokIButik_Click(object sender, RoutedEventArgs e)
        {
            if (böckerListBox.SelectedItem is Böcker selectedbok && böckerListBox.SelectedIndex != 0)
            {
                var inputDialog = new InputDialog($"Lägg till boken: {selectedbok.Title}\nTill: {_selectedButik.Butiksnamn}\nAnge antal:");

                if (inputDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputDialog.Answer, out var antalBöcker) && antalBöcker > 0)
                    {
                        using var db = new BokhandelContext();

                        var bookExistsInStoreCheck = db.LagerSaldos.FirstOrDefault(l => l.ButikId == _selectedButik.Id && l.Isbn13 == selectedbok.Isbn13);

                        MessageBox.Show($"Added {antalBöcker} books to the store.", "Information", MessageBoxButton.OK);
                        if (bookExistsInStoreCheck != null)
                        {
                            bookExistsInStoreCheck.AntalBöckerKvar += antalBöcker;
                        }
                        else
                        {
                            var newLagerSaldo = new LagerSaldo
                            {
                                ButikId = _selectedButik.Id,
                                Isbn13 = selectedbok.Isbn13,
                                AntalBöckerKvar = antalBöcker
                            };
                            db.LagerSaldos.Add(newLagerSaldo);
                        }
                        db.SaveChanges();
                        SetBöckerListBox();
                        NotifyDataUpdated();
                    }
                    else
                    {
                        MessageBox.Show("Något gick fel", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void TaBortBokIButik_Click(object sender, RoutedEventArgs e)
        {
            if (böckerMyDataGrid.SelectedItem is { } selectedBook)
            {
                dynamic book = selectedBook;
                var inputDialog = new InputDialog($"Ta bort: {book.Title}\nFrån: {_selectedButik.Butiksnamn} \nAnge antal: ");
                if (inputDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputDialog.Answer, out var antalBöcker) && antalBöcker > 0)
                    {
                        using var db = new BokhandelContext();
                        var bookToRemoveInStore = db.LagerSaldos.FirstOrDefault(l => l.ButikId == _selectedButik.Id);
                        if (bookToRemoveInStore != null)
                        {
                            if (bookToRemoveInStore.AntalBöckerKvar - antalBöcker > 0)
                            {
                              MessageBox.Show($"Removing {antalBöcker} books from the store.", "Information", MessageBoxButton.OK);
                                bookToRemoveInStore.AntalBöckerKvar -= antalBöcker;
                            }
                            else
                            {
                                 MessageBox.Show($"Removing {bookToRemoveInStore.AntalBöckerKvar} books from the store.", "Information", MessageBoxButton.OK);
                                db.LagerSaldos.Remove(bookToRemoveInStore);
                            }

                            db.SaveChanges();
                            SetBöckerListBox();
                            NotifyDataUpdated();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Något gick fel", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}