using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace _15Puzzle
{
    public class PuzzleImage : INotifyPropertyChanged 
    {
        public BitmapSource Image { get; set; }
        
        private const int COLUMNS_COUNT = 4;
        private const int ROWS_COUNT = 4;
        private const int SIZE_OF_BOUNDARY_LINE = 1; // Size of boundary line beween puzzles

        private int _imageIndex;
        public int ImageIndex
        {
            get { return this._imageIndex; }
            set 
            {
                if (this._imageIndex != value)
                {
                    this._imageIndex = value;
                    this._columnIndex = this.ImageIndex % COLUMNS_COUNT;
                    this._rowIndex = (this.ImageIndex - this._columnIndex) / COLUMNS_COUNT;
                    this.UpdateY();
                    this.UpdateX();
                    this.NotifyPropertyChanged("CurrentImageIndex");
                }
            }
        }

        private int _columnIndex;
        public int ColumnIndex 
        {
            get { return this._columnIndex; }
        }

        private int _rowIndex;
        public int RowIndex 
        {
            get { return this._rowIndex; }
        }

        private double _canvasWidth;
        public double CanvasWidth
        {
            get { return this._canvasWidth; }
            set 
            {
                this._canvasWidth = value;
            }
        }

        private double _canvasHeight;
        public double CanvasHeight
        {
            get { return this._canvasHeight; }
            set 
            { 
                this._canvasHeight = value; 
            }
        }

        private double _x_CanvasPosition;
        public double X_CanvasPosition 
        {
            get { return this._x_CanvasPosition; }
        }

        private double _y_CanvasPosition = 0;
        public double Y_CanvasPosition 
        {
            get { return this._y_CanvasPosition; }
        }

        private void UpdateX()
        {
            int columnIndex = this.ImageIndex % COLUMNS_COUNT;
            double imageWidth = this._canvasWidth / COLUMNS_COUNT;

            this._x_CanvasPosition = (imageWidth) * columnIndex + (columnIndex * SIZE_OF_BOUNDARY_LINE);
        }

        private void UpdateY()
        {
            int columnIndex = this.ImageIndex % COLUMNS_COUNT;
            int rowIndex = (this.ImageIndex - columnIndex) / COLUMNS_COUNT;
            double imageHeight = this._canvasHeight / ROWS_COUNT;
            
            this._y_CanvasPosition = (imageHeight) * rowIndex + (rowIndex * SIZE_OF_BOUNDARY_LINE);
        }

        public bool IsGap
        {
            get;
            set;
        }
        
        

        

        /*
        public PuzzleImage(BitmapSource image, double x)
        {
            this.Image = image;
            this.X = x;
        }*/

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
    }
}
