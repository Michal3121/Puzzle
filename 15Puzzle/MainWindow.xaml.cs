using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _15Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PuzzleManager puzzleManager;

        public MainWindow()
        {
            InitializeComponent();
            puzzleManager = new PuzzleManager();
        }

        private void FileImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            var puzzleClicked = frameworkElement.DataContext as _15Puzzle.PuzzleImage; 
            var viewModel = this.itemsControl.DataContext as ViewModels.MainViewModel;

            if (viewModel.GameStarted)
            {
                IList<PuzzleImage> tmp = puzzleManager.Swap(puzzleClicked, viewModel.PuzzleCardsList);
                viewModel.PuzzleCardsList = new ObservableCollection<PuzzleImage>(tmp);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {           
            var frameworkElement = (FrameworkElement)sender;
            var viewModel = frameworkElement.DataContext as ViewModels.MainViewModel;
            if (viewModel.GameStarted)
            {
                IList<PuzzleImage> tmp;
                switch (e.Key)
                {
                    case Key.Up:
                        tmp = puzzleManager.Swap("up", viewModel.PuzzleCardsList);
                        viewModel.PuzzleCardsList = new ObservableCollection<PuzzleImage>(tmp);
                        break;
                    case Key.Down:
                        tmp = puzzleManager.Swap("down", viewModel.PuzzleCardsList);
                        viewModel.PuzzleCardsList = new ObservableCollection<PuzzleImage>(tmp);
                        break;
                    case Key.Left:
                        tmp = puzzleManager.Swap("left", viewModel.PuzzleCardsList);
                        viewModel.PuzzleCardsList = new ObservableCollection<PuzzleImage>(tmp);
                        break;
                    case Key.Right:
                        tmp = puzzleManager.Swap("right", viewModel.PuzzleCardsList);
                        viewModel.PuzzleCardsList = new ObservableCollection<PuzzleImage>(tmp);
                        break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            var viewModel = frameworkElement.DataContext as ViewModels.MainViewModel;

            viewModel.SaveRegistry();
        }
    }
}
