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
        private long timeElapsed;
        private DispatcherTimer timer;
        private PuzzleManager puzzleManager;
        Thread threadShuffle;
        private delegate void changeCanExecuteDelegat();
        private BitmapImage imageWood;
        private BitmapImage imageGrey;
        private BitmapImage imagePaint;
        private BitmapImage imageRafael;
        private int difficulty;
        private int difficultyIndex;
        private BitmapImage defaultImage;
        private int defaultImageIndex;
        
        public MainViewModel()
        {
            this.LoadRegistry();
            this.timeElapsed = 0;
            this.timer = new DispatcherTimer();
            this._actualCanvasHeight = 320;
            this._actualCanvasWidth = 320;
            this.puzzleManager = new PuzzleManager();
            this.imageWood = new BitmapImage(new Uri("pack://application:,,,/Images/Wood.jpg", UriKind.Absolute));
            this.imageGrey = new BitmapImage(new Uri("pack://application:,,,/Images/Grey.jpg", UriKind.Absolute));
            this.imagePaint = new BitmapImage(new Uri("pack://application:,,,/Images/Paint.jpg", UriKind.Absolute));
            this.imageRafael = new BitmapImage(new Uri("pack://application:,,,/Images/Rafael.jpg", UriKind.Absolute));
            this.difficulty = this.GetDifficultyValue(this.difficultyIndex);
            this.defaultImage = this.GetImage(this.defaultImageIndex);
            this._currentImageIndex = this.defaultImageIndex;
            this._currentImage = this.defaultImage;
            this._puzzleCardsList = this.Image2Collection(this._currentImage);
        }

        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get { return this._currentImage; }
            set { this._currentImage = value; }
        }

        private bool _canNewGameExecute = true;
        public bool CanNewGameExecute
        {
            get { return this._canNewGameExecute; }
            set 
            {
                if (this._canNewGameExecute != value)
                {
                    this._canNewGameExecute = value;
                    RaisePropertyChanged("CanNewGameExecute");
                }
            }
        }

        private bool _gameStarted = false;
        public bool GameStarted
        {
            get { return this._gameStarted; }
            set
            {
                if (this._gameStarted != value)
                {
                    this._gameStarted = value;
                    RaisePropertyChanged("GameStarted");
                }
            }
        }

        private bool _gamePaused = false;
        public bool GamePaused
        {
            get { return this._gamePaused; }
            set 
            {
                if (this._gamePaused != value)
                {
                    this._gamePaused = value;
                    RaisePropertyChanged("GamePaused");
                }
            }
        }

        private double _actualCanvasWidth = 100;
        public double ActualCanvasWidth
        {
            get { return this._actualCanvasWidth; }
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
                }
            }
        }

        private string _shufflingLabel = " ";
        public string ShufflingLabel
        {
            get { return this._shufflingLabel; }
            set 
            {
                if (this._shufflingLabel != value)
                {
                    this._shufflingLabel = value;
                    RaisePropertyChanged("ShufflingLabel");
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

        private ObservableCollection<PuzzleImage> _puzzleCardsList = new ObservableCollection<PuzzleImage>(); 
        public ObservableCollection<PuzzleImage> PuzzleCardsList
        {
            get { return this._puzzleCardsList; }
            set 
            {
                if (this._puzzleCardsList != value )
                {
                    this._puzzleCardsList = value;
                    RaisePropertyChanged("PuzzleCardsList");
                    if (this.IsPuzzleSolved(this._puzzleCardsList) && this.GameStarted)
                    {
                        this.GameStarted = false;
                        this.TimerStopImpl();
                        MessageBox.Show("Congratulations :) ! The Puzzle is solved. Your time is " + this.Time, 
                            "Solved", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }        
            }
        }

        private ObservableCollection<string> _listOfImages = new ObservableCollection<string>(Enum.GetNames(typeof(Images)));
        public ObservableCollection<string> ListOfImages
        {
            get { return this._listOfImages; }
        }

        private int _currentImageIndex;
        public int CurrentImageIndex
        {
            get { return this._currentImageIndex; }
            set 
            {
                if (this._currentImageIndex != value)
                {
                    this.GameStarted = false;
                    this.GamePaused = false;
                    this.TimerReset();
                    this.ShufflingLabel = " ";
                    this._currentImageIndex = value;
                    this.CurrentImage = this.GetImage(this._currentImageIndex);
                    this.PuzzleCardsList = this.Image2Collection(this.CurrentImage);
                    RaisePropertyChanged("CurrentImageIndex");
                }
            }
        }

        private void LoadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey
                ("Software\\FIMU\\15Puzzle");

            this.difficultyIndex = (int)key.GetValue("DefaultDifficulty", 0);
            this.defaultImageIndex = (int)key.GetValue("DefaultImage", 0);

            key.Close();
        }

        public void SaveRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("Software\\FIMU\\15Puzzle", true);

            key.SetValue("DefaultDifficulty", this.difficultyIndex);
            key.SetValue("DefaultImage", this.defaultImageIndex);

            key.Close();
        }

        private ObservableCollection<PuzzleImage> Image2Collection(BitmapImage image)
        { 
            ImageSplitter imageSplitter;
            List<BitmapSource> images = new List<BitmapSource>();
            ObservableCollection<PuzzleImage> puzzleImagesList = new ObservableCollection<PuzzleImage>();

            imageSplitter = new ImageSplitter(image);
            images = imageSplitter.SplitImage(4, 4);
                
            if (images.Any())
            {
                int i = 0;
                images.ForEach(e =>
                {
                    if (i < 15)
                    {
                        puzzleImagesList.Add(new PuzzleImage()
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
                        int rawStride = (80 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
                        var rawImage = new byte[rawStride * 80];

                        BitmapSource transparentImage = BitmapSource.Create(80, 80, 96, 96, PixelFormats.Bgra32,
                                                                            null, rawImage, rawStride);
                        puzzleImagesList.Add(new PuzzleImage()
                        {
                            Image = transparentImage,
                            CanvasWidth = this._actualCanvasWidth,
                            CanvasHeight = this._actualCanvasHeight,
                            ImageIndex = i,
                            IsGap = true
                        });
                    }
                });

                puzzleImagesList.Last().IsGap = true;
            }

            return puzzleImagesList;
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
                return new RelayCommand(() => this.OpenFile(), () => this.CanNewGameExecute);
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
                try
                {
                    BitmapImage loadImage = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                    this.PuzzleCardsList = this.Image2Collection(loadImage);
                }
                catch (ArgumentException ex1)
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
                }
                catch (NotSquareImageSizeException)
                {
                    MessageBox.Show("Image does not have square size!");
                }
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand(() => Application.Current.Shutdown());
            }
        }

        private void SufflePuzzle()
        {
            this.CanNewGameExecute = false;
            changeCanExecuteDelegat changeCanExecuteDel = ChangeCanNewGameExecute;

            this.TimerReset();
            this.ShufflingLabel = "Shuffling Puzzle...";

            
            IList<PuzzleImage> listTmp = this.PuzzleCardsList;
            IList<PuzzleImage> listTmpShuffle = this.puzzleManager.Shuffle(listTmp, this.difficulty);
            this.GameStarted = true;
            this.PuzzleCardsList = new ObservableCollection<PuzzleImage>(listTmpShuffle);

            this.TimerStart();

            this.CanNewGameExecute = true;
            this.ShufflingLabel = " ";   
        }

        private bool IsPuzzleSolved(ObservableCollection<PuzzleImage> puzzleImagesList)
        {
            int auxIndex = 0;
            foreach (var item in puzzleImagesList)
            {
                if (item.ImageIndex != auxIndex)
                {
                    return false;
                }
                auxIndex++;
            }
            return true;
        }

        public ICommand StartNewGameCommand
        {
            get 
            {
                return new RelayCommand(() => this.StartNewGameImpl(), () => this.CanNewGameExecute);   
            }
        }

        public void StartNewGameImpl()
        {
            threadShuffle = new Thread(SufflePuzzle);
            threadShuffle.IsBackground = true;
            threadShuffle.Start();
        }

        public void TimerStart()
        {
            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += new EventHandler(Each_Tick);
            this.timer.Start();
        }

        private void ChangeCanNewGameExecute()
        {
            this.CanNewGameExecute = !this.CanNewGameExecute;
        }

        public void Each_Tick(object o, EventArgs sender)
        {
            this.timeElapsed++;
            TimeSpan t = TimeSpan.FromSeconds(this.timeElapsed);
            this.Time = String.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }

        public void TimerReset()
        {
            this.timeElapsed = 0;
            this.Time = String.Format("  :  ");
            this.timer.Tick -= new EventHandler(Each_Tick);
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
            this.GamePaused = false;
            this.GameStarted = true;
            this.ShufflingLabel = " ";
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
            this.GamePaused = true;
            this.GameStarted = false;
            this.ShufflingLabel = "Game Paused";
        }


        public ICommand OptionsDialogCommand
        {
            get
            {
                return new RelayCommand(() => this.OptionsDialogImpl(), () => this.CanNewGameExecute);
            }
        }

        private int GetDifficultyValue(int n)
        {
            switch (n)
            {
                case 0:
                    return 20;
                case 1:
                    return 50;
                case 2:
                    return 100;
                case 3:
                    return 200;
            }
            return 50;
        }

        private BitmapImage GetImage(int n)
        {
            switch (n)
            {
                case 0:
                    return this.imageWood;
                case 1:
                    return this.imageGrey;
                case 2:
                    return this.imagePaint;
                case 3:
                    return this.imageRafael;
            }
            return this.imageWood;
        }

        private void OptionsDialogImpl()
        {
            OptionsDialog optionsDialog = new OptionsDialog() {SelectedDifficulty = this.difficultyIndex, SelectedImage = this.defaultImageIndex }; 
             
            this.TimerStopImpl();

            if (optionsDialog.ShowDialog() == true)
            {
                this.difficultyIndex = optionsDialog.SelectedDifficulty;
                this.difficulty = this.GetDifficultyValue(this.difficultyIndex);
                this.defaultImageIndex = optionsDialog.SelectedImage;
                this.defaultImage = this.GetImage(this.defaultImageIndex);
            }

            this.TimerContinueImpl();
        }

        public ICommand AboutDialogCommand
        {
            get
            {
                return new RelayCommand(() => this.AboutDialogImpl());
            }
        }

        private void AboutDialogImpl()
        {
            About about = new About();

            about.ShowDialog();
        }
    }
}
