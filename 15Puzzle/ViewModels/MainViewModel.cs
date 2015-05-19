using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace _15Puzzle.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private BitmapSource _image00 = new BitmapImage(new Uri("/Images/Rafael2.jpg", UriKind.Relative));
        private BitmapSource _image01 = new BitmapImage();
        private double _puzzleWidth = 400;


        public BitmapSource Image00
        {
            get { return this._image00; }
            set
            {
                if (this._image00 != value)
                {
                    this._image00 = value;
                    RaisePropertyChanged("Image00");
                }
            }
        }


        public BitmapSource Image01
        {
            get { return this._image01; }
            set
            {
                if (this._image01 != value)
                {
                    this._image01 = value;
                    RaisePropertyChanged("Image01");
                    Console.WriteLine("///////////////////////////////////////////");
                }
            }
        }

        public double ActualWidth
        {
            get { return this._puzzleWidth / 2; }
            set
            {
                if (this._puzzleWidth != value)
                {
                    this._puzzleWidth = value;
                    RaisePropertyChanged("ActualWidth");
                    Console.WriteLine("///////////////////////////////////////////");
                    Console.WriteLine("Width: " + this._puzzleWidth);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new RelayCommand(() => this.OpenFile());
            }
        }

        public void OpenFile()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "JPEG (*.jpg, *.jpeg, *.jpe, *.jfif,) | *.jpg; *.jpeg; *.jpe; *.jfif|" +
                         "PNG (*.png)|*.png"
            };


            if (ofd.ShowDialog() == true)
            {
                ImageSplitter imgSplit;
                List<BitmapSource> images = new List<BitmapSource>();

                try
                {
                    imgSplit = new ImageSplitter(new Uri(ofd.FileName, UriKind.Absolute));
                    images = imgSplit.SplitImage(4, 4);
                }
                /*catch (ArgumentException ex1)
                {
                    MessageBox.Show(ex1.Source);
                }
                catch (UriFormatException ex2)
                {
                    MessageBox.Show(ex2.Source);
                }
                catch (NotSupportedException ex3)
                {
                    MessageBox.Show(ex3.Source);
                }*/
                catch (NotSquareImageSizeException)
                {
                    MessageBox.Show("Image does not have square size!");
                }

                if (images.Any())
                {
                    this.Image01 = images[4];
                }

                Console.WriteLine("FileName " + ofd.FileName);
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand(() => Application.Current.Shutdown());
            }
        }

        private void Parse()
        {

        }


    }
}
