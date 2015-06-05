using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
        #region Class Attributes
        private long timeElapsed;
        private DispatcherTimer timer;
        private PuzzleManager puzzleManager;
        Thread threadShuffle;
        private delegate void changeCanExecuteDelegat();
        private BitmapImage imageWood;
        private BitmapImage currentImage;
        private string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private Dictionary<string, BitmapImage> _loadedImages;
        //private string path = Directory.GetCurrentDirectory();
        private string imagesDirectoryPath;
        #endregion

        public MainViewModel()
        {
            this.imagesDirectoryPath = imagesDirectoryPath = String.Format("{0}\\Images", path);
            this._loadedImages = this.LoadImageFromFolder(this.imagesDirectoryPath);
            this._loadedImagesNames = new List<string>(this._loadedImages.Keys);
            //this.LoadedImagesNamesForOptions = this._loadedImagesNames;
            this.LoadedImagesNamesForOptions = new List<string>(this._loadedImages.Keys);
            this.LoadRegistry();
            this.timeElapsed = 0;
            this.timer = new DispatcherTimer();
            this._actualCanvasHeight = 320;
            this._actualCanvasWidth = 320;
            this.puzzleManager = new PuzzleManager();
            this.Background = new BitmapImage(new Uri("pack://application:,,,/Images/Background.bmp", UriKind.Absolute));
            this.imageWood = new BitmapImage(new Uri("pack://application:,,,/Images/Wood.jpg", UriKind.Absolute)); // using as default image
            this._difficulty = this.GetDifficultyValue(this._difficultyIndex);
            this.currentImage = this.GetImage(this._defaultImageName);
            this.SelectedImageValue = this._defaultImageName;
            this._puzzleCardsList = this.Image2Collection(this.currentImage);
        }

        public BitmapImage Background { get; private set; }

        public List<string> LoadedImagesNamesForOptions { get; private set; }

        private List<string> _loadedImagesNames;
        public List<string> LoadedImagesNames
        {
            get { return new List<string>(this._loadedImages.Keys); }
            set 
            {
                if (this._loadedImagesNames != value)
                {
                    this._loadedImagesNames = value;
                    RaisePropertyChanged("LoadedImagesNames");
                }
            }
        }

        private int _difficultyIndex;
        public int DifficultyIndex
        {
            get { return this._difficultyIndex; }
            set 
            {
                if (this._difficultyIndex != value)
                {
                    this._difficultyIndex = value;
                    RaisePropertyChanged("DifficultyIndex");
                }
            }
        }

        private int _difficulty;
        public int Difficulty
        {
            get { return this._difficulty; }
            set 
            {
                if (this._difficulty != value)
                {
                    this._difficulty = value;
                    RaisePropertyChanged("Difficulty");
                }
            }
        }

        private string _defaultImageName;
        public string DefaultImageName
        {
            get { return this._defaultImageName; }
            set 
            {
                if (this._defaultImageName != value)
                {
                    this._defaultImageName = value;
                    RaisePropertyChanged("DefaultImageName");
                }
            }
        }

        private BitmapImage _defaultImage;
        public BitmapImage DefaultImage
        {
            get { return this._defaultImage; }
            set 
            {
                if (this._defaultImage != value)
                {
                    this._defaultImage = value;
                    RaisePropertyChanged("DefaultImage");
                }
            }
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

        public bool _gamePaused = false;
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

        private bool _gameContinue = false;
        public bool GameContinue
        {
            get { return this._gameContinue; }
            set
            {
                if (this._gameContinue != value)
                {
                    this._gameContinue = value;
                    RaisePropertyChanged("GameContinue");
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
                        this.TimerStopImpl();
                        this.GameStarted = false;
                        this.GamePaused = false;
                        this.GameContinue = false;
                        this.ShufflingLabel = "Puzzle Solved";
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

        public string _selectedImageValue;
        public string SelectedImageValue
        {
            get { return this._selectedImageValue; }
            set 
            {
                if (this._selectedImageValue != value)
                {
                    this.GameStarted = false;
                    this.GamePaused = false;
                    this.GameContinue = false;
                    this.TimerReset();
                    this.ShufflingLabel = " ";
                    this._selectedImageValue = value;
                    this.currentImage = this.GetImage(this._selectedImageValue);
                    this.PuzzleCardsList = this.Image2Collection(this.currentImage);
                    RaisePropertyChanged("SelectedImageValue");
                }
            }
        }

        #region Registry
        private void LoadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey
                ("Software\\FIMU\\15Puzzle");

            this._difficultyIndex = (int)key.GetValue("DefaultDifficulty", 0);
            this._defaultImageName = (string)key.GetValue("DefaultImageName", "Wood");

            key.Close();
        }

        public void SaveRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("Software\\FIMU\\15Puzzle", true);

            key.SetValue("DefaultDifficulty", this._difficultyIndex);
            key.SetValue("DefaultImageName", this._defaultImageName);

            key.Close();
        }
        #endregion

        public int GetDifficultyValue(int n)
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

        private BitmapImage GetImage(string name)
        {
            BitmapImage imageRet;
            if (this._loadedImages.TryGetValue(name, out imageRet))
            {
                return imageRet;
            }
            this.SelectedImageValue = "Wood";
            return imageWood;
        }

        private Dictionary<string, BitmapImage> LoadImageFromFolder(string path)
        {
            Dictionary<string, BitmapImage> loadedImages = new Dictionary<string, BitmapImage>();
            string name = "";

            string[] allImagesWithFullPath = Directory.GetFiles(this.imagesDirectoryPath);

            foreach (string item in allImagesWithFullPath)
            {
                try
                {
                    name = Path.GetFileNameWithoutExtension(item);
                    BitmapImage image = new BitmapImage(new Uri(item, UriKind.Absolute));
                    int imageWidth = image.PixelWidth;
                    int imageHeight = image.PixelHeight;

                    if (imageWidth != imageHeight)
                    {
                        throw new NotSquareImageSizeException("NotSqureImageSizeException");
                    }

                    if (loadedImages.ContainsKey(name))
                    {
                        throw new ImagesHaveSameNamesException("SameNameException");
                    }
                    loadedImages.Add(name, image);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(String.Format("Image {0} could not be loaded.", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (UriFormatException)
                {
                    MessageBox.Show(String.Format("Image {0} could not be loaded.", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show(String.Format("Image {0} could not be displayed. Not supported format.", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NotSquareImageSizeException)
                {
                    MessageBox.Show(String.Format("Image {0} does not have square size!", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (ImagesHaveSameNamesException)
                {
                    MessageBox.Show(String.Format("Image with name {0} already exists!", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show(String.Format("Image {0} could not be loaded.", name), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

            return loadedImages;
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

        #region Commands
        public ICommand OpenCommand
        {
            get
            {
                return new RelayCommand(() => this.OpenFile(), () => this.CanNewGameExecute);
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand(() => Application.Current.Shutdown());
            }
        }

        public ICommand StartNewGameCommand
        {
            get
            {
                return new RelayCommand(() => this.StartNewGameImpl(), () => this.CanNewGameExecute);
            }
        }

        public ICommand TimerContinueCommand
        {
            get
            {
                return new RelayCommand(() => this.TimerContinueImpl());
            }
        }

        public ICommand TimerStopCommand
        {
            get
            {
                return new RelayCommand(() => this.TimerStopImpl());
            }
        }
        #endregion

        #region CommandImpl
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
                    this.GameStarted = false;
                    this.GamePaused = false;
                    this.GameContinue = false;
                    this._loadedImages.Remove("Custom");
                    this.currentImage = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                    //BitmapImage imageCustom = currentImage;
                    BitmapImage imageCustom = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                    this._loadedImages.Add("Custom", imageCustom);
                    this.LoadedImagesNames = new List<string>(this._loadedImages.Keys);
                    this.SelectedImageValue = "Custom";
                    this.TimerReset();
                    this.ShufflingLabel = " ";
                    this.PuzzleCardsList = this.Image2Collection(this.currentImage);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Image could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("Image could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("Image could not be displayed. Not supported format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NotSquareImageSizeException)
                {
                    MessageBox.Show("Image does not have square size!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("Image could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
 
        private void SufflePuzzle()
        {
            this.CanNewGameExecute = false;
            changeCanExecuteDelegat changeCanExecuteDel = ChangeCanNewGameExecute;

            this.TimerReset();
            this.ShufflingLabel = "Shuffling Puzzle...";
            
            IList<PuzzleImage> listTmp = this.PuzzleCardsList;
            IList<PuzzleImage> listTmpShuffle = this.puzzleManager.Shuffle(listTmp, this._difficulty);
            this.GameStarted = true;
            this.PuzzleCardsList = new ObservableCollection<PuzzleImage>(listTmpShuffle);

            this.TimerStart();

            this.CanNewGameExecute = true;
            this.GamePaused = true;
            this.GameContinue = false;
            this.ShufflingLabel = " ";   
        }

        private void ChangeCanNewGameExecute()
        {
            this.CanNewGameExecute = !this.CanNewGameExecute;
        }

        public void StartNewGameImpl()
        {
            if (!this.IsPuzzleSolved(this._puzzleCardsList))
            {
                this._puzzleCardsList = this.Image2Collection(this.currentImage);
            }
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

        public void TimerContinueImpl()
        {
            this.GamePaused = true;
            this.GameStarted = true;
            this.GameContinue = false;
            this.ShufflingLabel = " ";
            if (!this.IsPuzzleSolved(this._puzzleCardsList)) this.timer.Start();
        }

        public void TimerStopImpl()
        {
            this.timer.Stop();
            this.GamePaused = false;
            this.GameStarted = false;
            this.GameContinue = true;
            this.ShufflingLabel = "Game Paused";
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
        #endregion

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

    }
}
