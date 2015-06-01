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
        private List<int> _difficultyList;
        public List<int> DifficultyList
        {
            get { return this._difficultyList; }
            set 
            { 
                this._difficultyList = value;
                this.comboBoxDifficulty.DataContext = this.DifficultyList;
            }
        }

        public int SelecteDifficulty
        {
            get { return (int)this.comboBoxDifficulty.SelectedItem; }
        }


        public OptionsDialog()
        {
            InitializeComponent();
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
