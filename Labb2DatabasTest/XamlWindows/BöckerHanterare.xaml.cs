using Labb2DatabasTest.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Labb2DatabasTest.XamlWindows
{
    public partial class BöckerHanterare : Window
    {
        private readonly Butiker _selectedButik;

        public event Action? ButikUpdated;
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

            var böckerIButik = db.LagerSaldos
        .Where(l => l.ButikId == _selectedButik.Id && l.AntalBöckerKvar > 0)
        .Select(l => new
        {
            l.Böckers.Isbn13,
            l.Böckers.Title, 
            l.Böckers.Språk,
            l.Böckers.Pris, 
            l.Böckers.Utgivardatum,
            l.AntalBöckerKvar
        })
        .ToList();

            böckerListBox.ItemsSource = böcker;
            böckerListBox.DisplayMemberPath = "Title";

            böckerMyDataGrid.ItemsSource = böckerIButik;
        }

        private void Lägg_Till_Bok_I_Sortimentet_Click(object sender, RoutedEventArgs e)
        {
            var läggTillNyBokWindow = new LäggTillNyBok();
            if (läggTillNyBokWindow.ShowDialog() == true)
            {
                SetBöckerListBox();
            }
        }

        private void Ta_Bort_Bok_Från_Sortimentet_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show("Du kan inte ta bort en bok som finns i en butiks lager.", "Error", MessageBoxButton.OK);
                    return;
                }
                var result = MessageBox.Show(
                $"Är du säker på att du vill TA BORT {selectedBook.Title}?",
                "Bekräfta",
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

        private void LäggTillBokIButik_Click(object sender, RoutedEventArgs e)
        {
            if (böckerListBox.SelectedItem is Böcker selectedbok)
            {
                var inputAntalDialog = new InputAntalDialog($"Lägg till boken: {selectedbok.Title}\nTill: {_selectedButik.Butiksnamn}\nAnge antal:");

                if (inputAntalDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputAntalDialog.Answer, out var antalBöcker) && antalBöcker > 0)
                    {
                        using var db = new BokhandelContext();

                        var bookExistsInStoreCheck = db.LagerSaldos.FirstOrDefault(l => l.ButikId == _selectedButik.Id && l.Isbn13 == selectedbok.Isbn13);

                        MessageBox.Show($"La till {antalBöcker} böcker till butiken.", "Information", MessageBoxButton.OK);
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
                var inputAntalDialog = new InputAntalDialog($"Ta bort: {book.Title}\nFrån: {_selectedButik.Butiksnamn} \nAnge antal: ");
                if (inputAntalDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputAntalDialog.Answer, out var antalBöcker) && antalBöcker > 0)
                    {
                        var selectedBookIsbn = (string)book.Isbn13;
                        using var db = new BokhandelContext();
                        var bookToRemoveInStore = db.LagerSaldos.FirstOrDefault(l => l.ButikId == _selectedButik.Id && l.Isbn13 == selectedBookIsbn);
                        if (bookToRemoveInStore != null)
                        {
                            if (bookToRemoveInStore.AntalBöckerKvar - antalBöcker > 0)
                            {
                                MessageBox.Show($"Tar bort {antalBöcker} böcker från butiken.", "Information", MessageBoxButton.OK);
                                bookToRemoveInStore.AntalBöckerKvar -= antalBöcker;
                            }
                            else
                            {
                                MessageBox.Show($"Tar bort {bookToRemoveInStore.AntalBöckerKvar} böcker från butiken.", "Information", MessageBoxButton.OK);
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

        private void EnableButtonsOnListboxChangedItem(object sender, SelectionChangedEventArgs e)
        {
            if (böckerListBox.SelectedItem is Böcker selectedBok)
            {
                taBortBokFrånSortimentet.IsEnabled = selectedBok != null;
                läggTillBokIButik.IsEnabled = selectedBok != null;
            }
            else
            {
                taBortBokFrånSortimentet.IsEnabled = false;
                läggTillBokIButik.IsEnabled = false;
            }
        }
        private void BöckerMyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            taBortBokIButik.IsEnabled = böckerMyDataGrid.SelectedItem != null;
        }
    }
}