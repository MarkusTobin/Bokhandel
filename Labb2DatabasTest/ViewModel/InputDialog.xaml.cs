using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Labb2DatabasTest.ViewModel
{
    public partial class InputDialog : Window
    {
        public string Answer { get; private set; }

        public InputDialog(string antal)
        {
            InitializeComponent();
            AntalText.Text = antal;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Answer = AntalTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}