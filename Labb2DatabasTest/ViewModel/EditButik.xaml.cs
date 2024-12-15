using Labb2DatabasTest.Model;
using System.Windows;

namespace Labb2DatabasTest.ViewModel
{
    public partial class EditButik : Window
    {
        private readonly Butiker _oldButik;
        private readonly Butiker _newButik;
        public EditButik(Butiker butik)
        {
            InitializeComponent();

            _oldButik = new Butiker
            {
                Butiksnamn = butik.Butiksnamn,
                Adress = butik.Adress,
                Stad = butik.Stad,
                Hemsida = butik.Hemsida
            };


            _newButik = butik;

            txtButiksnamn.Text = _newButik.Butiksnamn;
            txtAdress.Text = _newButik.Adress;
            txtStad.Text = _newButik.Stad;
            txtHemsida.Text = _newButik.Hemsida;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            var result = MessageBox.Show(
                "Are you sure you want to save changes?",
                "Confirm Save",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _newButik.Butiksnamn = txtButiksnamn.Text;
                _newButik.Adress = txtAdress.Text;
                _newButik.Stad = txtStad.Text;
                _newButik.Hemsida = txtHemsida.Text;

                DialogResult = true;
                Close();
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _newButik.Butiksnamn = _oldButik.Butiksnamn;
            _newButik.Adress = _oldButik.Adress;
            _newButik.Stad = _oldButik.Stad;
            _newButik.Hemsida = _oldButik.Hemsida;

            DialogResult = false;
            Close();
        }
    }
}
