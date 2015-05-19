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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _15Puzzle
{
    public class ImageSplitter : NotSquareImageSizeException
    {
        private BitmapImage bitmapImage;
        private double imageWidth;
        private double imageHeight;

        public ImageSplitter(Uri fileName)
        {
            try
            {
                this.bitmapImage = new BitmapImage(fileName);
            }
            catch (NotSupportedException ex1)
            {
                if (ex1.Source != null)
                    Console.WriteLine("Exception source: {0} TypeExcepion: {1}", ex1.Source, ex1.GetType().FullName);
                throw new NotSupportedException(ex1.Source, ex1);
            }
            /*catch (FileNotFoundException ex2)
            {
                if (ex2.Source != null)
                    Console.WriteLine("Exception source: {0} TypeExcepion: {1}", ex2.Source, ex2.GetType().FullName);
                throw new FileNotFoundException(ex2.Source, ex2);
            }
            catch (Exception ex3)
            {
                if (ex3.Source != null)
                    Console.WriteLine("Exception source: {0} TypeExcepion: {1}", ex3.Source, ex3.GetType().FullName);
                throw new ArgumentNullException(ex3.Source, ex3);
            }*/

            this.imageWidth = bitmapImage.Width;
            this.imageHeight = bitmapImage.Height;

            if (imageWidth != imageHeight)
            {
                throw new NotSquareImageSizeException("NotSqureImageSizeException");
            }
        }

        public List<BitmapSource> SplitImage(int numberOfHorizontalChunks, int numberOfVerticalChunks)
        {
            List<BitmapSource> listOfImageChunks = new List<BitmapSource>();

            int chunkWidth = (int)this.imageWidth / numberOfHorizontalChunks;
            int chunkHeight = (int)this.imageHeight / numberOfVerticalChunks;

            int stride = (int)bitmapImage.PixelHeight * ((bitmapImage.Format.BitsPerPixel + 7) / 8);
            int[] pixelArray = new int[(int)this.imageHeight * stride];

            Console.WriteLine("Stride " + stride);
            Console.WriteLine("PixelArray Length " + pixelArray.Length);

            for (int startX = 0; startX < this.imageWidth; startX += chunkWidth)
            {
                for (int startY = 0; startY < this.imageHeight; startY += chunkHeight)
                {
                    Int32Rect sourceRect = new Int32Rect(startX, startY, chunkWidth, chunkHeight);
                    bitmapImage.CopyPixels(sourceRect, pixelArray, stride, 0);
                    BitmapSource imageChunk = BitmapSource.Create(
                                                sourceRect.Width,
                                                sourceRect.Height,
                                                bitmapImage.DpiX,
                                                bitmapImage.DpiY,
                                                bitmapImage.Format,
                                                bitmapImage.Palette,
                                                pixelArray, stride);
                    listOfImageChunks.Add(imageChunk);
                }
            }
            Console.WriteLine("Pocet " + listOfImageChunks.Count);
            return listOfImageChunks;
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
