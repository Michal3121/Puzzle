using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _15Puzzle
{
    class PuzzleManager
    {
        private const int NUMBER_OF_SHUFFLING_CYCLES = 50;

        public IList<PuzzleImage> Swap(PuzzleImage puzzleClicked, IList<PuzzleImage> listOfPuzzle)
        {
            PuzzleImage gapImage = this.FindGapPuzzle(listOfPuzzle);

            if (this.HasGapLowerNeighbour(gapImage, listOfPuzzle))
            {
                PuzzleImage lowerImage = this.GetLowerNeighbour(gapImage, listOfPuzzle);
                if (lowerImage.Equals(puzzleClicked))
                {
                    return this.SwapImageIndex(listOfPuzzle, puzzleClicked.ImageIndex, gapImage.ImageIndex);
                }
            }

            if (this.HasGapUpperNeighbour(gapImage, listOfPuzzle))
            {
                PuzzleImage upperImage = this.GetUpperNeighbour(gapImage, listOfPuzzle);
                if (upperImage.Equals(puzzleClicked))
                {
                    return this.SwapImageIndex(listOfPuzzle, puzzleClicked.ImageIndex, gapImage.ImageIndex);
                }
            }

            if (this.HasGapRightNeighbour(gapImage, listOfPuzzle))
            {
                PuzzleImage rightImage = this.GetRightNeighbour(gapImage, listOfPuzzle);
                if (rightImage.Equals(puzzleClicked))
                {
                    return this.SwapImageIndex(listOfPuzzle, puzzleClicked.ImageIndex, gapImage.ImageIndex);
                }
            }

            if (this.HasGapLeftNeighbour(gapImage, listOfPuzzle))
            {
                PuzzleImage leftImage = this.GetLeftNeighbour(gapImage, listOfPuzzle);
                if (leftImage.Equals(puzzleClicked))
                {
                    return this.SwapImageIndex(listOfPuzzle, puzzleClicked.ImageIndex, gapImage.ImageIndex);
                }
            }

            return listOfPuzzle; 
        }

        public IList<PuzzleImage> Swap(string arrowKey, IList<PuzzleImage> listOfPuzzle)
        {
            PuzzleImage gapImage = this.FindGapPuzzle(listOfPuzzle);
            switch (arrowKey)
            {
                case "up":
                    if (this.HasGapLowerNeighbour(gapImage, listOfPuzzle))
                    {
                        PuzzleImage lowerImage = this.GetLowerNeighbour(gapImage, listOfPuzzle);
                        return this.SwapImageIndex(listOfPuzzle, lowerImage.ImageIndex, gapImage.ImageIndex);
                    }
                    break;
                case "down":
                    if (this.HasGapUpperNeighbour(gapImage, listOfPuzzle))
                    {
                        PuzzleImage upperImage = this.GetUpperNeighbour(gapImage, listOfPuzzle);
                        return this.SwapImageIndex(listOfPuzzle, upperImage.ImageIndex, gapImage.ImageIndex); 
                    }
                    break;
                case "left":
                    if (this.HasGapRightNeighbour(gapImage, listOfPuzzle))
                    {
                        PuzzleImage rightImage = this.GetRightNeighbour(gapImage, listOfPuzzle);
                        return this.SwapImageIndex(listOfPuzzle, rightImage.ImageIndex, gapImage.ImageIndex);
                    }
                    break;
                case "right":
                    if (this.HasGapLeftNeighbour(gapImage, listOfPuzzle))
                    {
                        PuzzleImage leftImage = this.GetLeftNeighbour(gapImage, listOfPuzzle);
                        return this.SwapImageIndex(listOfPuzzle, leftImage.ImageIndex, gapImage.ImageIndex);
                    }
                    break;
                default:
                    return listOfPuzzle;
            }

            return listOfPuzzle;
        }

        public IList<PuzzleImage> Shuffle(IList<PuzzleImage> listOfPuzzle)
        {
            IList<PuzzleImage> listOfPuzzleTmp = listOfPuzzle;
            //IList<PuzzleImage> listOfPuzzleTmp = new List<PuzzleImage>(listOfPuzzle);
            Random randNum = new Random();

            for (int i = 0; i < NUMBER_OF_SHUFFLING_CYCLES; i++ )
            {
                PuzzleImage gapImage = this.FindGapPuzzle(listOfPuzzleTmp);
                int random = randNum.Next(0, 4);
                Console.WriteLine("rand " + random);
                switch (random)
                {
                    case 0:
                        if (this.HasGapLowerNeighbour(gapImage, listOfPuzzle))
                        {
                            PuzzleImage lowerImage = this.GetLowerNeighbour(gapImage, listOfPuzzle);
                            listOfPuzzleTmp = this.SwapImageIndex(listOfPuzzle, lowerImage.ImageIndex, gapImage.ImageIndex);
                        }
                        break;
                    case 1:
                        if (this.HasGapUpperNeighbour(gapImage, listOfPuzzle))
                        {
                            PuzzleImage upperImage = this.GetUpperNeighbour(gapImage, listOfPuzzle);
                            listOfPuzzleTmp = this.SwapImageIndex(listOfPuzzle, upperImage.ImageIndex, gapImage.ImageIndex);
                        }
                        break;
                    case 2:
                        if (this.HasGapRightNeighbour(gapImage, listOfPuzzle))
                        {
                            PuzzleImage rightImage = this.GetRightNeighbour(gapImage, listOfPuzzle);
                            listOfPuzzleTmp = this.SwapImageIndex(listOfPuzzle, rightImage.ImageIndex, gapImage.ImageIndex);
                        }
                        break;
                    case 3:
                        if (this.HasGapLeftNeighbour(gapImage, listOfPuzzle))
                        {
                            PuzzleImage leftImage = this.GetLeftNeighbour(gapImage, listOfPuzzle);
                            listOfPuzzleTmp = this.SwapImageIndex(listOfPuzzle, leftImage.ImageIndex, gapImage.ImageIndex);
                        }
                        break;
                }
            }

            return listOfPuzzleTmp;
        }

        private PuzzleImage FindGapPuzzle(IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.IsGap == true).First();
        }

        private bool HasGapUpperNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == gapImage.ColumnIndex)
                               .Where(e => e.RowIndex == (gapImage.RowIndex - 1))
                               .Any();
        }

        private PuzzleImage GetUpperNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == gapImage.ColumnIndex)
                               .Where(e => e.RowIndex == (gapImage.RowIndex - 1))
                               .First();
        }

        private bool HasGapLowerNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == gapImage.ColumnIndex)
                               .Where(e => e.RowIndex == (gapImage.RowIndex + 1))
                               .Any();
        }

        private PuzzleImage GetLowerNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == gapImage.ColumnIndex)
                               .Where(e => e.RowIndex == (gapImage.RowIndex + 1))
                               .First();
        }

        private bool HasGapLeftNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == (gapImage.ColumnIndex - 1))
                               .Where(e => e.RowIndex == gapImage.RowIndex)
                               .Any();
        }

        private PuzzleImage GetLeftNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == (gapImage.ColumnIndex - 1))
                               .Where(e => e.RowIndex == gapImage.RowIndex)
                               .First();
        }

        private bool HasGapRightNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == (gapImage.ColumnIndex + 1))
                               .Where(e => e.RowIndex == gapImage.RowIndex)
                               .Any();
        }

        private PuzzleImage GetRightNeighbour(PuzzleImage gapImage, IList<PuzzleImage> listOfPuzzle)
        {
            return listOfPuzzle.Where(e => e.ColumnIndex == (gapImage.ColumnIndex + 1))
                               .Where(e => e.RowIndex == gapImage.RowIndex)
                               .First();
        }

        private IList<PuzzleImage> SwapImageIndex(IList<PuzzleImage> list, int imageIndexClicked, int imageIndexGap)
        {
            PuzzleImage clickedImage = list.Where(e => e.ImageIndex == imageIndexClicked).First();
            int RealClickedIndex = list.IndexOf(clickedImage);
            PuzzleImage gapImage = list.Where(e => e.ImageIndex == imageIndexGap).First();
            int RealGapIndex = list.IndexOf(gapImage);

            int tmp_ImageIndex = list[RealClickedIndex].ImageIndex;
            list[RealClickedIndex].ImageIndex = list[RealGapIndex].ImageIndex;
            list[RealGapIndex].ImageIndex = tmp_ImageIndex;
                        
            return list;
        }

    }
}
