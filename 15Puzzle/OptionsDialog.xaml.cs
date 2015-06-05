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
using System.Windows.Shapes;

namespace _15Puzzle
{
    /// <summary>
    /// Interaction logic for OptionsDialog.xaml
    /// </summary>
    public partial class OptionsDialog : Window
    {
        public int SelectedDifficulty
        {
            get { return (int) this.difficultyComboBox.SelectedIndex; }
            set { this.difficultyComboBox.SelectedIndex = value; }
        }
       
        public string SelectedImageName
        {
            get { return (string)this.imagesComboBox.SelectedValue; }
            set { this.imagesComboBox.SelectedValue = value; }
        }

        public OptionsDialog(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();
            this.difficultyComboBox.ItemsSource = Enum.GetNames(typeof(Difficulty));
            this.imagesComboBox.ItemsSource = viewModel.LoadedImagesNamesForOptions;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
