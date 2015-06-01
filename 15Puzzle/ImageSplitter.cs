using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace _15Puzzle
{
    public class ImageSplitter : NotSquareImageSizeException
    {
        private BitmapImage bitmapImageTmp;
        private double imageWidth;
        private double imageHeight;
        private Uri path;

        public ImageSplitter(BitmapImage image)
        {
            try
            {
                //this.bitmapImageTmp = new BitmapImage(fileName);
                this.bitmapImageTmp = image;
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

            this.imageWidth = bitmapImageTmp.PixelWidth;
            this.imageHeight = bitmapImageTmp.PixelHeight;

            if (imageWidth != imageHeight)
            {
                throw new NotSquareImageSizeException("NotSqureImageSizeException");
            }
        }

        public List<BitmapSource> SplitImage(int numberOfHorizontalChunks, int numberOfVerticalChunks)
        {
            BitmapImage bitmapImage;

            if ((this.bitmapImageTmp.Width % numberOfHorizontalChunks) != 0
                && (this.bitmapImageTmp.Height % numberOfVerticalChunks) != 0)
            {
                bitmapImage = this.ResizeImage(numberOfHorizontalChunks, numberOfVerticalChunks);
            }
            else
            {
                bitmapImage = this.bitmapImageTmp;
            }

            List<BitmapSource> listOfImageChunks = new List<BitmapSource>();

            int chunkWidth = (int)this.imageWidth / numberOfHorizontalChunks;
            int chunkHeight = (int)this.imageHeight / numberOfVerticalChunks;

            int stride = (int)bitmapImage.PixelHeight * ((bitmapImage.Format.BitsPerPixel + 7) / 8);
            int[] pixelArray = new int[(int)this.imageHeight * stride];

            Console.WriteLine("Stride " + stride);
            Console.WriteLine("PixelArray Length " + pixelArray.Length);

            for (int startY = 0; startY < this.imageHeight; startY += chunkHeight)
            {
                for (int startX = 0; startX < this.imageWidth; startX += chunkWidth)
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

        private BitmapImage ResizeImage(int numberOfHorizontalChunks, int numberOfVerticalChunks)
        {
            int imageWidthMod = (int)this.bitmapImageTmp.Width % numberOfHorizontalChunks;
            int imageHeightMod = (int)this.bitmapImageTmp.Height % numberOfVerticalChunks;

            int widthDifference = numberOfHorizontalChunks - imageWidthMod;
            int heightDifference = numberOfVerticalChunks - imageHeightMod;

            BitmapImage imageRet = new BitmapImage();

            imageRet.BeginInit();
            imageRet.UriSource = this.path;
            imageRet.DecodePixelWidth = (int)this.bitmapImageTmp.Width + widthDifference;
            imageRet.DecodePixelHeight = (int)this.bitmapImageTmp.Height + heightDifference;
            imageRet.EndInit();

            this.imageWidth = imageRet.PixelWidth;
            this.imageHeight = imageRet.PixelHeight;

            return imageRet;
        }

    }
}
