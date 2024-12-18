using Labb2DatabasTest.Model;
using Labb2DatabasTest.XamlWindows;
using System.Windows;

namespace Labb2DatabasTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetButikListBox();
        }

        private void SetButikListBox()
        {
            using var db = new BokhandelContext();
            var butiker = db.Butikers.ToList();

            var allaButiker = new Butiker { Id = 0, Butiksnamn = "Alla butiker" };
            butiker.Insert(0, allaButiker);

            butikListBox.ItemsSource = butiker;

            butikListBox.SelectedIndex = 0;
            ButikListBox_SelectionChanged(butikListBox, null);
            butikListBox.SelectionChanged += ButikListBox_SelectionChanged;
        }

        private void ButikListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (butikListBox.SelectedItem is Butiker selectedButik)
            {
                using var db = new BokhandelContext();
                if (selectedButik.Id == 0)
                {
                    editButton.IsEnabled = false;
                    myDataGrid.ItemsSource = db.Butikers.ToList();
                }
                else
                {
                    editButton.IsEnabled = true;
                    myDataGrid.ItemsSource = new[] { selectedButik };
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (butikListBox.SelectedItem is Butiker selectedButik)
            {
           
                var editWindow = new EditButik(selectedButik);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    using var db = new BokhandelContext();
                    db.Update(selectedButik);
                    db.SaveChanges();


                    var butiker = db.Butikers.ToList();
                    var allaButiker = new Butiker { Id = 0, Butiksnamn = "Alla butiker" };
                    butiker.Insert(0, allaButiker);

                    butikListBox.ItemsSource = butiker;


                    myDataGrid.ItemsSource = new[] { selectedButik };
                }
            }
            else
            {
                MessageBox.Show("Välj en butik att edita.", "Inget valt", MessageBoxButton.OK);
            }
        }

        private void Utökad_InformationButton_Click(object sender, RoutedEventArgs e)
        {
            if (butikListBox.SelectedItem is Butiker selectedButik)
            {
                var utökadInfoWindow = new UtökadButiksInformation(selectedButik);
                utökadInfoWindow.ShowDialog();
            }
        }

        private void Exit_Applikationen_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Är du säker att du vill avsluta programet??", "Stänger", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}