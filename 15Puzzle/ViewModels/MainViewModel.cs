using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace _15Puzzle.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //private Solver solver;
        private MatrixNode matrix;
        private MatrixManager matrixManager = new MatrixManager();
        private long timeElapsed; 
        private DispatcherTimer timer;
        private PuzzleManager puzzleManager;
        private bool canNewGameExecute = true;
        Thread threadShuffle;
        private delegate void changeCanExecuteDelegat();
        private BitmapImage aux;
        
        public MainViewModel()
        {
            this.timeElapsed = 0;
            this.timer = new DispatcherTimer();
            this._actualCanvasHeight = 320;
            this._actualCanvasWidth = 320;
            this.puzzleManager = new PuzzleManager();
            this.aux = new BitmapImage(new Uri("pack://application:,,,/Images/Rafael2.jpg", UriKind.Absolute));
            //this._imageSplit = new ImageSplitter(aux);
            this.Image2Collection(aux);
            
            //this._imageSplit = Properties.Resources.Grey;
        }
        /*
        private BitmapSource _image00 = new BitmapImage(new Uri("/Images/Rafael2.jpg", UriKind.Relative));
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
        */
        private BitmapSource _image01 = new BitmapImage();
        public BitmapSource Image01
        {
            get { return this._image01; }
            set
            {
                if (this._image01 != value)
                {
                    this._image01 = value;
                    RaisePropertyChanged("Image01");
                    //Console.WriteLine("///////////////////////////////////////////");
                }
            }
        }

        private ImageSplitter _imageSplit;
        public ImageSplitter ImageSplit
        {
            get { return this._imageSplit; }
            set { this._imageSplit = value; }
        }

        private double _actualCanvasWidth = 100;
        public double ActualCanvasWidth
        {
            get
            {
                return this._actualCanvasWidth;    
            }
            set
            {
                if (this._actualCanvasWidth != value)
                {
                    this._actualCanvasWidth = value;
                    RaisePropertyChanged("ActualCanvasWidth");
                }
            }
        }

        private double _actualCanvasHeight;
        public double ActualCanvasHeight
        {
            get { return this._actualCanvasHeight; }
            set
            {
                if (this._actualCanvasHeight != value)
                {
                    this._actualCanvasHeight = value;
                    RaisePropertyChanged("ActualCanvasHeight");
                    Console.WriteLine("ActualCanvasHeight" + this._actualCanvasHeight);
                }
            }
        }

        private string _time = String.Format("  :  ");
        public string Time
        {
            get { return this._time; }
            set
            {
                if (this._time != value)
                {
                    this._time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }

        private ObservableCollection<PuzzleImage> _puzzleCardsList = new ObservableCollection<PuzzleImage>(); // BitmapSource
        public ObservableCollection<PuzzleImage> PuzzleCardsList
        {
            get
            {
                //this._puzzleCardsList.Add(new PuzzleImage(Image00, 50));
                /*
                this._puzzleCardsList.Add(new PuzzleImage() 
                            { 
                                Image = Image00, 
                                CanvasWidth = this._actualCanvasWidth, 
                                CanvasHeight = this._actualCanvasHeight,
                                ImageIndex = 0
                            });*/
                return this._puzzleCardsList;
            }
            set 
            {
                if (this._puzzleCardsList != value)
                {
                    this._puzzleCardsList = value;
                    Console.WriteLine("///////////Pocet prvkov" + _puzzleCardsList.Count);
                    RaisePropertyChanged("PuzzleCardsList");
                }        
            }
        }

        private void Image2Collection(BitmapImage image)
        {
            {
                ImageSplitter imgSplit;
                List<BitmapSource> images = new List<BitmapSource>();

                Console.WriteLine("ShowDialog ActualCanvasHeight" + this._actualCanvasHeight);


                try
                {
                    //imgSplit = new ImageSplitter(new Uri(ofd.FileName, UriKind.Absolute));
                    imgSplit = new ImageSplitter(this.aux);
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
                    int i = 0;
                    images.ForEach(e =>
                    {
                        if (i < 15)
                        {
                            this._puzzleCardsList.Add(new PuzzleImage()
                            {
                                Image = e,

                                CanvasWidth = this._actualCanvasWidth,
                                CanvasHeight = this._actualCanvasHeight,
                                ImageIndex = i,
                                IsGap = false,
                            });
                            i++;
                        }
                        else
                        {
                            /*
                             * BitmapSource transparentImage = BitmapImage.Create(1, 1, 96, 96, PixelFormats.Bgra32, 
                                                                               null, new byte[] { 0, 0, 0, 0 }, 4);
                            
                             * */

                            int rawStride = (80 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
                            var rawImage = new byte[rawStride * 80];

                            BitmapSource transparentImage = BitmapSource.Create(80, 80, 96, 96, PixelFormats.Bgra32,
                                                                               null, rawImage, rawStride);
                            this.PuzzleCardsList.Add(new PuzzleImage()
                            {
                                Image = transparentImage,
                                CanvasWidth = this._actualCanvasWidth,
                                CanvasHeight = this._actualCanvasHeight,
                                ImageIndex = i,
                                IsGap = true
                            });
                        }



                        Console.WriteLine("Canvas Width = {0}", this._actualCanvasWidth);

                    });

                    this.PuzzleCardsList.Last().IsGap = true;

                    Console.WriteLine("Pure Load data");

                    int j = 0;
                    foreach (var item in this._puzzleCardsList)
                    {
                        //Console.WriteLine("X_CanvasPosition = {0} Y_CanvasPosition = {1} index = {2} Canvas Width = {3} IsGap = {4} Real ImageIndex ={5} ", item.X_CanvasPosition, item.Y_CanvasPosition, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString(), j);
                        Console.WriteLine("ColumnIndex = {0} RowIndex = {1} index = {2} Canvas Width = {3} IsGap = {4} Real ImageIndex ={5} ", item.ColumnIndex, item.RowIndex, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString(), j++);

                    }

                    //this.SwapItems();

                    /*
                    foreach (var item in this._puzzleCardsList)
                    {
                        Console.WriteLine("X_CanvasPosition = {0} Y_CanvasPosition = {1} index = {2} Canvas Width = {3} IsGap = {4}", item.X_CanvasPosition, item.Y_CanvasPosition, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString());
                    }
                    */
                    //this.PuzzleCardsList.Add(new PuzzleImage(images[4], 10));
                    //this.PuzzleCardsList.Add(new PuzzleImage() { Image = images[4], X = 100, CanvasWidth = this._actualCanvasWidth });
                    //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
                    //Console.WriteLine("_PuzzleCardsList" + this._puzzleCardsList.Count);
                    //Console.WriteLine("this.puzzleWidth je 8888 " + this._actualCanvasWidth);
                    //Console.WriteLine("PuzzleCardsList " + this.PuzzleCardsList.Count);
                }



                Console.WriteLine("ObservableCollection Size" + this._puzzleCardsList.Count);
                //Console.WriteLine("FileName " + ofd.FileName);
            }
        }

        private IEnumerable<PuzzleImage> list;


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
                Filter = "All Image Files | *.bmp; *.dib; *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.gif; *.tif; *.tiff|" +
                         "BMP (*.bmp, *.dib)| *.bmp; *.dib|" +
                         "JPEG (*.jpg, *.jpeg, *.jpe, *.jfif,) | *.jpg; *.jpeg; *.jpe; *.jfif|" +
                         "GIF (*.gif)|*.gif|" +
                         "TIFF (*.tif, *.tiff)| *.tif; *.tiff|" +
                         "PNG (*.png)|*.png"
            };


            if (ofd.ShowDialog() == true)
            {
                ImageSplitter imgSplit;
                List<BitmapSource> images = new List<BitmapSource>();

                Console.WriteLine("ShowDialog ActualCanvasHeight" + this._actualCanvasHeight);
                

                try
                {
                    //imgSplit = new ImageSplitter(new Uri(ofd.FileName, UriKind.Absolute));
                    imgSplit = new ImageSplitter(this.aux);
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
                    int i = 0;
                    images.ForEach(e =>
                    {
                        if (i < 15)
                        {
                            this._puzzleCardsList.Add(new PuzzleImage()
                                {
                                    Image = e,
                                    
                                    CanvasWidth = this._actualCanvasWidth,
                                    CanvasHeight = this._actualCanvasHeight,
                                    ImageIndex = i,
                                    IsGap = false,
                                });
                            i++;
                        }
                        else
                        {
                            /*
                             * BitmapSource transparentImage = BitmapImage.Create(1, 1, 96, 96, PixelFormats.Bgra32, 
                                                                               null, new byte[] { 0, 0, 0, 0 }, 4);
                            
                             * */

                            int rawStride = (80 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
                            var rawImage = new byte[rawStride * 80];

                            BitmapSource transparentImage = BitmapSource.Create(80, 80, 96, 96, PixelFormats.Bgra32, 
                                                                               null, rawImage, rawStride);
                            this.PuzzleCardsList.Add(new PuzzleImage()
                                {
                                    Image = transparentImage,
                                    CanvasWidth = this._actualCanvasWidth,
                                    CanvasHeight = this._actualCanvasHeight,
                                    ImageIndex = i,
                                    IsGap = true
                                });
                        }
                        


                        Console.WriteLine("Canvas Width = {0}", this._actualCanvasWidth);
                        
                    });

                    this.PuzzleCardsList.Last().IsGap = true;

                    Console.WriteLine("Pure Load data");
                    
                    int j = 0;
                    foreach (var item in this._puzzleCardsList)
                    {
                        //Console.WriteLine("X_CanvasPosition = {0} Y_CanvasPosition = {1} index = {2} Canvas Width = {3} IsGap = {4} Real ImageIndex ={5} ", item.X_CanvasPosition, item.Y_CanvasPosition, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString(), j);
                        Console.WriteLine("ColumnIndex = {0} RowIndex = {1} index = {2} Canvas Width = {3} IsGap = {4} Real ImageIndex ={5} ", item.ColumnIndex, item.RowIndex, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString(), j++);
                    
                    }
                    
                    //this.SwapItems();

                    /*
                    foreach (var item in this._puzzleCardsList)
                    {
                        Console.WriteLine("X_CanvasPosition = {0} Y_CanvasPosition = {1} index = {2} Canvas Width = {3} IsGap = {4}", item.X_CanvasPosition, item.Y_CanvasPosition, item.ImageIndex, item.CanvasWidth, item.IsGap.ToString());
                    }
                    */
                    //this.PuzzleCardsList.Add(new PuzzleImage(images[4], 10));
                    //this.PuzzleCardsList.Add(new PuzzleImage() { Image = images[4], X = 100, CanvasWidth = this._actualCanvasWidth });
                    //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
                    //Console.WriteLine("_PuzzleCardsList" + this._puzzleCardsList.Count);
                    //Console.WriteLine("this.puzzleWidth je 8888 " + this._actualCanvasWidth);
                    //Console.WriteLine("PuzzleCardsList " + this.PuzzleCardsList.Count);
                }

                

                Console.WriteLine("ObservableCollection Size" + this._puzzleCardsList.Count);
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

        public ICommand Swap
        {
            get 
            {
                return new RelayCommand(() => this.SwapItems());
            }
        }

        private void SwapItems()
        {
            Console.WriteLine("SwapImageIndex index 11 is {0} X_CanvasPosition = {1} Y_CanvasPosition = {2} ", this._puzzleCardsList[11].ImageIndex, this.PuzzleCardsList[11].X_CanvasPosition, this.PuzzleCardsList[11].Y_CanvasPosition);
            Console.WriteLine("SwapImageIndex index 15 is {0} X_CanvasPosition = {1} Y_CanvasPosition = {2} ", this._puzzleCardsList[15].ImageIndex, this.PuzzleCardsList[15].X_CanvasPosition, this.PuzzleCardsList[15].Y_CanvasPosition);
            
            int aux = this._puzzleCardsList[11].ImageIndex;
            this._puzzleCardsList[11].ImageIndex = 15;
            this._puzzleCardsList[15].ImageIndex = aux;

            PuzzleImage im = this._puzzleCardsList[11];
            this._puzzleCardsList.Remove(im);
            this._puzzleCardsList.Add(im);

            Console.WriteLine("SwapImageIndex index 11 is {0} X_CanvasPosition = {1} Y_CanvasPosition = {2} ", this._puzzleCardsList[11].ImageIndex, this.PuzzleCardsList[11].X_CanvasPosition, this.PuzzleCardsList[11].Y_CanvasPosition);
            Console.WriteLine("SwapImageIndex index 15 is {0} X_CanvasPosition = {1} Y_CanvasPosition = {2} ", this._puzzleCardsList[15].ImageIndex, this.PuzzleCardsList[15].X_CanvasPosition, this.PuzzleCardsList[15].Y_CanvasPosition);

            //this.PuzzleCardsList.CollectionChanged += this.UpdatePuzzleCardList;
        }

        public void UpdatePuzzleCardList(object sender, NotifyCollectionChangedEventArgs e)
        {
            var list = sender as ObservableCollection<PuzzleImage>;
        }
        /*
        public ICommand SuffleCommand
        {
            get
            {
                //return new RelayCommand(() => this.SufflePuzzle());
                return new RelayCommand(() => { threadShuffle = new Thread(SufflePuzzle); threadShuffle.IsBackground = true; threadShuffle.Start(); });
            }
        }*/
        
        private void SufflePuzzle()
        {
            this.canNewGameExecute = false;
            changeCanExecuteDelegat changeCanExecuteDel = ChangeCanNewGameExecute;
            
            IList<PuzzleImage> listTmp = this.PuzzleCardsList;
            IList<PuzzleImage> listTmpShuffle = this.puzzleManager.Shuffle(listTmp);
            this.PuzzleCardsList = new ObservableCollection<PuzzleImage>(listTmpShuffle);

            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += new EventHandler(Each_Tick);
            this.timer.Start();

            this.canNewGameExecute = false;
            
        }

        public ICommand StartNewGameCommand
        {
            get 
            {
                return new RelayCommand(() => this.StartNewGameImpl(), () => this.canNewGameExecute);   
            }
        }

        public void StartNewGameImpl()
        {
            threadShuffle = new Thread(SufflePuzzle);
            threadShuffle.IsBackground = true;
            threadShuffle.Start();
            /*
            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += new EventHandler(Each_Tick);
            this.timer.Start();
            */
        }


        private void ChangeCanNewGameExecute()
        {
            this.canNewGameExecute = !this.canNewGameExecute;
        }

        public void Each_Tick(object o, EventArgs sender)
        {
            this.timeElapsed++;
            //TimeSpan t = new TimeSpan(this.timeElapsed);
            TimeSpan t2 = TimeSpan.FromSeconds(this.timeElapsed);
            //this.Time = String.Format(" : {0:00}", this.timeElapsed.ToString());
            this.Time = String.Format("{0:00}:{1:00}", t2.Minutes, t2.Seconds);
        }

        public ICommand TimerContinueCommand
        {
            get
            {
                return new RelayCommand(() => this.TimerContinueImpl());
            }
        }

        public void TimerContinueImpl()
        {
            this.timer.Start();
        }

        public ICommand TimerStopCommand
        {
            get
            {
                return new RelayCommand(() => this.TimerStopImpl());
            }
        }

        public void TimerStopImpl()
        {
            this.timer.Stop();
        }

        private void Parse()
        {

        }

        public ICommand OptionsDialogCommand
        {
            get
            {
                return new RelayCommand(() => this.OptionsDialogImpl());
            }
        }

        

        private void OptionsDialogImpl()
        {
            OptionsDialog od = new OptionsDialog();

            if (od.ShowDialog() == true)
            {
                
            }
        }


    }
}
